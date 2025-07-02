using NUnit.Framework;

namespace Imanage.Tests
{
    [TestFixture()]
    public class ImanageUserTests
    {
        [Test()]
        public void ImanageUserTest()
        {
            string user = "user";
            var right = ImanageAclItem.ImanageAccessRight.ALL;
            var aclitem = ImanageAclItem.ImanageAclItemType.USER;

            var imanageUser = new ImanageAclItem(user, right, aclitem);

            Assert.AreEqual(user, imanageUser.Name);
            Assert.AreEqual(right, imanageUser.ImanAccessRight);
        }

        [Test()]
        public void ImanageUserAclItemTest()
        {
            var name = "user";

            var imanageUser = new ImanageAclItem(
                name, 
                ImanageAclItem.ImanageAccessRight.ALL, 
                ImanageAclItem.ImanageAclItemType.USER);

            Assert.AreEqual(name, imanageUser.Name);
            Assert.AreEqual(ImanageAclItem.ImanageAccessRight.ALL, imanageUser.ImanAccessRight);
        }
    }
}
