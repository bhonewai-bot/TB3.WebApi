using System.Runtime.InteropServices.JavaScript;

namespace TB3.WebApi.Services.Customer;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;
    private readonly ISequenceService _sequenceService;

    public CustomerService(ISequenceService sequenceService, AppDbContext db)
    {
        _sequenceService = sequenceService;
        _db = db;
    }

    public async Task<Result<List<CustomerResponseDto>>> GetCustomers(int pageNo, int pageSize)
    {
        try
        {
            if (pageNo <= 0)
                return Result<List<CustomerResponseDto>>.ValidationError("Page size must be greater than zero");
            
            if (pageSize <= 0)
                return Result<List<CustomerResponseDto>>.ValidationError("Page number must be greater than zero");
            
            var customers = await _db.TblCustomers
                .AsNoTracking()
                .OrderByDescending(x => x.CustomerId)
                .Skip((pageNo -1) * pageSize)
                .Take(pageSize)
                .Select(x => new CustomerResponseDto()
                {
                    CustomerId = x.CustomerId,
                    CustomerCode = x.CustomerCode,
                    CustomerName = x.CustomerName,
                    MobileNo = x.MobileNo,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender
                })
                .ToListAsync();

            return Result<List<CustomerResponseDto>>.Success(customers);
        }
        catch (Exception ex)
        {
            return Result<List<CustomerResponseDto>>.SystemError(ex.Message);
        }
    }

    public async Task<Result<CustomerResponseDto>> GetCustomer(string customerCode)
    {
        try
        {
            var customer = await _db.TblCustomers
                .AsNoTracking()
                .Select(x => new CustomerResponseDto()
                {
                    CustomerId = x.CustomerId,
                    CustomerCode = x.CustomerCode,
                    CustomerName = x.CustomerName,
                    MobileNo = x.MobileNo,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender
                })
                .FirstOrDefaultAsync(x => x.CustomerCode == customerCode);
            
            if (customer is null)
                return Result<CustomerResponseDto>.ValidationError("Customer not found");
        
            return Result<CustomerResponseDto>.Success(customer);
        }
        catch (Exception ex)
        {
            return Result<CustomerResponseDto>.SystemError(ex.Message);
        }
    }

    public async Task<Result<CustomerResponseDto>> CreateCustomer(CustomerCreateRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CustomerName))
                return Result<CustomerResponseDto>.ValidationError("Customer name is required");
            
            if (string.IsNullOrWhiteSpace(request.MobileNo))
                return Result<CustomerResponseDto>.ValidationError("MobileNo is required");

            var exists = await _db.TblCustomers
                .AnyAsync(x => x.MobileNo == request.MobileNo);
            
            if (exists)
                return Result<CustomerResponseDto>.ValidationError("MobileNo is already exists");
            
            if (request.DateOfBirth == default)
                return Result<CustomerResponseDto>.ValidationError("Date of birth is required");
            
            var age = CalculateAge(request.DateOfBirth);
            
            if (age < 18)
                return Result<CustomerResponseDto>.ValidationError("Age must be greater than 18");
            
            string customerCode = await _sequenceService.GenerateCode("CustomerCode");

            var customer = new TblCustomer()
            {
                CustomerCode = customerCode,
                CustomerName = request.CustomerName,
                MobileNo = request.MobileNo,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender
            };
        
            _db.TblCustomers.Add(customer);
            await _db.SaveChangesAsync();

            var customerDto = new CustomerResponseDto()
            {
                CustomerId = customer.CustomerId,
                CustomerCode = customerCode,
                CustomerName = customer.CustomerName,
                MobileNo = customer.MobileNo,
                DateOfBirth = customer.DateOfBirth,
                Gender = customer.Gender
            };
            
            return Result<CustomerResponseDto>.Success(customerDto);
        }
        catch (Exception ex)
        {
            return Result<CustomerResponseDto>.SystemError(ex.Message);
        }
    }

    private int CalculateAge(DateTime birthDate)
    {
        DateTime now = DateTime.Today;
        TimeSpan ageDiff = now - birthDate;
        int age = (int)(ageDiff.TotalDays / 365);
        return age;
    }
}