using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using System.Xml.Linq;

namespace Soap.Tests
{
    [TestFixture()]
    public class SoapCallTests
    {
        XNamespace ENV = "http://schemas.xmlsoap.org/soap/envelope/";
        const string ENC = "http://schemas.xmlsoap.org/soap/encoding/";
        const string XSI = "http://www.w3.org/2001/XMLSchema-instance";
        const string XSD = "http://www.w3.org/2001/XMLSchema";
        const string GETRECORDSRESPONSE = "getRecordsResponse";
        const string GETRECORDSRESULT = "getRecordsResult";
        const string SYSID = "sys_id";
        const string SYSIDVALUE1 = "sysid1";
        const string SYSIDVALUE2 = "sysid2";

        [Test()]
        public void Call_Post_CalledOnce()
        {
            var mockSoapWebService = Mock.Of<ISoapWebService>();
            Mock.Get(mockSoapWebService).Setup(s => s.Post(It.IsAny<XDocument>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(CreateTestDocument());

            var soapCall = new SoapCall(mockSoapWebService);
            soapCall.Call("testNode", new Dictionary<string, string>() { { SYSID, SYSIDVALUE1 } }, "serviceurl", "username", 
                "password", GETRECORDSRESULT);

            Mock.Get(mockSoapWebService).Verify(s => s.Post(It.IsAny<XDocument>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        public XDocument CreateTestDocument()
        {
            var document = new XDocument();
            var envelope = new XElement(ENV + "Envelope",
                new XAttribute(XNamespace.Xmlns + "SOAP-ENV", ENV.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "SOAP-ENC", ENC),
                new XAttribute(XNamespace.Xmlns + "xsi", XSI),
                new XAttribute(XNamespace.Xmlns + "xsd", XSD),
                new XAttribute(ENV + "encodingStyle", ENV.NamespaceName),
                new XElement(ENV + "Body", 
                    new XElement(GETRECORDSRESPONSE, 
                        new XElement(
                            GETRECORDSRESULT, 
                            new XElement(SYSID, SYSIDVALUE1)
                            ),
                        new XElement(
                            GETRECORDSRESULT, 
                            new XElement(SYSID, SYSIDVALUE2)
                            )
                        )
                    )
                );

            document.Add(envelope);

            return document;
        }
    }
}
