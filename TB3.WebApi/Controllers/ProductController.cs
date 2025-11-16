using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TB3.Database.AppDbContextModels;

namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProductController()
        {
            _db = new AppDbContext();
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            List<ProductResponseDto> lts = _db.TblProducts
                .Where(x => x.DeleteFlag == false)
                .OrderByDescending(x => x.ProductId)
                .Select(x => new ProductResponseDto()
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    DeleteFlag = x.DeleteFlag,
                    ProductCategoryId = x.ProductCategoryId,
                    CreatedDateTime = x.CreatedDateTime,
                    ModifiedDateTime = x.ModifiedDateTime
                })
                .ToList();
            
            return Ok(lts);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _db.TblProducts
                .Where(x => x.DeleteFlag == false)
                .FirstOrDefault(x => x.ProductId == id);

            if (product is null)
            {
                return NotFound("Product not found");
            }

            var response = new ProductResponseDto()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                Quantity = product.Quantity,
                DeleteFlag = product.DeleteFlag,
                ProductCategoryId = product.ProductCategoryId,
                CreatedDateTime = product.CreatedDateTime,
                ModifiedDateTime = product.ModifiedDateTime
            };
            
            return Ok(response);
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductCreateRequestDto request)
        {
            _db.TblProducts.Add(new TblProduct()
            {
                ProductName = request.ProductName,
                Price = request.Price,
                Quantity = request.Quantity,
                ProductCategoryId = request.ProductCategoryId,
                CreatedDateTime = DateTime.Now,
                DeleteFlag = false
            });
            int result = _db.SaveChanges();
            string message = result > 0 ? "Saving successful" : "Saving failed";
            
            return Ok(message);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductUpdateRequestDto request)
        {
            var product = _db.TblProducts
                .Where(x => x.DeleteFlag == false)
                .FirstOrDefault(x => x.ProductId == id);
            
            if (product is null)
            {
                return NotFound("Product not found");
            }

            product.ProductName = request.ProductName;
            product.Price = request.Price;
            product.Quantity = request.Quantity;
            product.ProductCategoryId = request.ProductCategoryId;
            product.ModifiedDateTime = DateTime.Now;
            
            int result = _db.SaveChanges();
            string message = result > 0 ? "Updating successful" : "Updating failed";
            
            return Ok(message);
        }

        [HttpPatch("{id}")]
        public IActionResult PatchProduct(int id, ProductPatchRequestDto request)
        {
            var product = _db.TblProducts
                .Where(x => x.DeleteFlag == false)
                .FirstOrDefault(x => x.ProductId == id);
            
            if (product is null)
            {
                return NotFound("Product not found");
            }

            if (!string.IsNullOrEmpty(request.ProductName))
                product.ProductName = request.ProductName;
            if (request.Price is not null && request.Price > 0)
                product.Price = request.Price ?? 0;
            if (request.Quantity is not null && request.Quantity > 0)
                product.Quantity = request.Quantity ?? 0;
            if (request.ProductCategoryId is not null && request.ProductCategoryId > 0)
                product.ProductCategoryId = request.ProductCategoryId ?? 0;
            product.ModifiedDateTime = DateTime.Now;
            
            int result = _db.SaveChanges();
            string message = result > 0 ? "Patching successful" : "Patching failed";
            
            return Ok(message);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _db.TblProducts
                .Where(x => x.DeleteFlag == false)
                .FirstOrDefault(x => x.ProductId == id);
            
            if (product is null)
            {
                return NotFound("Product not found");
            }
            
            product.DeleteFlag = true;
            int result = _db.SaveChanges();
            string message = result > 0 ? "Deleting successful" : "Deleting failed";
            
            return Ok(message);
        }
    }

    public class ProductResponseDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public bool DeleteFlag { get; set; }
        
        public int ProductCategoryId { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }
    }

    public class ProductCreateRequestDto
    {
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        
        public int ProductCategoryId { get; set; }
    }
    
    public class ProductUpdateRequestDto
    {
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        
        public int ProductCategoryId { get; set; }
    }
    
    public class ProductPatchRequestDto
    {
        public string? ProductName { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
        
        public int? ProductCategoryId { get; set; }
    }
}