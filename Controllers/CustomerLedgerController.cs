using ERP_sys.Attributes;
using ERP_sys.Models.DTOs;
using ERP_sys.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERP_sys.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class CustomerLedgerController : ControllerBase
    {
        private readonly ICustomerLedgerRepository _customerLedgerRepository;
        private readonly ISalesLedgerRepository _salesLedgerRepository;

        public CustomerLedgerController(
            ICustomerLedgerRepository customerLedgerRepository,
            ISalesLedgerRepository salesLedgerRepository)
        {
            _customerLedgerRepository = customerLedgerRepository;
            _salesLedgerRepository = salesLedgerRepository;
        }

        [HttpGet]
        [AuditAction("Gets the customer ledger")]
        public async Task<IActionResult> GetLedger(
            [FromQuery] int customerId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? paymentStatus,
            [FromQuery] string? invoiceNo)
        {
            if (customerId <= 0)
                return BadRequest(new { message = "A valid customerId is required" });

            var filter = new CustomerLedgerFilterDto
            {
                CustomerId = customerId,
                FromDate = fromDate,
                ToDate = toDate,
                PaymentStatus = paymentStatus,
                InvoiceNo = invoiceNo
            };

            try
            {
                var result = await _customerLedgerRepository.GetCustomerLedgerAsync(filter);
                if (result == null)
                    return NotFound(new { message = $"Customer with id {customerId} not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load customer ledger", detail = ex.Message });
            }
     
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetLedgerByCustomerId(int customerId)
        {
            if (customerId <= 0)
                return BadRequest(new { message = "A valid customerId is required" });

            var filter = new CustomerLedgerFilterDto { CustomerId = customerId };

            try
            {
                var result = await _customerLedgerRepository.GetCustomerLedgerAsync(filter);
                if (result == null)
                    return NotFound(new { message = $"Customer with id {customerId} not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load customer ledger", detail = ex.Message });
            }
        }

        [HttpGet("ViewInvoice/{invoiceId}")]
        public async Task<IActionResult> ViewInvoice(int invoiceId)
        {
            try
            {
                var invoice = await _salesLedgerRepository.GetInvoiceByIdAsync(invoiceId);
                if (invoice == null)
                    return NotFound(new { message = $"Invoice with id {invoiceId} not found" });

                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to load invoice", detail = ex.Message });
            }
        }
    }
}