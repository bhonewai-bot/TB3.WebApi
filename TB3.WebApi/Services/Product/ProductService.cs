namespace TB3.WebApi.Services.Product;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductResponseDto>> GetProducts(string? productCategoryCode)
    {
        var query = _db.TblProducts
            .AsNoTracking()
            .Where(x => x.DeleteFlag == false);

        if (!string.IsNullOrEmpty(productCategoryCode))
        {
            query = query.Where(x => x.ProductCategoryCode == productCategoryCode);
        }
            
        var product = await query
            .OrderByDescending(x => x.ProductId)
            .Select(x => new ProductResponseDto()
            {
                ProductId = x.ProductId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                Price = x.Price,
                Quantity = x.Quantity,
                ProductCategoryCode = x.ProductCategoryCode,
                CreatedDateTime = x.CreatedDateTime,
                ModifiedDateTime = x.ModifiedDateTime
            })
            .ToListAsync();

        return product;
    }

    public async Task<ProductResponseDto?> GetProduct(int id)
    {
        var product = await _db.TblProducts
            .AsNoTracking()
            .Where(x => x.ProductId == id && x.DeleteFlag == false)
            .Select(x => new ProductResponseDto()
            {
                ProductId = x.ProductId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                Price = x.Price,
                Quantity = x.Quantity,
                ProductCategoryCode = x.ProductCategoryCode,
                CreatedDateTime = x.CreatedDateTime,
                ModifiedDateTime = x.ModifiedDateTime
            })
            .FirstOrDefaultAsync();

        return product;
    }

    public async Task<ProductResponseDto?> CreateProduct(ProductCreateRequestDto request)
    {
        var exists = await _db.TblProducts
            .AnyAsync(x => x.ProductCode == request.ProductCode);

        if (exists)
        {
            return null;
        }

        var product = new TblProduct()
        {
            ProductCode = request.ProductCode,
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
            ProductId = product.ProductId,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            Price = product.Price,
            Quantity = product.Quantity,
            ProductCategoryCode = product.ProductCategoryCode,
            CreatedDateTime = product.CreatedDateTime,
            ModifiedDateTime = product.ModifiedDateTime
        };
    }

    public async Task<ProductResponseDto?> PatchProduct(int id, ProductPatchRequestDto request)
    {
        var product = await _db.TblProducts
            .FirstOrDefaultAsync(x => x.DeleteFlag == false && x.ProductId == id);

        if (product is null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(request.ProductCode))
            product.ProductCode = request.ProductCode;
        if (!string.IsNullOrEmpty(request.ProductName))
            product.ProductName = request.ProductName;
        if (request.Price is not null && request.Price > 0)
            product.Price = request.Price ?? 0;
        if (request.Quantity is not null && request.Quantity > 0)
            product.Quantity = request.Quantity ?? 0;
        if (!string.IsNullOrEmpty(request.ProductCategoryCode))
            product.ProductCategoryCode = request.ProductCategoryCode;
        
        product.ModifiedDateTime = DateTime.Now;

        await _db.SaveChangesAsync();
        return new ProductResponseDto()
        {
            ProductId = product.ProductId,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            Price = product.Price,
            Quantity = product.Quantity,
            ProductCategoryCode = product.ProductCategoryCode,
            CreatedDateTime = product.CreatedDateTime,
            ModifiedDateTime = product.ModifiedDateTime
        };
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var product = await _db.TblProducts
            .FirstOrDefaultAsync(x => x.DeleteFlag == false && x.ProductId == id);

        if (product is null)
        {
            return false;
        }            
        
        product.DeleteFlag = true;
        product.ModifiedDateTime = DateTime.Now;

        await _db.SaveChangesAsync();
        return true;
    }
}