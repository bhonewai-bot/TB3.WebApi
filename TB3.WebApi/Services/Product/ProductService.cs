using TB3.Models;

namespace TB3.WebApi.Services.Product;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;
    private readonly ISequenceService _sequenceService;

    public ProductService(AppDbContext db, ISequenceService sequenceService)
    {
        _db = db;
        _sequenceService = sequenceService;
    }

    public async Task<ProductGetResponseDto> GetProducts(int pageNo = 1, int pageSize = 10)
    {
        if (pageNo == 0)
        {
            return new ProductGetResponseDto()
            {
                IsSuccess = false,
                Message = "Page number must be greater than zero.",
            };
        }

        if (pageSize == 0)
        {
            return new ProductGetResponseDto()
            {
                IsSuccess = false,
                Message = "Page size must be greater than zero.",
            };
        }
        
        var products = await _db.TblProducts
            .AsNoTracking()
            .OrderByDescending(x => x.ProductId)
            .Where(x => x.DeleteFlag == false)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new ProductDto()
            {
                ProductId = item.ProductId,
                ProductCode = item.ProductCode,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity,
                ProductCategoryCode = item.ProductCategoryCode,
            })
            .ToListAsync();

        return new ProductGetResponseDto()
        {
            IsSuccess = true,
            Message = "Success.",
            Products = products,
        };
    }

    public async Task<ProductGetByCodeResponseDto> GetProductByCode(string productCode)
    {
        var product = await _db.TblProducts
            .AsNoTracking()
            .Where(x => x.ProductCode == productCode && x.DeleteFlag == false)
            .Select(item => new ProductDto()
            {
                ProductId = item.ProductId,
                ProductCode = item.ProductCode,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity,
                ProductCategoryCode = item.ProductCategoryCode,
            })
            .FirstOrDefaultAsync();

        if (product is null)
        {
            return new ProductGetByCodeResponseDto()
            {
                IsSuccess = false,
                Message = "Product not found."
            };
        }

        return new ProductGetByCodeResponseDto()
        {
            IsSuccess = true,
            Message = "Success.",
            Product = product,
        };
    }

    public async Task<ProductResponseDto> CreateProduct(ProductCreateRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductName))
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Product name is required"
            };
        }

        if (request.Price <= 0)
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Price must be greater than zero"
            };
        }
        
        if (request.Quantity <= 0)
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Quantity must be greater than zero"
            };
        }
        
        if (string.IsNullOrWhiteSpace(request.ProductCategoryCode))
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Product name is required"
            };
        }
        
        var nameExists = await _db.TblProducts
            .AnyAsync(x => x.ProductName == request.ProductName);

        if (nameExists)
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Product name already exists."
            };
        }
        
        var categoryExists = await _db.TblProductCategories
            .AnyAsync(x => x.ProductCategoryCode == request.ProductCategoryCode);

        if (!categoryExists)
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Invalid product category code."
            };
        }
        
        string productCode = await _sequenceService.GenerateCode("ProductCode");

        var product = new TblProduct()
        {
            ProductCode = productCode,
            ProductName = request.ProductName,
            Price = request.Price,
            Quantity = request.Quantity,
            ProductCategoryCode = request.ProductCategoryCode,
            CreatedDateTime = DateTime.Now,
            DeleteFlag = false
        };
        
        _db.TblProducts.Add(product);
        await _db.SaveChangesAsync();
        
        return new ProductResponseDto()
        {
            IsSuccess = true,
            Message = "Product created successfully.",
            Product = new ProductDto()
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = product.Quantity,
                ProductCategoryCode = product.ProductCategoryCode
            }
        };
    }

    public async Task<ProductResponseDto> UpdateProduct(int id, ProductUpdateRequestDto request)
    {
        var product = await _db.TblProducts
            .FirstOrDefaultAsync(x => x.ProductId == id);

        if (product is null)
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Product not found."
            };
        }
        
        product.ProductCode = request.ProductCode;
        product.ProductName = request.ProductName;
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        product.ProductCategoryCode = request.ProductCategoryCode;
        product.ModifiedDateTime = DateTime.Now;
        product.DeleteFlag = false;

        await _db.SaveChangesAsync();
        return new ProductResponseDto()
        {
            IsSuccess = true,
            Message = "Product updated successfully.",
            Product = new ProductDto()
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = product.Quantity,
                ProductCategoryCode = product.ProductCategoryCode
            }
        };

    }

    public async Task<ProductResponseDto> PatchProduct(int id, ProductPatchRequestDto request)
    {
        var product = await _db.TblProducts
            .FirstOrDefaultAsync(x => x.DeleteFlag == false && x.ProductId == id);

        if (product is null)
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Product not found."
            };
        }

        bool isUpdated = false;

        if (!string.IsNullOrEmpty(request.ProductCode))
        {
            product.ProductCode = request.ProductCode;
            isUpdated = true;
        }

        if (!string.IsNullOrEmpty(request.ProductName))
        {
            product.ProductName = request.ProductName;
            isUpdated = true;
        }

        if (request.Price is not null && request.Price > 0)
        {
            product.Price = request.Price ?? 0;
            isUpdated = true;
        }

        if (request.Quantity is not null && request.Quantity > 0)
        {
            product.Quantity = request.Quantity ?? 0;
            isUpdated = true;
        }

        if (!string.IsNullOrEmpty(request.ProductCategoryCode))
        {
            product.ProductCategoryCode = request.ProductCategoryCode;
            isUpdated = true;
        }

        if (!isUpdated)
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Invalid action."
            };
        }
        
        product.ModifiedDateTime = DateTime.Now;

        await _db.SaveChangesAsync();
        return new ProductResponseDto()
        {
            IsSuccess = true,
            Message = "Product patched successfully.",
            Product = new ProductDto()
            {
                ProductId = product.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = product.Quantity,
                ProductCategoryCode = product.ProductCategoryCode
            }
        };
    }

    public async Task<ProductResponseDto> DeleteProduct(int id)
    {
        var product = await _db.TblProducts
            .FirstOrDefaultAsync(x => x.DeleteFlag == false && x.ProductId == id);

        if (product is null)
        {
            return new ProductResponseDto()
            {
                IsSuccess = false,
                Message = "Product not found."
            };
        }            
        
        product.DeleteFlag = true;
        product.ModifiedDateTime = DateTime.Now;

        await _db.SaveChangesAsync();
        return new ProductResponseDto()
        {
            IsSuccess = true,
            Message = "Product deleted successfully."
        };
    }
}