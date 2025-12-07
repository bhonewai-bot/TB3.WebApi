namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{pageNo}/{pageSize}")]
        public async Task<IActionResult> GetCustomers(int pageNo = 1, int pageSize = 10)
        {
            var result = await _customerService.GetCustomers(pageNo, pageSize);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }

        [HttpGet("{customerCode}")]
        public async Task<IActionResult> GetCustomer(string customerCode)
        {
            var result = await _customerService.GetCustomer(customerCode);

            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(CustomerCreateRequest request)
        {
            var result = await _customerService.CreateCustomer(request);
            
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }
    }
}
