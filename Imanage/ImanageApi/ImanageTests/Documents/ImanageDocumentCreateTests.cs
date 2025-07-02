using NUnit.Framework;
using static Imanage.DocumentProfileItems;

namespace Imanage.Tests.Documents
{
    [TestFixture()]
    public class ImanageDocumentCreateTests
    {
        [Test()]
        public void ImanageDocumentCreate_RequiredFields_AreEqualTest()
        {
            var imanageDocumentCreate = new ImanageDocumentCreate(
                new byte[1],
                "database",
                new DocumentProfileItemsCreate(
                    "author",
                    null,
                    null,
                    "operator",
                    "EML",
                    null),
                new ImanageSecurityObject(
                    new ImanageAclItem[] {
                        new ImanageAclItem(
                            "username",
                            ImanageAclItem.ImanageAccessRight.ALL,
                            ImanageAclItem.ImanageAclItemType.USER)
                    },
                    ImanageSecurityObject.SecurityType.PRIVATE));

            Assert.AreEqual("author", imanageDocumentCreate.DocumentProfileItems.Author);
            Assert.AreEqual("operator", imanageDocumentCreate.DocumentProfileItems.Operator);
            Assert.AreEqual("username", imanageDocumentCreate.SecurityObject.ImanageAclItems[0].Name);
            Assert.AreEqual(ImanageAclItem.ImanageAccessRight.ALL, imanageDocumentCreate.SecurityObject.ImanageAclItems[0].ImanAccessRight);
            Assert.AreEqual(ImanageAclItem.ImanageAclItemType.USER, imanageDocumentCreate.SecurityObject.ImanageAclItems[0].ImanAclType);
            Assert.AreEqual(ImanageSecurityObject.SecurityType.PRIVATE, imanageDocumentCreate.SecurityObject.Security);
            Assert.AreEqual("database", imanageDocumentCreate.Database);
        }
    }
}
