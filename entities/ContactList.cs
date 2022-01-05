using System;

namespace AcumulusClient.entities
{
    [Serializable]
    public class ContactList : AcumulusBaseObject
    {


        public int contactstatus { get; set; }
        public int contacttype { get; set; }
        public string filter { get; set; }
    }
}
