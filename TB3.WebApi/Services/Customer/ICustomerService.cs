namespace TB3.WebApi.Services.Customer;

public interface ICustomerService
{
    Task<List<CustomerResponseDto>> GetCustomers(string customerName);
    Task<CustomerResponseDto?> GetCustomer(int id);
    Task<CustomerResponseDto?> CreateCustomer(CustomerCreateRequest request);
}