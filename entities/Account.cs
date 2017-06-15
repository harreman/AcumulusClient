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
            get
            {
                return this.accountField;
            }
            set
            {
                this.accountField = value;
            }
        }
    }

    /// <remarks/>
    [Serializable, XmlRoot("account")]
    public partial class ACAccount
    {

        private object accountidField;

        private object accountnumberField;

        private object accountdescriptionField;

        /// <remarks/>
        public object accountid
        {
            get
            {
                return this.accountidField;
            }
            set
            {
                this.accountidField = value;
            }
        }

        /// <remarks/>
        public object accountnumber
        {
            get
            {
                return this.accountnumberField;
            }
            set
            {
                this.accountnumberField = value;
            }
        }

        /// <remarks/>
        public object accountdescription
        {
            get
            {
                return this.accountdescriptionField;
            }
            set
            {
                this.accountdescriptionField = value;
            }
        }
    }


}
