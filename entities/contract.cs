using System;
using System.Configuration;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{
    [XmlRoot("contact")]
    public class Contract
    {
        public Contract() : base()
        {

        }
        public string contractcode { get; set; }
        public string username { get; set; }

        public string password { get; set; }
        public string emailonerror { get; set; }
        public string emailonwarning { get; set; }
        public string BaseUrl { get;  set; }
    }
}
