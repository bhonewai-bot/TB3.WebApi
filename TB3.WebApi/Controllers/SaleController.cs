using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TB3.Database.AppDbContextModels;

namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SaleController()
        {
            _db = new AppDbContext();
        }

        [HttpGet]
        public IActionResult GetSales()
        {
            List<SaleResponseDto> lts = _db.TblSales
                .Select(x => new SaleResponseDto()
                {
                    SaleId = x.SaleId,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    CreatedDateTime = x.CreatedDateTime
                })
                .ToList();

            return Ok(lts);
        }

        [HttpPost]
        public IActionResult CreateSale(SaleCreateRequestDto request)
        {
            var product = _db.TblProducts
                .FirstOrDefault(x => x.ProductId == request.ProductId && x.DeleteFlag == false);

            if (product is null)
            {
                return BadRequest("Product not found");
            }

            if (product.Quantity < request.Quantity)
            {
                return BadRequest("Insufficient quantity");
            }
            
            _db.TblSales.Add(new TblSale()
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = request.Price,
                CreatedDateTime = DateTime.Now
            });

            product.Quantity -= request.Quantity;
            product.ModifiedDateTime = DateTime.Now;
            
            int result = _db.SaveChanges();
            string message = result > 0 ? "Saving successful" : "Saving failed";
            
            return Ok(message);
        }
    }

    public class SaleResponseDto
    {
        public int SaleId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }

    public class SaleCreateRequestDto
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
