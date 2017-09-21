using AcumulusClient.entities;
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
            var fromLength = (from ?? string.Empty).Length;
            var startIndex = !string.IsNullOrEmpty(from)
                ? @this.IndexOf(from, comparison) + fromLength
                : 0;

            if (startIndex < fromLength) { throw new ArgumentException("from: Failed to find an instance of the first anchor"); }

            var endIndex = !string.IsNullOrEmpty(until)
            ? @this.IndexOf(until, startIndex, comparison)
            : @this.Length;

            if (endIndex < 0) { throw new ArgumentException("until: Failed to find an instance of the last anchor"); }

            var subString = @this.Substring(startIndex, endIndex - startIndex);
            return subString;
        }
    }

    public partial class ACClient : IACClient
    {

        private readonly Contract contract;
        private readonly Connector connector;
        private readonly HttpClient client = new HttpClient();

        public ACClient(Contract _contract, Connector _connector)
        {
            contract = _contract;
            client = new HttpClient();
            connector = _connector;
            client.BaseAddress = new Uri("https://api.sielsystems.nl");
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        public async Task<AcumulusBaseObject> GetAsync(dynamic data)
        {

            string xmlstring = data.ToXML().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            var response = await client.GetAsync(data.Url + "?xmlstring=" + HttpUtility.HtmlEncode(xmlstring));
#if DEBUG
#pragma warning disable S1481 // Unused local variables should be removed
            var text = response.Content.ReadAsStringAsync();
#pragma warning restore S1481 // Unused local variables should be removed
#endif
            return new AcumulusBaseObject(contract, connector);

        }

        public async Task<string> PostAsync(AcumulusBaseObject data, bool removeentryelement = false)
        {
            var text = "";

            var result = await client.PostAsync(data.Url, GetHttpRequestMessage(data, removeentryelement));
            text = result.Content.ReadAsStringAsync().Result;


            return text;

        }

        public async Task<string> PutAsync<t>(int id, AcumulusBaseObject data)
        {

            var requestMessage = GetHttpRequestMessage(data);

            var result = await client.PutAsync(data.Url + id, requestMessage);

            return result.Content.ReadAsStringAsync().Result;

        }

        public async Task<string> DeleteAsync(int id, AcumulusBaseObject data)
        {

            var result = await client.DeleteAsync(data.Url + id);

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
                xmlstring = xmlstring.Replace("<entryid />", alreadyencoded);

            var content = new FormUrlEncodedContent(new[]
            {
                     new KeyValuePair<string, string>("xmlstring", xmlstring)
                            });

            return content;
        }

        public async Task GetGeneralInfoAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector);
            obj.Url = "/acumulus/stable/general/my_acumulus.php";
            obj.withcontract = true;
            await PostAsync(obj);
        }
        public async Task<IList<Contact>> GetContactListAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector);
            obj.Url = "/acumulus/stable/contacts/contacts_list.php";
            obj.withcontract = true;
            var t = await PostAsync(obj);
            return AcumulusBaseObject.ListFromXML<Contact>(t);
        }


        public async Task<IList<Product>> GetProductsAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector);
            obj.Url = "/acumulus/stable/picklists/picklist_products.php";
            obj.withcontract = true;
            var t = await PostAsync(obj);
            return AcumulusBaseObject.ListFromXML<Product>(t);
        }

        public async Task<IList<Entry>> GetOpenInvoicesAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector);
            obj.Url = "/acumulus/stable/reports/report_unpaid_debtors.php";
            obj.withcontract = true;
            var t = await PostAsync(obj);
            return AcumulusBaseObject.ListFromXML<Entry>(t);
        }


        public async Task<IList<ACInvoice>> GetInvoicesAsync(string contactid)
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector);
            obj.Url = "/acumulus/stable/contacts/contact_invoices_outgoing.php";
            obj.contactid = contactid.ToString();
            obj.withcontract = true;
            var t = await PostAsync(obj);
            return AcumulusBaseObject.ListFromXML<ACInvoice>(t);
        }

        public async Task<Entry> GetInvoiceDetailAsync(string entryid)
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector);
            obj.Url = "/acumulus/stable/entry/entry_info.php";
            obj.entryid = entryid;
            obj.withcontract = true;
            var t = await PostAsync(obj);
            return AcumulusBaseObject.FromXML<Entry>(t);
        }

        public async Task<CreateInvoiceResponse> CreateInvoiceAsync(Customer invoice)
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector);
            obj.withcontract = false;
            obj.Url = "/acumulus/stable/invoices/invoice_add.php";
            obj.entryid = invoice.ToXML().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("<format>xml</format>", "");
            var t = await PostAsync(obj, true);

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

            PaymentStatus obj = new PaymentStatus();
            obj.Contract = contract;
            obj.Connector = connector;
            obj.withcontract = true;
            obj.paymentdate = invdate.ToString("yyyy-MM-dd");
            obj.token = token;
            obj.paymentstatus = "2";
            obj.Url = "/acumulus/stable/invoices/invoice_paymentstatus_set.php";

            await PostAsync(obj, true);
        }

        public async Task<bool> EmailInvoiceAsync(EmailInvoice obj)
        {
            obj.Contract = contract;
            obj.Connector = connector;
            obj.withcontract = true;
            obj.invoicetype = "0";
            obj.Url = "/acumulus/stable/invoices/invoice_mail.php";
             await PostAsync(obj, true);

            return true;

        }
        public async Task<bool> RemindInvoiceAsync(EmailInvoice obj)
        {
            obj.Contract = contract;
            obj.Connector = connector;
            obj.withcontract = true;
            obj.invoicetype = "1";
            obj.Url = "/acumulus/stable/invoices/invoice_mail.php";

            await PostAsync(obj, true);
            return true;
        }

        public async Task<IList<ACAccount>> GetBankAccountsAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector);
            obj.Url = "acumulus/stable/picklists/picklist_accounts.php";
            obj.withcontract = true;
            var t = await PostAsync(obj);
            return AcumulusBaseObject.ListFromXML<ACAccount>(t);
        }
        public async Task<IList<ACInvoiceTermplate>> GetInvoiceTemplatesAsync()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract, connector);
            obj.Url = "acumulus/stable/picklists/picklist_invoicetemplates.php";
            obj.withcontract = true;
            var t = await PostAsync(obj);
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
