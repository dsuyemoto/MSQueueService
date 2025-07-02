using NUnit.Framework;

namespace Imanage.Tests
{
    [TestFixture()]
    public class JsonNetSerializerTests
    {

        [Test()]
        public void JsonNetSerializer_Serializer_AreEqualTest()
        {
            var json = "{" + "\"author\":\"author\"" + "}";
            var jsonNetSerializer = new CustomJsonNetSerializer();
            var result = jsonNetSerializer.Serialize(new { Author = "author" });

            Assert.AreEqual(json, result);
        }
    }
}
