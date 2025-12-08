namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffs(int pageNo = 1, int pageSize = 10)
        {
            var result = await _staffService.GetStaffs(pageNo, pageSize);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }

        [HttpGet("{staffCode}")]
        public async Task<IActionResult> GetStaff(string staffCode)
        {
            var result = await _staffService.GetStaff(staffCode);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff(StaffCreateRequest request)
        {
            var result = await _staffService.CreateStaff(request);
            if (result.IsValidatorError)
                return BadRequest(result.Message);
            
            if (result.IsSystemError)
                return StatusCode(500, result.Message);
            
            return Ok(result.Data);
        }
    }
}
