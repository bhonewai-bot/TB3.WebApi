namespace TB3.Models.Dtos.Staff;

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
}

public class StaffCreateRequest
{
    public string StaffName { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string Password { get; set; }
}