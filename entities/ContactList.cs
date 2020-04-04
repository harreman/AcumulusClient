using System;
using System.Collections.Generic;
using System.Text;

namespace AcumulusClient.entities
{
    [Serializable]
    public  class ContactList:AcumulusBaseObject
    {


        public int contactstatus { get; set; }
        public int contacttype { get; set; }
        public String filter { get;  set; }
    }
}
