using System.Configuration;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{
    [XmlRoot("contact")]
    public class Contract
    {
        public Contract() : base()
        {
            contractcode = ConfigurationManager.AppSettings["contractcode"];
            username = ConfigurationManager.AppSettings["username"];
            password = ConfigurationManager.AppSettings["password"];
            emailonerror = ConfigurationManager.AppSettings["emailonerror"];
            emailonwarning = ConfigurationManager.AppSettings["emailonwarning"];
        }
        public string contractcode { get; set; }
        public string username { get; set; }

        public string password { get; set; }
        public string emailonerror { get; set; }
        public string emailonwarning { get; set; }

    }
}
