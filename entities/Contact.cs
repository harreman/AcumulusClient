using System;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{
    [Serializable]
    [XmlRoot("contact")]
    public class Contact : AcumulusBaseObject
    {
        public Contact() : base()
        {
            Url = "/acumulus/stable/contacts/contact_manage.php";
            UrlPickList = "/acumulus/stable/picklists/picklist_contacttypes.php";
        }

        // public int contactid { get; set; }
        public string contactyourid { get; set; }
        public string contactemail { get; set; }
        public string contacttype { get; set; }
        public string overwriteifexists { get; set; }
        public string contactname1 { get; set; }
        public string contactname2 { get; set; }
        public string contactperson { get; set; }
        public string contactsalutation { get; set; }
        public string contactaddress1 { get; set; }
        public string contactaddress2 { get; set; }
        public string contactpostalcode { get; set; }
        public string contactcity { get; set; }
        public string contactlocationcode { get; set; }
        public string contactcountrycode { get; set; }
        public string contactcountry { get; set; }
        public string contactvatnumber { get; set; }
        public string contactvatratebase { get; set; }
        public string contacttelephone { get; set; }
        public string contactfax { get; set; }
        public string contactmark { get; set; }
        public string contactinvoicetemplateid { get; set; }
        public int contactstatus { get; set; }
        public string contactiban { get; set; }
        public string contactbic { get; set; }
        public string contactsepamandatenr { get; set; }
        public string contactsepamandatedate { get; set; }
        public string contactsepaincassostatus { get; set; }
        public string contactinvoicenotes { get; set; }
        public string contactabout { get; set; }

    }
}
