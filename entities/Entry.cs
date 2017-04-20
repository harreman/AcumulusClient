using System;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{
    [Serializable]
    [XmlRoot("entry")]
    public class Entry : AcumulusBaseObject
    {
        public string token { get; set; }
        public string number { get; set; }
        public string issuedate { get; set; }
        public string expirationdate { get; set; }
        public int daysdue { get; set; }
        public int invoicedaylimit { get; set; }
        public string contactname { get; set; }
        public int accountid { get; set; }
        public string accountnumber { get; set; }
        public string amount { get; set; }
        public string entrydate { get; set; }
        public string entrytype { get; set; }
        public string entrydescription { get; set; }
        public string entrynote { get; set; }
        public string fiscaltype { get; set; }
        public string vatreversecharge { get; set; }
        public string foreigneu { get; set; }
        public string foreignnoneu { get; set; }
        public string marginscheme { get; set; }

        public string costcenterid { get; set; }
        public string costtypeid { get; set; }
        public string invoicenumber { get; set; }
        public string invoicenote { get; set; }
        public string descriptiontext { get; set; }
        public string invoicelayoutid { get; set; }
        public string totalvalueexclvat { get; set; }
        public string totalvalue { get; set; }
        public string paymenttermdays { get; set; }
        public string paymentdate { get; set; }
        public string paymentstatus { get; set; }
        public string deleted { get; set; }
    }
}
