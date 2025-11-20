using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult GetProducts([FromQuery] string? productCategoryCode)
        {
            var products = _db.TblProducts
                .Where(x => x.DeleteFlag == false);

            if (!string.IsNullOrEmpty(productCategoryCode))
            {
                products = products.Where(x => x.ProductCategoryCode == productCategoryCode);
            }
            
            List<ProductResponseDto> lts = products
                .AsNoTracking()
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
                .ToList();
            
            return Ok(lts);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            var product = _db.TblProducts
                .Where(x => x.DeleteFlag == false)
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
                .FirstOrDefault(x => x.ProductId == id);

            if (product is null)
            {
                return NotFound("Product not found");
            }
            
            return Ok(product);
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductCreateRequestDto request)
        {
            var product = _db.TblProducts
                .FirstOrDefault(x => x.ProductCode == request.ProductCode);
            if (product is not null)
            {
                return BadRequest("Product code already exists");
            }
            
            _db.TblProducts.Add(new TblProduct()
            {
                ProductCode = request.ProductCode,
                ProductName = request.ProductName,
                Price = request.Price,
                Quantity = request.Quantity,
                ProductCategoryCode = request.ProductCategoryCode,
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

            product.ProductCode = request.ProductCode;
            product.ProductName = request.ProductName;
            product.Price = request.Price;
            product.Quantity = request.Quantity;
            product.ProductCategoryCode = request.ProductCategoryCode;
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
        
        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        
        public decimal Price { get; set; }

        public int Quantity { get; set; }
        
        public string ProductCategoryCode { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public DateTime? ModifiedDateTime { get; set; }
    }

    public class ProductCreateRequestDto
    {
        public string ProductCode { get; set; }
        
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        
        public string ProductCategoryCode { get; set; }
    }
    
    public class ProductUpdateRequestDto
    {
        public string ProductCode { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        
        public string ProductCategoryCode { get; set; }
    }
    
    public class ProductPatchRequestDto
    {
        public string? ProductCode { get; set; }
        
        public string? ProductName { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
        
        public string? ProductCategoryCode { get; set; }
    }
}