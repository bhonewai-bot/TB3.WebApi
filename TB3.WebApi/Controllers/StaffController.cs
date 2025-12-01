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
        public async Task<IActionResult> GetStaffs([FromQuery] string? staffName)
        {
            var staffs = await _staffService.GetStaffs(staffName);
            return Ok(staffs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaff(int id)
        {
            var staff = await _staffService.GetStaff(id);

            if (staff is null)
            {
                return NotFound("Staff not found");
            }

            return Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff(StaffCreateRequest request)
        {
            var response = await _staffService.CreateStaff(request);

            if (response is null)
            {
                return BadRequest("Failed to create staff");
            }

            return Ok(response);
        }
    }
}
