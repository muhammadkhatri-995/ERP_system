using ERP_sys.Attributes;
using ERP_sys.Models;
using ERP_sys.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace ERP_sys.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // 1. Create a new customer

        [HttpPost]
        [AuditAction("customer created")]
        public async Task<IActionResult> Create(Customers customer)
        {
            var id = await _customerRepository.CreateAsync(customer);

            return Ok(new
            {
                Message = "Customer created successfully.",
                CustomerId = id
            });
        }
        // 2. Get a customer by ID
        // Get Customer By Id
        // GET: api/customer/5
        // ==========================
        [HttpGet("{id}")]
        [AuditAction("view customer by id")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return NotFound(new { Message = "Customer not found." });

            return Ok(customer);
        }


        // 3. Get all customers
        [HttpGet]
        [AuditAction("get all customers")]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerRepository.GetAllAsync();

            return Ok(customers);
        }
        [HttpPut("{id}")]
        [AuditAction("update the customer")]
        public async Task<IActionResult> Update(int id, Customers customer)
        {
            var rowsAffected = await _customerRepository.UpdateAsync(customer);

            if (rowsAffected == 0)
                return NotFound(new { Message = "Customer not found." });

            return Ok(new
            {
                Message = "Customer updated successfully."
            });
        }

        // Delete Customer
        // DELETE: api/customer/5
        // ==========================
        [HttpDelete("{id}")]
        [AuditAction("Deletes the customer")]
        public async Task<IActionResult> Delete(int id)
        {
            var rowsAffected = await _customerRepository.DeleteAsync(id);

            if (rowsAffected == 0)
                return NotFound(new { Message = "Customer not found." });

            return Ok(new
            {
                Message = "Customer deleted successfully."
            });
        }
    }
}
   