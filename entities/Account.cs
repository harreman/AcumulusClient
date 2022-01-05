using System;
using System.Xml.Serialization;

namespace AcumulusClient.entities
{

    /// <remarks/>
    [Serializable, XmlRoot("accounts")]
    public partial class Accounts
    {

        private ACAccount[] accountField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("account")]
        public ACAccount[] account
        {
            get => accountField;
            set => accountField = value;
        }
    }

    /// <remarks/>
    [Serializable, XmlRoot("account")]
    public partial class ACAccount
    {

        private string accountidField;

        private string accountnumberField;

        private string accountdescriptionField;

        /// <remarks/>
        public string accountid
        {
            get => accountidField;
            set => accountidField = value;
        }

        /// <remarks/>
        public string accountnumber
        {
            get => accountnumberField;
            set => accountnumberField = value;
        }

        /// <remarks/>
        public string accountdescription
        {
            get => accountdescriptionField;
            set => accountdescriptionField = value;
        }
    }


}
