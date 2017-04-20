using System;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{

    [Serializable]
    [XmlRoot("customer")]

    public class Customer : AcumulusBaseObject
    {
        private bool v;

        public Customer() : base()
        {
            Url = "/acumulus/stable/contacts/contact_manage.php";
            UrlPickList = "/acumulus/stable/picklists/picklist_contacttypes.php";
        }

        public string type { get; set; }
        public string contactyourid { get; set; }
        public string contactstatus { get; set; }
        public string companyname1 { get; set; }
        public string companyname2 { get; set; }
        public string fullname { get; set; }
        public string salutation { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string postalcode { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countrycode { get; set; }
        public string vatnumber { get; set; }
        public string telephone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string overwriteifexists { get; set; }
        public string bankaccountnumber { get; set; }
        public string mark { get; set; }
        public string disableduplicates { get; set; }
        public ACInvoice invoice { get; set; }
    }
}