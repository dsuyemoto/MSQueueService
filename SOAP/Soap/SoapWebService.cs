using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Soap
{
    public class SoapWebService : ISoapWebService
    {
        public SoapWebService()
        {
        }

        public XDocument Post(XDocument requestBodyXml, string serviceUrl, string username, string password)
        {
            IAsyncResult asyncResult = null;

            var webRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            var base64UsernamePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));

            webRequest.Headers.Add("Authorization", "Basic " + base64UsernamePassword);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            
            InsertSoapEnvelopeIntoWebRequest(requestBodyXml, webRequest);

            asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
      
            return XDocument.Parse(GetStringFromWebRequest(webRequest, asyncResult));
        }

        public XDocument Post(Action<HttpWebRequest> serializer, string serviceUrl, string username, string password)
        {
            IAsyncResult asyncResult = null;

            var webRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            var base64UsernamePassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));

            webRequest.Headers.Add("Authorization", "Basic " + base64UsernamePassword);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            serializer(webRequest);

            asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();

            var soapResult = GetStringFromWebRequest(webRequest, asyncResult);

            return XDocument.Parse(soapResult);
        }

        private static void InsertSoapEnvelopeIntoWebRequest(XDocument bodyXmlDocument, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
                bodyXmlDocument.Save(stream);
        }

        private static string GetStringFromWebRequest(HttpWebRequest webRequest, IAsyncResult asyncResult)
        {
            var soapResult = string.Empty;

            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                    soapResult = rd.ReadToEnd();
            
            return soapResult;
        }
    }
}
