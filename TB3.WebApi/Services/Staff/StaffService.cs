namespace TB3.WebApi.Services.Staff;

public class StaffService : IStaffService
{
    private readonly AppDbContext _db;
    private readonly ISequenceService _sequenceService;
    
    public StaffService(ISequenceService sequenceService, AppDbContext db)
    {
        _sequenceService = sequenceService;
        _db = db;
    }
    
    public async Task<List<StaffResponseDto>> GetStaffs(string? staffName)
    {
        var query = _db.TblStaffs.AsQueryable();

        if (!string.IsNullOrEmpty(staffName))
        {
            query = query.Where(x => x.StaffName.Contains(staffName));
        }

        var staffs = await query
            .AsNoTracking()
            .OrderByDescending(x => x.StaffId)
            .Select(x => new StaffResponseDto
            {
                StaffId = x.StaffId,
                StaffCode = x.StaffCode,
                StaffName = x.StaffName,
                DateOfBirth = x.DateOfBirth,
                MobileNo = x.MobileNo,
                Address = x.Address,
                Gender = x.Gender,
                Position = x.Position
            })
            .ToListAsync();

        return staffs;
    }

    public async Task<StaffResponseDto?> GetStaff(int id)
    {
        var staff = await _db.TblStaffs
            .AsNoTracking()
            .Where(x => x.StaffId == id)
            .Select(x => new StaffResponseDto
            {
                StaffId = x.StaffId,
                StaffCode = x.StaffCode,
                StaffName = x.StaffName,
                DateOfBirth = x.DateOfBirth,
                MobileNo = x.MobileNo,
                Address = x.Address,
                Gender = x.Gender,
                Position = x.Position
            })
            .FirstOrDefaultAsync();

        return staff;
    }

    public async Task<StaffResponseDto?> CreateStaff(StaffCreateRequest request)
    {
        string staffCode = await _sequenceService.GenerateCode("StaffCode");

        var staff = new TblStaff
        {
            StaffCode = staffCode,
            StaffName = request.StaffName,
            DateOfBirth = request.DateOfBirth,
            MobileNo = request.MobileNo,
            Address = request.Address,
            Gender = request.Gender,
            Position = request.Position,
            Password = request.Password
        };

        _db.TblStaffs.Add(staff);
        await _db.SaveChangesAsync();

        return new StaffResponseDto
        {
            StaffId = staff.StaffId,
            StaffCode = staff.StaffCode,
            StaffName = staff.StaffName,
            DateOfBirth = staff.DateOfBirth,
            MobileNo = staff.MobileNo,
            Address = staff.Address,
            Gender = staff.Gender,
            Position = staff.Position
        };
    }
}