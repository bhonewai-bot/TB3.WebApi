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

    public async Task<List<CustomerResponseDto>> GetCustomers(string customerName)
    {
        var query = _db.TblCustomers.AsQueryable();

        if (!string.IsNullOrEmpty(customerName))
        {
            query = query.Where(x => x.CustomerName.Contains(customerName));
        }
            
        var customers = await query
            .AsNoTracking()
            .OrderByDescending(x => x.CustomerId)
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

        return customers;
    }

    public async Task<CustomerResponseDto?> GetCustomer(int id)
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
            .FirstOrDefaultAsync(x => x.CustomerId == id);
        
        return customer;
    }

    public async Task<CustomerResponseDto?> CreateCustomer(CustomerCreateRequest request)
    {
        string customerCode = _sequenceService.GenerateCode("CustomerCode");

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

        return new CustomerResponseDto()
        {
            CustomerId = customer.CustomerId,
            CustomerCode = customer.CustomerCode,
            CustomerName = customer.CustomerName,
            MobileNo = customer.MobileNo,
            DateOfBirth = customer.DateOfBirth,
            Gender = customer.Gender
        };
    }
}