using System.Xml.Serialization;

namespace AcumulusClient.entities
{
    public class EmailInvoice : AcumulusBaseObject
    {

        public EmailInvoice() : base()
        {
            Url = "acumulus/stable/invoices/invoice_mail.php";
            UrlPickList = "";
        }

        [XmlElement("emailaspdf")]
        public EmailAsPdf emailaspdf { get; set; }

        public string token { get; set; }
        public string invoicetype { get; set; }
    }


}
