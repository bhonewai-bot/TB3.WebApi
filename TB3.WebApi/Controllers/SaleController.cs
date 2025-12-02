namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSales(DateTime? from, DateTime? to)
        {
            var sales = await _saleService.GetSales(from, to);
            return Ok(sales);
        }
        
        [HttpGet("{voucherNo}")]
        public async Task<IActionResult> GetSale(string voucherNo)
        {
            var sale = await _saleService.GetSale(voucherNo);

            if (sale is null)
            {
                return NotFound("Sale not found");
            }
            
            return Ok(sale);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale(SaleCreateRequestDto request)
        {
            var response = await _saleService.CreateSale(request);
            
            if (response is null)
            {
                return BadRequest("Product not found or not enough stock");
            }
            
            return Ok(response);
        }
    }
}
