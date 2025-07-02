using LoggerHelper;
using Moq;
using MSMQHandlerService.Services;
using NUnit.Framework;
using System;
using System.Threading;

namespace MSMQHandlerService.Tests.Services
{
    [TestFixture]
    public class CacheServiceTests
    {
        TestService1 _testService1;
        TestService2 _testService2;
        ICacheService _cacheService;
        TestObject _testObject;
        object _service;
        CancellationToken _token;
        ILogger _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockLogger = Mock.Of<ILogger>();
            _testService1 = new TestService1();
            _testService2 = new TestService2();
            _cacheService = new CacheService(_mockLogger);
            _testObject = new TestObject() { Name = "TEST", Creds = "creds" };
            _token = new CancellationToken();
        }

        [Test]
        public void CacheService_Service1_AreEqualTest()
        {
            _service = _cacheService.GetConnectionAsync(
                _testObject,
                () => { return _testService1; }, _token).Result;
            
            Assert.AreEqual(_testService1, _service);
        }

        [Test]
        public void CacheService_Service2_AreEqualTest()
        {
            _testObject = new TestObject() { Name = "TEST2", Creds = "creds" };
            _service = _cacheService.GetConnectionAsync(
                _testObject,
                () => { return _testService2; }, _token).Result;

            Assert.AreEqual(_testService2, _service);
        }
    }

    class TestObject
    {
        public string Name { get; set; }
        public string Creds { get; set; }
    }

    class TestService1 { }
    class TestService2 { }
}
