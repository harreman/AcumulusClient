using System;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{
    [Serializable]
    [XmlRoot("product")]
    public class Product : AcumulusBaseObject
    {
        public Product() : base()
        {
            UrlPickList = "/acumulus/stable/picklists/picklist_products.php";
        }

        public string productid { get; set; }
        public string productnature { get; set; }
        public string productdescription { get; set; }
        public string producttagid { get; set; }
        public string productcontactid { get; set; }
        public string productprice { get; set; }
        public string productvatrate { get; set; }
        public string productsku { get; set; }
        public string productean { get; set; }
        public string productnotes { get; set; }
    }
}
