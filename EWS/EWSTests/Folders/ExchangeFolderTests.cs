using NUnit.Framework;

namespace EWS.Tests
{
    [TestFixture()]
    public class ExchangeFolderTests
    {
        [Test()]
        public void ExchangeFolderTest()
        {
            var exchangeFolder = new ExchFolder();

            Assert.IsNull(exchangeFolder.Name);
            Assert.IsNull(exchangeFolder.UniqueId);
        }
    }
}