using NUnit.Framework;

namespace EWS.Tests
{
    [TestFixture()]
    public class ExchEmailTests
    {
        [Test()]
        public void ExchEmailParameters_NotNullTest()
        {
            var email = new ExchEmail();

            Assert.NotNull(email.Content);
            Assert.NotNull(email.ToEmailAddresses);
            Assert.NotNull(email.CcEmailAddresses);
            Assert.NotNull(email.BccEmailAddresses);
            Assert.NotNull(email.Attachments);
        }
    }
}
