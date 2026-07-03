using System.Collections.Generic;
using System.Threading.Tasks;
using ERP_sys.Models;
namespace ERP_sys.Repositories
{
	public interface ISalesLedgerRepository
	{

		Task<int> CreateInvoiceAsync(SalesLedger ledger);
		Task<SalesLedger> GetInvoiceByIdAsync(int id);
		Task<List<SalesLedger>> GetAllInvoicesAsync();
		Task<int> UpdateInvoiceAsync(SalesLedger ledger);
		Task<int> DeleteInvoiceAsync(int id);



	}
}