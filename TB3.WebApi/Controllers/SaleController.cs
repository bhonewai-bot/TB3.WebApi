using TB3.Database;

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
        public IActionResult GetSales(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? cashierName,
            [FromQuery] string? paymentType)
        {
            var sales = _db.TblSales.AsQueryable();

            if (from.HasValue)
            {
                sales = sales.Where(x => x.CreatedDateTime >= from.Value);
            }

            if (to.HasValue)
            {
                var endDate = to.Value.AddDays(1);
                sales = sales.Where(x => x.CreatedDateTime < endDate);
            }

            if (!string.IsNullOrEmpty(cashierName))
            {
                sales = sales.Where(x => x.CashierName.Contains(cashierName));
            }

            if (!string.IsNullOrEmpty(paymentType))
            {
                sales = sales.Where(x => x.PaymentType == paymentType);
            }
            
            List<SaleResponseDto> lts = sales
                .OrderByDescending(x => x.SaleId)
                .Select(x => new SaleResponseDto()
                {
                    SaleId = x.SaleId,
                    ProductId = x.ProductId,
                    CashierName = x.CashierName,
                    PaymentType = x.PaymentType,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    TotalAmount = x.TotalAmount,
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

            if (request.PaymentType != EnumPaymentType.Card && request.PaymentType != EnumPaymentType.Cash)
            {
                return BadRequest("Invalid payment method");
            }
            
            _db.TblSales.Add(new TblSale()
            {
                ProductId = request.ProductId,
                CashierName = request.CashierName,
                PaymentType = request.PaymentType.ToString(),
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
        
        public string CashierName { get; set; }
        
        public string PaymentType { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal? TotalAmount { get; set; }
        
        public DateTime CreatedDateTime { get; set; }
    }

    public class SaleCreateRequestDto
    {
        public int ProductId { get; set; }
        
        public string CashierName { get; set; }
        
        public EnumPaymentType PaymentType { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
