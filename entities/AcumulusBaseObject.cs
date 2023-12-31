﻿using System;
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
        public AcumulusBaseObject()
        {
        }
        public AcumulusBaseObject(Contract _contract, Connector _connector) : base()
        {
            Contract = _contract;
            Connector = _connector;
        }


        [XmlIgnore]
        public string Url = "";
        [XmlIgnore]
        public string UrlPickList = "";

        [XmlElement(ElementName = "contract")]
        public virtual Contract Contract { get; set; }
        [XmlElement(ElementName = "connector")]
        public virtual Connector Connector { get; set; }
        public string format = "xml";
        [XmlIgnore]
        public int testmode = 1;
        [XmlIgnore]
        private bool _withcontract = false;
        [XmlIgnore]
        public bool withcontract
        {
            get => _withcontract;
            set => _withcontract = value;
        }
        [XmlIgnore]
        private string _contactid = null;
        public string contactid
        {
            get => _contactid;
            set => _contactid = value;
        }
        [XmlIgnore]
        private string _entryid = null;

        public string entryid

        {
            get => _entryid;
            set => _entryid = value;
        }

        public string ToXML()

        {
            XmlSerializer x = new XmlSerializer(GetType());
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
            {
                fullName = "AcumulusClient.entities.ACInvoice";
            }
            else if (fullName == "account")
            {
                fullName = "AcumulusClient.entities.ACAccount";
            }
            else if (fullName == "invoicetemplate")
            {
                fullName = "AcumulusClient.entities.ACInvoiceTermplate";
            }
            else
            {
                fullName = "AcumulusClient.entities." + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fullName);
            }

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
            data = data.Replace(Environment.NewLine, "");
            char tab = '\u0009';
            data = data.Replace(tab.ToString(), "");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                List<T> returnlist = new List<T>();
                foreach (XmlNode innernode in node.ChildNodes)
                {
                    Type found = FindType(innernode.Name);
                    if (found == null)
                    {
                        continue;
                    }

                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(innernode.OuterXml)))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        returnlist.Add((T)serializer.Deserialize(stream));
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

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stream);

                }
            }
            return default(T);
        }
    }
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
