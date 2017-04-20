using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcumulusClient.interfaces
{
    public interface IAcumulusObject
    {
        string ToXML();
        Type FindType(string fullName);
        List<T> ListFromXML<T>(string data);
    }
}
