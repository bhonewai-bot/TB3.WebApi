using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TB3.Database.AppDbContextModels;

namespace TB3.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CustomerController()
        {
            _db = new AppDbContext();
        }

        [HttpGet]
        public IActionResult GetCustomers([FromQuery] string? customerName)
        {
            var customers = _db.TblCustomers.AsQueryable();

            if (!string.IsNullOrEmpty(customerName))
            {
                customers = customers.Where(x => x.CustomerName.Contains(customerName));
            }
            
            List<CustomerResponseDto> lts = customers
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
                .ToList();
            
            return Ok(lts);
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomer(int id)
        {
            var customer = _db.TblCustomers
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
                .FirstOrDefault(x => x.CustomerId == id);

            if (customer is null)
            {
                return NotFound("Customer not found");
            }

            return Ok(customer);
        }

        [HttpPost]
        public IActionResult CreateCustomer(CustomerCreateRequest request)
        {
            var nextNumber = (_db.TblCustomers.Any()) 
                ? _db.TblCustomers.Max(x => x.CustomerId) + 1 
                : 1;  
            
            string customerCode = $"CUS{nextNumber:D4}";

            _db.TblCustomers.Add(new TblCustomer()
            {
                CustomerCode = customerCode,
                CustomerName = request.CustomerName,
                MobileNo = request.MobileNo,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender
            });

            int result = _db.SaveChanges();
            string message = result > 0 ? "Saving successful" : "Saving failed";

            return Ok(message);
        }
    }

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
}
