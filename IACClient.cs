using AcumulusClient.entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcumulusClient
{
    public interface IACClient : IDisposable
    {
        Task<IList<Contact>> GetContactListAsync();
        Task<IList<Product>> GetProductsAsync();
        Task<string> PostAsync(AcumulusBaseObject data, bool removeentryelement = false);
        Task<IList<ACInvoice>> GetInvoicesAsync(string contactid);
        Task<Entry> GetInvoiceDetailAsync(string entryid);
        Task<IList<Entry>> GetOpenInvoicesAsync();
        Task<CreateInvoiceResponse> CreateInvoiceAsync(Customer acinvoice);
        Task SetPaidStatusAsync(string token,DateTime? invoicedate);
        Task<IList<ACAccount>> GetBankAccountsAsync();
        Task<IList<ACInvoiceTermplate>> GetInvoiceTemplatesAsync();
        Task<bool> EmailInvoiceAsync(EmailInvoice obj);
        Task<bool> RemindInvoiceAsync(EmailInvoice obj);
    }
}
