namespace TB3.WebApi.Services.Staff;

public interface IStaffService
{
    Task<Result<List<StaffResponseDto>>> GetStaffs(int pageNo, int pageSize);
    Task<Result<StaffResponseDto>> GetStaff(string staffCode);
    Task<Result<StaffResponseDto>> CreateStaff(StaffCreateRequest request);
}