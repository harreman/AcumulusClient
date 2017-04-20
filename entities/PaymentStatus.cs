namespace AcumulusClient.entities
{
    public class PaymentStatus : AcumulusBaseObject
    {
        public PaymentStatus() : base()
        {
            Url = "acumulus/stable/invoices/invoice_paymentstatus_set.php";
            UrlPickList = "";
        }
        public string token { get; set; }
        public string paymentstatus { get; set; }
        public string paymentdate { get; set; }

    }
}
