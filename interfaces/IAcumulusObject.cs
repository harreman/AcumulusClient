using System;
using System.Collections.Generic;

namespace AcumulusClient.interfaces
{
    public interface IAcumulusObject
    {
        string ToXML();
        Type FindType(string fullName);
        List<T> ListFromXML<T>(string data) where T : class;
    }
}
