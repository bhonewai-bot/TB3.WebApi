namespace TB3.Models.Dtos.Customer;

public class CustomerResponseDto
{
    public int CustomerId { get; set; }
        
    public string CustomerCode { get; set; }
        
    public string CustomerName { get; set; }
        
    public string? MobileNo { get; set; }
        
    public DateTime? DateOfBirth { get; set; }
        
    public string? Gender { get; set; }
}

public class CustomerCreateRequest
{ 
    public string CustomerName { get; set; }
        
    public string? MobileNo { get; set; }
        
    public DateTime? DateOfBirth { get; set; }
        
    public string? Gender { get; set; }
}