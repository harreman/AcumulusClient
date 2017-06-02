using AcumulusClient.entities;
using System;
using System.Collections.Generic;

namespace AcumulusClient
{
    public interface IACClient : IDisposable
    {
        IList<Contact> GetContactList();
        IList<Product> GetProducts();
        string Post(AcumulusBaseObject data, bool removeentryelement = false);
        IList<ACInvoice> GetInvoices(string contactid);
        Entry GetInvoiceDetail(string entryid);
        IList<Entry> GetOpenInvoices();
        CreateInvoiceResponse CreateInvoice(Customer acinvoice);
        void SetPaidStatus(string token,DateTime? invoicedate);
        IList<ACAccount> GetBankAccounts();
        IList<ACInvoiceTermplate> GetInvoiceTemplates();
        bool EmailInvoice(EmailInvoice obj);
        bool RemindInvoice(EmailInvoice obj);
    }
}
