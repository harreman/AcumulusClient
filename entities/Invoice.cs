using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{
    [Serializable]
    [XmlRoot("invoice")]
    public class ACInvoice : AcumulusBaseObject
    {
        [XmlIgnore]
        public override Contract Contract { get; set; }
        [XmlIgnore]
        public override Connector Connector { get; set; }
        public string concept { get; set; }
        public string concepttype { get; set; }
        public string vattype { get; set; }
        public string costcenter { get; set; }
        public string description { get; set; }
        public string template { get; set; }
        public string invoicenotes { get; set; }
        public string issuedate { get; set; }
        public string accountnumber { get; set; }
        public string paymentstatus { get; set; }
        public string token { get; set; }
        public string entrydate { get; set; }
        public string invoicenumber { get; set; }
        public string invoicedescription { get; set; }
        public string amount { get; set; }


        [XmlElement("line")]
        public List<ACInvoiceLine> Lines { get; set; }
        [XmlElement("emailaspdf")]
        public EmailAsPdf emailaspdf { get; set; }

    }
    [XmlRoot("line")]
    public class ACInvoiceLine
    {
        public string itemnumber { get; set; }
        public string product { get; set; }
        public string nature { get; set; }
        public string unitprice { get; set; }
        public int vatrate { get; set; }
        public string quantity { get; set; }
        public string costprice { get; set; }
    }

    [XmlRoot("emailaspdf")]
    public class EmailAsPdf
    {
        public string emailto { get; set; }
        public string emailbcc { get; set; }
        public string emailfrom { get; set; }
        public string subject { get; set; }
        public string confirmreading { get; set; }
    }
}
