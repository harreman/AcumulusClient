using System;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{

    /// <remarks/>
    [Serializable, XmlRoot("invoicetemplates")]
    public partial class Invoicetemplates
    {

        private ACInvoiceTermplate[] invoicetemplateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("invoicetemplate")]
        public ACInvoiceTermplate[] invoicetemplate
        {
            get
            {
                return this.invoicetemplateField;
            }
            set
            {
                this.invoicetemplateField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable, XmlRoot("invoicetemplate")]
    public partial class ACInvoiceTermplate
    {

        private string invoicetemplateidField;

        private string invoicetemplatenameField;

        /// <remarks/>
        public string invoicetemplateid
        {
            get
            {
                return this.invoicetemplateidField;
            }
            set
            {
                this.invoicetemplateidField = value;
            }
        }

        /// <remarks/>
        public string invoicetemplatename
        {
            get
            {
                return this.invoicetemplatenameField;
            }
            set
            {
                this.invoicetemplatenameField = value;
            }
        }
    }


}
