namespace TB3.WebApi.Services.Staff;

public interface IStaffService
{
    Task<List<StaffResponseDto>> GetStaffs(string? staffName);
    Task<StaffResponseDto?> GetStaff(int id);
    Task<StaffResponseDto?> CreateStaff(StaffCreateRequest request);
}