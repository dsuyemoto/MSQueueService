using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.Text;

namespace Imanage.Tests
{
    [TestFixture()]
    public class ImanageDocumentNrlTests
    {
        const string DATABASE = "database";
        const string SESSION = "session";
        const string NUMBER = "007";
        const string VERSION = "1";
        const string DOCUMENTOBJECTID = "!nrtdms:0:!session:" + SESSION + ":!database:" + DATABASE + ":!document:" + NUMBER + "," + VERSION + ":";
        const string DESCRIPTION = "description";

        [Test()]
        public void ImanageDocumentLink_ImanageDocumentOutputTest()
        {
            var nrlLinkbyte = Encoding.UTF8.GetBytes(SESSION + Environment.NewLine
                    + DOCUMENTOBJECTID + Environment.NewLine
                    + "[Version]" + Environment.NewLine + "Latest=Y");
            var response = new RestResponse();
            response.Content = JsonConvert.SerializeObject(
                new DocumentResponseSingle()
                {
                    data = new DocumentResponseSingleData()
                    {
                        Name = DESCRIPTION,
                        Number = NUMBER,
                        Version = VERSION
                    }
                });
            response.StatusCode = System.Net.HttpStatusCode.OK;
            var imanageDocumentOutput = new ImanageDocumentOutput(DATABASE, SESSION, response);

            var nrlLink = new ImanageDocumentNrl(imanageDocumentOutput);

            Assert.AreEqual(nrlLinkbyte, nrlLink.Data);
            Assert.AreEqual(DESCRIPTION + ".nrl", nrlLink.FileName);
        }
    }
}
