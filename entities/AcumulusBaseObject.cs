using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{
    [Serializable]
    [XmlRoot(ElementName = "myxml")]
    public class AcumulusBaseObject

    {
        public AcumulusBaseObject(Contract _contract) : base()
        {
            contract = _contract;
        }

        [XmlIgnore]
        public string Url = "";
        [XmlIgnore]
        public string UrlPickList = "";
        public Contract contract { get; set; }
        public string format = "xml";
        [XmlIgnore]
        public int testmode = 1;
        [XmlIgnore]
        private bool _withcontract = false;
        [XmlIgnore]
        public bool withcontract
        {
            get
            {
                return _withcontract;
            }
            set
            {
                _withcontract = value;
            }
        }
        [XmlIgnore]
        private string _contactid = null;
        public string contactid
        {
            get
            {
                return _contactid;
            }
            set
            {
                _contactid = value;
            }
        }
        [XmlIgnore]
        private string _entryid = null;

        public string entryid

        {
            get
            {
                return _entryid;
            }
            set
            {
                _entryid = value;
            }
        }
        [XmlElement("connector")]
        public Connector zconnector { get; set; }
        public string ToXML()

        {
            if (!withcontract)
            {
                contract = new Contract();

                zconnector = new Connector();
            }
            XmlSerializer x = new XmlSerializer(this.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (Utf8StringWriter sww = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    x.Serialize(writer, this, ns);

                    return sww.ToString(); // Your XML

                }
            }
        }
        private static Type FindType(string fullName)
        {
            if (fullName == "invoice")
                fullName = "AcumulusClient.entities.ACInvoice";
           else if (fullName == "account")
                fullName = "AcumulusClient.entities.ACAccount";
            else if (fullName == "invoicetemplate")
                fullName = "AcumulusClient.entities.ACInvoiceTermplate";
            else
                fullName = "AcumulusClient.entities." + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fullName);
            try
            {
                Type found = AppDomain.CurrentDomain.GetAssemblies()
                         .Where(a => !a.IsDynamic)
                         .SelectMany(a => a.GetTypes())
                         .FirstOrDefault(t => t.FullName.ToLower().Contains(fullName.ToLower()));
                return found;

            }
            catch { }
            return null;
        }
        public static List<T> ListFromXML<T>(string data)
        {
            data= data.Replace(Environment.NewLine, "");
            char tab = '\u0009';
            data = data.Replace(tab.ToString(), "");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                List<T> returnlist = new List<T>();
                foreach (XmlNode innernode in node.ChildNodes)
                {
                    var found = FindType(innernode.Name);
                    if (found == null)
                        continue;
                    try
                    {
                        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(innernode.OuterXml)))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(T));
                            returnlist.Add((T)serializer.Deserialize(stream));
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
                return returnlist;
            }
            return null;
        }
        public static T FromXML<T>(string data)
        {
            data = data.Replace(Environment.NewLine, "");
            char tab = '\u0009';
            data = data.Replace(tab.ToString(), "");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(node.OuterXml)))
                {
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        return (T)serializer.Deserialize(stream);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
            return default(T);
        }
    }
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
