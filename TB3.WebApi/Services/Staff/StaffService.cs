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
    
    public async Task<Result<List<StaffResponseDto>>> GetStaffs(int pageNo, int pageSize)
    {
        try
        {
            if (pageNo <= 0)
                return Result<List<StaffResponseDto>>.ValidationError("Page No must be greater than zero");
            
            if (pageSize <= 0)
                return Result<List<StaffResponseDto>>.ValidationError("Page Size must be greater than zero");

            var staffs = await _db.TblStaffs
                .AsNoTracking()
                .OrderByDescending(x => x.StaffId)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
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

            return Result<List<StaffResponseDto>>.Success(staffs);
        }
        catch (Exception ex)
        {
            return Result<List<StaffResponseDto>>.SystemError(ex.Message);
        }
    }

    public async Task<Result<StaffResponseDto>> GetStaff(string staffCode)
    {
        try
        {
            var staff = await _db.TblStaffs
                .AsNoTracking()
                .Where(x => x.StaffCode == staffCode)
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
            
            if (staff is null)
                return Result<StaffResponseDto>.ValidationError("Staff not found");

            return Result<StaffResponseDto>.Success(staff);
        }
        catch (Exception ex)
        {
            return Result<StaffResponseDto>.SystemError(ex.Message);
        }
    }

    public async Task<Result<StaffResponseDto>> CreateStaff(StaffCreateRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.StaffName))
                return Result<StaffResponseDto>.ValidationError("Staff name is required");
            
            if (string.IsNullOrWhiteSpace(request.MobileNo))
                return Result<StaffResponseDto>.ValidationError("Mobile No is required");
            
            if (string.IsNullOrWhiteSpace(request.Address))
                return Result<StaffResponseDto>.ValidationError("Address is required");
            
            if (string.IsNullOrWhiteSpace(request.Gender))
                return Result<StaffResponseDto>.ValidationError("Gender is required");

            if (string.IsNullOrWhiteSpace(request.Position))
                return Result<StaffResponseDto>.ValidationError("Position is required");

            var exists = await _db.TblStaffs
                .AnyAsync(x => x.MobileNo == request.MobileNo);
            
            if (exists)
                return Result<StaffResponseDto>.ValidationError("Mobile No is already registered");
            
            if (request.DateOfBirth == default)
                return Result<StaffResponseDto>.ValidationError("Date of birth is required");
            
            var age = DevCode.CalculateAge(request.DateOfBirth);
            
            if (age < 18)
                return Result<StaffResponseDto>.ValidationError("Age must be greater than 18");
            
            string staffCode = await _sequenceService.GenerateCode("StaffCode");

            var staff = new TblStaff()
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

            var staffDto = new StaffResponseDto
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
            
            return Result<StaffResponseDto>.Success(staffDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}