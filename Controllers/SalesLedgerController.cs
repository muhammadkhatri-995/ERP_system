using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERP_sys.Models;
using ERP_sys.Repositories;

namespace ERP_sys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesLedgerController : ControllerBase
    {
        private readonly ISalesLedgerRepository _salesLedgerRepository;

        public SalesLedgerController(ISalesLedgerRepository salesLedgerRepository)
        {
            _salesLedgerRepository = salesLedgerRepository;
        }

        // GET: api/SalesLedger
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _salesLedgerRepository.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        // GET: api/SalesLedger/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _salesLedgerRepository.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound(new { message = $"No invoice found with Id {id}" });

            return Ok(invoice);
        }

        // POST: api/SalesLedger
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalesLedger ledger)
        {
            if (string.IsNullOrWhiteSpace(ledger.InvoiceNo))
                return BadRequest(new { message = "InvoiceNo is required" });

            if (ledger.CustomerId <= 0)
                return BadRequest(new { message = "A valid CustomerId is required" });

            if (ledger.SalesLedgerItem == null || ledger.SalesLedgerItem.Count == 0)
                return BadRequest(new { message = "An invoice needs at least one line item" });

            ledger.CreatedBy = 1; // swap for logged-in user once auth is wired up

            var newId = await _salesLedgerRepository.CreateInvoiceAsync(ledger);
            ledger.Id = newId;

            return CreatedAtAction(nameof(GetById), new { id = newId }, ledger);
        }

        // PUT: api/SalesLedger/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SalesLedger ledger)
        {
            if (id != ledger.Id)
                return BadRequest(new { message = "Id in the URL does not match Id in the request body" });

            var updatedId = await _salesLedgerRepository.UpdateInvoiceAsync(ledger);
            return Ok(new { message = "Invoice updated successfully.", id = updatedId });
        }

        // DELETE: api/SalesLedger/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rowsAffected = await _salesLedgerRepository.DeleteInvoiceAsync(id);
            if (rowsAffected == 0)
                return NotFound(new { message = $"No invoice found with Id {id}" });

            return NoContent();
        }
    }
}