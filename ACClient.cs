using AcumulusClient.entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace AcumulusClient
{
    public static class StringExtensions
    {
        /// <summary>
        /// takes a substring between two anchor strings (or the end of the string if that anchor is null)
        /// </summary>
        /// <param name="this">a string</param>
        /// <param name="from">an optional string to search after</param>
        /// <param name="until">an optional string to search before</param>
        /// <param name="comparison">an optional comparison for the search</param>
        /// <returns>a substring based on the search</returns>
        public static string Substring(this string @this, string from = null, string until = null, StringComparison comparison = StringComparison.InvariantCulture)
        {
            int fromLength = (from ?? string.Empty).Length;
            int startIndex = !string.IsNullOrEmpty(from)
                ? @this.IndexOf(from, comparison) + fromLength
                : 0;

            if (startIndex < fromLength) { throw new ArgumentException("from: Failed to find an instance of the first anchor"); }

            int endIndex = !string.IsNullOrEmpty(until)
            ? @this.IndexOf(until, startIndex, comparison)
            : @this.Length;

            if (endIndex < 0) { throw new ArgumentException("until: Failed to find an instance of the last anchor"); }

            string subString = @this.Substring(startIndex, endIndex - startIndex);
            return subString;
        }
    }

    public partial class ACClient : IACClient
    {
        private readonly Contract contract;
        private readonly Connector connector;
        private readonly HttpClient client;

        public ACClient(IOptions<Contract> _contract, IOptions<Connector> _connector, IHttpClientFactory httpClientFactory)
        {
            contract = _contract.Value;

            client = httpClientFactory.CreateClient("AcumulusHttpClient");
            connector = _connector.Value;
            client.BaseAddress = new Uri(contract.BaseUrl);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        public async Task<AcumulusBaseObject> GetAsync(dynamic data)
        {

            string xmlstring = data.ToXML().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            dynamic response = await client.GetAsync(data.Url + "?xmlstring=" + HttpUtility.HtmlEncode(xmlstring)).ConfigureAwait(false);
#if DEBUG
            dynamic text = response.Content.ReadAsStringAsync();
#endif
            return new AcumulusBaseObject(contract, connector);

        }

        public async Task<string> PostAsync(AcumulusBaseObject data, bool removeentryelement = false)
        {
            HttpResponseMessage result = await client.PostAsync(data.Url, GetHttpRequestMessage(data, removeentryelement)).ConfigureAwait(false);
            string text = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            return text;

        }

        public async Task<string> PutAsync<t>(int id, AcumulusBaseObject data)
        {

            FormUrlEncodedContent requestMessage = GetHttpRequestMessage(data);

            HttpResponseMessage result = await client.PutAsync(data.Url + id, requestMessage).ConfigureAwait(false);

            return await result.Content.ReadAsStringAsync().ConfigureAwait(false);

        }

        public async Task<string> DeleteAsync(int id, AcumulusBaseObject data)
        {

            HttpResponseMessage result = await client.DeleteAsync(data.Url + id).ConfigureAwait(false);

            return result.Content.ToString();

        }
        protected FormUrlEncodedContent GetHttpRequestMessage(AcumulusBaseObject data, bool removeentryelement = false)
        {
            string alreadyencoded = data.entryid;
            if (removeentryelement)
            {

                data.entryid = "";
            }
            string xmlstring = data.ToXML().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            xmlstring = xmlstring.Replace("<contract />", "");
            if (removeentryelement)
            {
                xmlstring = xmlstring.Replace("<entryid />", alreadyencoded);
            }

            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                     new KeyValuePair<string, string>("xmlstring", xmlstring)
                            });

            Dictionary<string, string> properties = new Dictionary<string, string> { { "xmlstring", xmlstring } };
            return content;
        }

        public async Task GetGeneralInfoAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector)
            {
                Url = "/acumulus/stable/general/my_acumulus.php",
                withcontract = true
            };
            await PostAsync(obj).ConfigureAwait(false);
        }
        public async Task<IList<Contact>> GetContactListAsync(string filter = "")
        {
            ContactList obj = new ContactList
            {
                Contract = contract,
                Connector = connector,

                Url = "/acumulus/stable/contacts/contacts_list.php",
                withcontract = true,
                contactstatus = 1,
                filter = filter
            };
            string t = await PostAsync(obj).ConfigureAwait(false);
            return AcumulusBaseObject.ListFromXML<Contact>(t);
        }


        public async Task<IList<Product>> GetProductsAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector)
            {
                Url = "/acumulus/stable/picklists/picklist_products.php",
                withcontract = true
            };
            string t = await PostAsync(obj).ConfigureAwait(false);
            return AcumulusBaseObject.ListFromXML<Product>(t);
        }

        public async Task<IList<Entry>> GetOpenInvoicesAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector)
            {
                Url = "/acumulus/stable/reports/report_unpaid_debtors.php",
                withcontract = true
            };
            string t = await PostAsync(obj).ConfigureAwait(false);
            return AcumulusBaseObject.ListFromXML<Entry>(t);
        }


        public async Task<IList<ACInvoice>> GetInvoicesAsync(string contactid)
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector)
            {
                Url = "/acumulus/stable/contacts/contact_invoices_outgoing.php",
                contactid = contactid.ToString(),
                withcontract = true
            };
            string t = await PostAsync(obj).ConfigureAwait(false);
            return AcumulusBaseObject.ListFromXML<ACInvoice>(t);
        }

        public async Task<Entry> GetInvoiceDetailAsync(string entryid)
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector)
            {
                Url = "/acumulus/stable/entry/entry_info.php",
                entryid = entryid,
                withcontract = true
            };
            string t = await PostAsync(obj).ConfigureAwait(false);
            return AcumulusBaseObject.FromXML<Entry>(t);
        }

        public async Task<CreateInvoiceResponse> CreateInvoiceAsync(Customer invoice)
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector)
            {
                withcontract = false,
                Url = "/acumulus/stable/invoices/invoice_add.php",
                entryid = invoice.ToXML().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("<format>xml</format>", "")
            };
            string t = await PostAsync(obj, true).ConfigureAwait(false);

            return new CreateInvoiceResponse()
            {
                Entryid = t.Substring(from: "<entryid>", until: "</entryid>"),
                Invoicenumber = t.Substring(from: "<invoicenumber>", until: "</invoicenumber>"),
                Token = t.Substring(from: "<token>", until: "</token>"),
                NonParsedMessage = t
            };
        }

        public async Task SetPaidStatusAsync(string token, DateTime? invoicedate)
        {

            DateTime invdate = invoicedate.HasValue ? (DateTime)invoicedate : DateTime.Now.Date;

            PaymentStatus obj = new PaymentStatus
            {
                Contract = contract,
                Connector = connector,
                withcontract = true,
                paymentdate = invdate.ToString("yyyy-MM-dd"),
                token = token,
                paymentstatus = "2",
                Url = "/acumulus/stable/invoices/invoice_paymentstatus_set.php"
            };

            await PostAsync(obj, true).ConfigureAwait(false);
        }

        public async Task<bool> EmailInvoiceAsync(EmailInvoice obj)
        {
            obj.Contract = contract;
            obj.Connector = connector;
            obj.withcontract = true;
            obj.invoicetype = "0";
            obj.Url = "/acumulus/stable/invoices/invoice_mail.php";
            await PostAsync(obj, true).ConfigureAwait(false);

            return true;

        }
        public async Task<bool> RemindInvoiceAsync(EmailInvoice obj)
        {
            obj.Contract = contract;
            obj.Connector = connector;
            obj.withcontract = true;
            obj.invoicetype = "1";
            obj.Url = "/acumulus/stable/invoices/invoice_mail.php";

            await PostAsync(obj, true).ConfigureAwait(false);
            return true;
        }

        public async Task<IList<ACAccount>> GetBankAccountsAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector)
            {
                Url = "acumulus/stable/picklists/picklist_accounts.php",
                withcontract = true
            };
            string t = await PostAsync(obj).ConfigureAwait(false);
            return AcumulusBaseObject.ListFromXML<ACAccount>(t);
        }
        public async Task<IList<ACInvoiceTermplate>> GetInvoiceTemplatesAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector)
            {
                Url = "acumulus/stable/picklists/picklist_invoicetemplates.php",
                withcontract = true
            };
            string t = await PostAsync(obj).ConfigureAwait(false);
            return AcumulusBaseObject.ListFromXML<ACInvoiceTermplate>(t);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ACClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
