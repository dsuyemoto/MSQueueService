using NUnit.Framework;
using System;

namespace Imanage.Tests
{
    [TestFixture()]
    public class CustomJsonNetSerializerTests
    {
        [Test()]
        public void Serialize_String_AreEqualTest()
        {
            var customSerializer = new CustomJsonNetSerializer();

            var serialized = customSerializer.Serialize(new { Test = "test" });

            Assert.AreEqual("{\"test\":\"test\"}", serialized);
        }

        [Test()]
        public void Serialize_Object_AreEqualTest()
        {
            var customSerializer = new CustomJsonNetSerializer();

            var serialized = customSerializer.Serialize(new { Test = new { Test2 = new TestClass { Test = "test3" } } });

            Assert.AreEqual("{\"test\":{\"test2\":{\"test\":\"test3\"}}}", serialized);
        }

        [Test()]
        public void Serialize_Int_AreEqualTest()
        {
            var customSerializer = new CustomJsonNetSerializer();

            var serialized = customSerializer.Serialize(new { Test = 12 });

            Assert.AreEqual("{\"test\":12}", serialized);
        }

        [Test()]
        public void Serialize_DateTime_AreEqualTest()
        {
            var datetime = DateTime.Now;
            var formattedDateTime = datetime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var customSerializer = new CustomJsonNetSerializer();

            var serialized = customSerializer.Serialize(new { Test = datetime });

            Assert.AreEqual("{\"test\":\"" + formattedDateTime + "\"}", serialized);
        }
    }

    class TestClass
    {
        public string Test { get; set; }
    }
}
