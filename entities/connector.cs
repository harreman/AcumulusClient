using System;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{
    [Serializable]
    [XmlRoot("connector")]
    public class Connector
    {
        public Connector() : base()
        {
            application = "HarremanIT";
            webkoppel = "WebPlugin";
            development = "Production";
            remark = "";
            sourceuri = "http://www.harreman.it";
        }
        public string application { get; set; }
        public string webkoppel { get; set; }
        public string development { get; set; }
        public string remark { get; set; }
        public string sourceuri { get; set; }
    }
}
