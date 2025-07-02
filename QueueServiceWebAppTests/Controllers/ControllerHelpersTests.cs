using NUnit.Framework;
using QueueServiceWebApp.Controllers;
using System;
using System.Text;

namespace QueueServiceWebApp.Tests.Controllers
{
    public class ControllerHelpersTests
    {
        const string MESSAGEID = "messageid";

        [Test()]
        public void Base64ConvertFrom_MessageIdBase64_AreEqualTest()
        {
            var messageIdBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(MESSAGEID));

            var result = ControllerHelpers.Base64ConvertFrom(messageIdBase64);

            Assert.AreEqual(MESSAGEID, result);
        }

        [Test()]
        public void Base64ConvertTo_MessageId_AreEqualTest()
        {
            var messageIdBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(MESSAGEID));

            var result = ControllerHelpers.Base64ConvertTo(MESSAGEID);

            Assert.AreEqual(messageIdBase64, result);
        }
    }
}
