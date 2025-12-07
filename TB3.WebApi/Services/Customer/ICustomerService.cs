using TB3.Models;

namespace TB3.WebApi.Services.Customer;

public interface ICustomerService
{
    Task<Result<List<CustomerResponseDto>>> GetCustomers(int pageNo, int pageSize);
    Task<Result<CustomerResponseDto>> GetCustomer(string customerCode);
    Task<Result<CustomerResponseDto>> CreateCustomer(CustomerCreateRequest request);
}