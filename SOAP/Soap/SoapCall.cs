using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Soap
{
    public class SoapCall : ISoapCall
    {
        ISoapWebService _soapWebService;

        XNamespace ENV = "http://schemas.xmlsoap.org/soap/envelope/";
        const string ENC = "http://schemas.xmlsoap.org/soap/encoding/";
        const string XSI = "http://www.w3.org/2001/XMLSchema-instance";
        const string XSD = "http://www.w3.org/2001/XMLSchema";

        public SoapCall(ISoapWebService soapWebService)
        {
            _soapWebService = soapWebService;
        }

        public IEnumerable<Dictionary<string, string>> Call(
            string rootNode, 
            Dictionary<string, string> nodeHash, 
            string serviceUrl, 
            string userName, 
            string password, 
            string resultRootNodeName = null)
        {
            var xDocument = _soapWebService.Post(CreateSoapEnvelope(rootNode, nodeHash), serviceUrl, userName, password);

            return GetResultsFromXDocument(xDocument, resultRootNodeName);
        }

        private List<Dictionary<string, string>> GetResultsFromXDocument(XDocument xDocument, string resultRootNodeName)
        {
            var listFields = new List<Dictionary<string, string>>();

            if (xDocument == null) return listFields;

            var rootElements = xDocument.Descendants(resultRootNodeName);
            foreach (var rootElement in rootElements)
            {
                var elements = rootElement.Descendants();
                var row = new Dictionary<string, string>();
                foreach (var element in elements)
                    row.Add(element.Name.LocalName, element.Value);  
                                 
                listFields.Add(row);
            }
            return listFields;
        }

        private XDocument CreateSoapEnvelope(string rootNode, Dictionary<string, string> nodeHash)
        {
            var elementList = new List<XElement>();

            foreach (var node in nodeHash)
            {
                if (!string.IsNullOrEmpty(node.Value))
                    elementList.Add(new XElement(node.Key, node.Value));
            }

            var envelope = new XElement(ENV + "Envelope",
                new XAttribute(XNamespace.Xmlns + "SOAP-ENV", ENV.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "SOAP-ENC", ENC),
                new XAttribute(XNamespace.Xmlns + "xsi", XSI),
                new XAttribute(XNamespace.Xmlns + "xsd", XSD),
                new XAttribute(ENV + "encodingStyle", ENV.NamespaceName),
                new XElement(ENV + "Body", 
                    new XElement(rootNode, elementList.ToArray())
                    ));

            var xDocument = new XDocument();
            xDocument.Add(envelope);

            return xDocument;
        }
    }
}
