using AcumulusClient.entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
        private readonly HttpClient client = new HttpClient();

        public ACClient(Contract _contract)
        {
            contract = _contract;
            client = new HttpClient();
            client.BaseAddress = new Uri("https://api.sielsystems.nl");
        }

        public AcumulusBaseObject Get(dynamic data)
        {

            string xmlstring = data.ToXML().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            var response = client.GetAsync(data.Url + "?xmlstring=" + HttpUtility.HtmlEncode(xmlstring)).Result;
            var text = response.Content.ReadAsStringAsync();
            return new AcumulusBaseObject(contract);

        }

        public string Post(AcumulusBaseObject data, bool removeentryelement = false)
        {
            var text = "";

            var result = client.PostAsync(data.Url, GetHttpRequestMessage(data, removeentryelement)).Result;
            text = result.Content.ReadAsStringAsync().Result;


            return text;

        }

        public string Put<t>(int id, AcumulusBaseObject data)
        {

            var requestMessage = GetHttpRequestMessage(data);

            var result = client.PutAsync(data.Url + id, requestMessage).Result;

            return result.Content.ReadAsStringAsync().Result;

        }

        public string Delete(int id, AcumulusBaseObject data)
        {

            var result = client.DeleteAsync(data.Url + id).Result;

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

            if (removeentryelement)
                xmlstring = xmlstring.Replace("<entryid />", alreadyencoded);

            var content = new FormUrlEncodedContent(new[]
            {
                     new KeyValuePair<string, string>("xmlstring", xmlstring)
                            });

            return content;
        }

        public void GetGeneralInfo()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract);
            obj.Url = "/acumulus/stable/general/my_acumulus.php";
            obj.withcontract = true;
            var t = Post(obj);
        }
        public IList<Contact> GetContactList()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract);
            obj.Url = "/acumulus/stable/contacts/contacts_list.php";
            obj.withcontract = true;
            var t = Post(obj);
            return AcumulusBaseObject.ListFromXML<Contact>(t);
        }


        public IList<Product> GetProducts()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract);
            obj.Url = "/acumulus/stable/picklists/picklist_products.php";
            obj.withcontract = true;
            var t = Post(obj);
            return AcumulusBaseObject.ListFromXML<Product>(t);
        }

        public IList<Entry> GetOpenInvoices()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract);
            obj.Url = "/acumulus/stable/reports/report_unpaid_debtors.php";
            obj.withcontract = true;
            var t = Post(obj);
            return AcumulusBaseObject.ListFromXML<Entry>(t);
        }


        public IList<ACInvoice> GetInvoices(string contactid)
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract);
            obj.Url = "/acumulus/stable/contacts/contact_invoices_outgoing.php";
            obj.contactid = contactid.ToString();
            obj.withcontract = true;
            var t = Post(obj);
            return AcumulusBaseObject.ListFromXML<ACInvoice>(t);
        }

        public Entry GetInvoiceDetail(string entryid)
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract);
            obj.Url = "/acumulus/stable/entry/entry_info.php";
            obj.entryid = entryid;
            obj.withcontract = true;
            var t = Post(obj);
            return AcumulusBaseObject.FromXML<Entry>(t);
        }

        public CreateInvoiceResponse CreateInvoice(Customer invoice)
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract);
            obj.withcontract = true;
            obj.Url = "/acumulus/stable/invoices/invoice_add.php";
            obj.entryid = invoice.ToXML().Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "").Replace("<format>xml</format>", "");
            var t = Post(obj, true);

            return new CreateInvoiceResponse()
            {
                Entryid = t.Substring(from: "<entryid>", until: "</entryid>"),
                Invoicenumber = t.Substring(from: "<invoicenumber>", until: "</invoicenumber>"),
                Token = t.Substring(from: "<token>", until: "</token>"),
                NonParsedMessage = t
            };
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public void SetPaidStatus(string token, DateTime? invoicedate)
        {

            DateTime invdate = invoicedate.HasValue ? (DateTime)invoicedate : DateTime.Now.Date;

            PaymentStatus obj = new PaymentStatus();
            obj.withcontract = true;
            obj.paymentdate = invdate.ToString("yyyy-MM-dd");
            obj.token = token;
            obj.paymentstatus = "2";
            obj.Url = "/acumulus/stable/invoices/invoice_paymentstatus_set.php";

            var t = Post(obj, true);
        }

        public bool EmailInvoice(EmailInvoice obj)
        {
            obj.withcontract = true;
            obj.invoicetype = "0";
            obj.Url = "/acumulus/stable/invoices/invoice_mail.php";

            var t = Post(obj, true);

            return true;

        }
        public bool RemindInvoice(EmailInvoice obj)
        {

            obj.withcontract = true;
            obj.invoicetype = "1";
            obj.Url = "/acumulus/stable/invoices/invoice_mail.php";

            var t = Post(obj, true);
            return true;
        }

        public IList<ACAccount> GetBankAccounts()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract);
            obj.Url = "acumulus/stable/picklists/picklist_accounts.php";
            obj.withcontract = true;
            var t = Post(obj);
            return AcumulusBaseObject.ListFromXML<ACAccount>(t);
        }
        public IList<ACInvoiceTermplate> GetInvoiceTemplates()
        {
            AcumulusBaseObject obj = new AcumulusBaseObject(contract);
            obj.Url = "acumulus/stable/picklists/picklist_invoicetemplates.php";
            obj.withcontract = true;
            var t = Post(obj);
            return AcumulusBaseObject.ListFromXML<ACInvoiceTermplate>(t);
        }
    }
}
