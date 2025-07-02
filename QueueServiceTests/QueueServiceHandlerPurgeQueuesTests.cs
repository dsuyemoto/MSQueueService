using LoggerHelper;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace QueueService.Tests
{
    [TestFixture]
    public class QueueServiceHandlerPurgeQueuesTests
    {
        QueueServiceHandlerProcessQueues _msmqHandler;
        IQueueService _mockQueueService;
        ILogger _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockLogger = Mock.Of<ILogger>();
            _mockQueueService = Mock.Of<IQueueService>();
            Mock.Get(_mockQueueService).Setup(q => q.PurgeQueues(It.IsAny<TimeSpan>()));
            Mock.Get(_mockQueueService).Setup(q => q.RequestQueueName).Returns("TestRequestQueue");
            _msmqHandler = new QueueServiceHandlerProcessQueues(
                _mockQueueService,
                (o, t)=> { return Task.Run(() => new object()); },
                _mockLogger);
        }

        [TearDown]
        public void Teardown()
        {
             _msmqHandler.Stop();
        }
    }
}
