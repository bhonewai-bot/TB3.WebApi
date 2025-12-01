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

        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] string? customerName)
        {
            var products = await _customerService.GetCustomers(customerName);
            
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomer(id);

            if (customer is null)
            {
                return NotFound("Customer not found");
            }

            return Ok(customer);
        }

        [HttpPost]
        public IActionResult CreateCustomer(CustomerCreateRequest request)
        {
            var response = _customerService.CreateCustomer(request);
            return Ok(response);
        }
    }
}
