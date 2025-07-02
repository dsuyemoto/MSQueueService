using NUnit.Framework;

namespace EWS.Tests
{
    public class ExchFileTests
    {
        [Test()]
        public void ExchAttachmentFile_Parameters_AreEqualTest()
        {
            const string FILENAME = "filename";
            byte[] content = new byte[1];
            const string UNIQUEID = "uniqueid";

            var exchEmail = new ExchAttachmentFile(FILENAME, content, UNIQUEID);

            Assert.AreEqual(FILENAME, exchEmail.FileName);
            Assert.AreEqual(content, exchEmail.Content);
            Assert.AreEqual(UNIQUEID, exchEmail.UniqueId);
        }
    }
}
