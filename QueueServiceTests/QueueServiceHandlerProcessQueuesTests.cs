using LoggerHelper;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService.Tests
{
    [TestFixture()]
    public class QueueServiceHandlerProcessQueuesTests
    {
        IQueueService _mockQueueService;
        QueueServiceHandlerProcessQueues _msmqHandler;
        ILogger _mockLogger;

        [SetUp()]
        public void Setup()
        {
            _mockLogger = Mock.Of<ILogger>();
            _mockQueueService = Mock.Of<IQueueService>();
            Mock.Get(_mockQueueService).Setup(q => q.WaitOnQueue(It.IsAny<TimeSpan>())).Returns(null);
            Mock.Get(_mockQueueService).Setup(q => q.ProcessQueue(
                It.IsAny<Func<object, CancellationToken, Task<object>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<int>()));
            _msmqHandler = new QueueServiceHandlerProcessQueues(
                _mockQueueService,
                (o, t) => { return Task.Run(() => new object()); },
                _mockLogger);
        }

        [Test()]
        public void Start_ProcessQueue_CalledOnceTest()
        {
            _msmqHandler = new QueueServiceHandlerProcessQueues(
                _mockQueueService,
                (o, t) => { return Task.Run(() => new object()); },
                _mockLogger);
            _msmqHandler.Start();

            Mock.Get(_mockQueueService).Verify(q => q.ProcessQueue(
                It.IsAny<Func<object, CancellationToken, Task<object>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<int>()), Times.Once);
        }
    }
}
