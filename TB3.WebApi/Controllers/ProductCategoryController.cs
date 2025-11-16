using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TB3.Database.AppDbContextModels;

namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProductCategoryController()
        {
            _db = new AppDbContext();
        }

        [HttpGet]
        public IActionResult GetProductCategories()
        {
            List<ProductCategoryResponseDto> lts = _db.TblProductCategories
                .OrderByDescending(x => x.ProductCategoryId)
                .Select(x => new ProductCategoryResponseDto()
                {
                    ProductCategoryId = x.ProductCategoryId,
                    ProductCategoryCode = x.ProductCategoryCode,
                    ProductCategoryName = x.ProductCategoryName
                })
                .ToList();

            return Ok(lts);
        }

        [HttpPost]
        public IActionResult CreateProductCategory(ProductCategoryCreateRequestDto request)
        {
            _db.TblProductCategories.Add(new TblProductCategory()
            {
                ProductCategoryCode = request.ProductCategoryCode,
                ProductCategoryName = request.ProductCategoryName
            });
            int result = _db.SaveChanges();
            string message = result > 0 ? "Saving successful" : "Saving failed";
            
            return Ok(message);
        }
    }

    public class ProductCategoryResponseDto
    {
        public int ProductCategoryId { get; set; }

        public string ProductCategoryCode { get; set; }

        public string ProductCategoryName { get; set; }
    }

    public class ProductCategoryCreateRequestDto
    {
        public string ProductCategoryCode { get; set; }

        public string ProductCategoryName { get; set; }
    }
}
