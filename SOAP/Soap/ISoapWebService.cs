using System;
using System.Net;
using System.Xml.Linq;

namespace Soap
{
    public interface ISoapWebService
    {
        XDocument Post(XDocument soapEnvelopeXml, string serviceUrl, string username, string password);
        XDocument Post(Action<HttpWebRequest> serializer, string serviceUrl, string username, string password);
    }
}
