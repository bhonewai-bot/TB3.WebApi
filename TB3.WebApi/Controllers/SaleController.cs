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
        
        [HttpGet("{pageNo}/{pageSize}")]
        public async Task<IActionResult> GetSales(int pageNo = 1, int pageSize = 10)
        {
            var result = await _saleService.GetSales(pageNo, pageSize);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            return Ok(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetSales(DateTime from, DateTime to)
        {
            var result = await _saleService.GetSales(from, to);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            return Ok(result.Data);
        }
        
        [HttpGet("{voucherNo}")]
        public async Task<IActionResult> GetSale(string voucherNo)
        {
            var result = await _saleService.GetSale(voucherNo);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale(SaleCreateRequestDto request)
        {
            var result = await _saleService.CreateSale(request);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            return Ok(result.Data);
        }
    }
}
