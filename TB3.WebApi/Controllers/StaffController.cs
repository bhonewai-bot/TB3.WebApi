using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TB3.Database.AppDbContextModels;

namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StaffController()
        {
            _db = new AppDbContext();
        }

        [HttpGet]
        public IActionResult GetStaffs([FromQuery] string? staffName)
        {
            var staffs = _db.TblStaffs.AsQueryable();

            if (!string.IsNullOrEmpty(staffName))
            {
                staffs = staffs.Where(x => x.StaffName.Contains(staffName));
            }

            List<StaffResponseDto> lts = staffs
                .AsNoTracking()
                .OrderByDescending(x => x.StaffId)
                .Select(x => new StaffResponseDto()
                {
                    StaffId = x.StaffId,
                    StaffCode = x.StaffCode,
                    StaffName = x.StaffName,
                    MobileNo = x.MobileNo,
                    Address = x.Address,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender,
                    Position = x.Position,
                    Password = x.Password
                })
                .ToList();
            
            return Ok(lts);
        }

        [HttpGet("{id}")]
        public IActionResult GetStaff(int id)
        {
            var staff = _db.TblStaffs
                .AsNoTracking()
                .Select(x => new StaffResponseDto()
                {
                    StaffId = x.StaffId,
                    StaffCode = x.StaffCode,
                    StaffName = x.StaffName,
                    MobileNo = x.MobileNo,
                    Address = x.Address,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender,
                    Position = x.Position,
                    Password = x.Password
                })
                .FirstOrDefault(x => x.StaffId == id);

            if (staff is null)
            {
                return NotFound("Staff not found");
            }
            
            return Ok(staff);
        }

        [HttpPost]
        public IActionResult CreateStaff(StaffCreateRequest request)
        {
            var nextNumber = (_db.TblStaffs.Any())
                ? _db.TblStaffs.Max(x => x.StaffId) + 1
                : 1;

            string staffCode = $"STF{nextNumber:D4}";

            _db.TblStaffs.Add(new TblStaff()
            {
                StaffCode = staffCode,
                StaffName = request.StaffName,
                MobileNo = request.MobileNo,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Position = request.Position,
                Password = request.Password
            });
            
            int result = _db.SaveChanges();
            string message = result > 0 ? "Saving successful" : "Saving failed";
            
            return Ok(message);
        }
    }

    public class StaffResponseDto
    {
        public int StaffId { get; set; }

        public string StaffCode { get; set; }

        public string StaffName { get; set; }

        public string MobileNo { get; set; }

        public string Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Position { get; set; }

        public string? Password { get; set; }
    }

    public class StaffCreateRequest
    {
        public string StaffName { get; set; } = null!;

        public string MobileNo { get; set; } = null!;

        public string Address { get; set; } = null!;

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; } = null!;

        public string Position { get; set; } = null!;

        public string? Password { get; set; }
    }
}
