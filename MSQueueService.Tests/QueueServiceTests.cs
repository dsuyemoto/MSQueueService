using NUnit.Framework;
using System;
using System.Threading;
using static QueueService.QueueService;

namespace QueueService.Tests
{
    [TestFixture()]
    public class QueueServiceTests
    {
        const string TESTNAME = "TEST";
        TestObject _testObject = new TestObject { Name = TESTNAME };
        QueueService _queueService;
        CancellationTokenSource _cancellationTokenSource;

        [SetUp()]
        public void Setup()
        {
            _queueService = new QueueService();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        //[Test()]
        //public void QueueService_StartHandler_Test()
        //{
        //    _cancellationTokenSource.CancelAfter(3000);

        //    _queueService.StartHandler((qi) =>
        //    {
        //        var queueOutput = new QueueResponse() { Output = qi };
        //        return queueOutput;
        //    }, _cancellationTokenSource.Token);
        //    var queueId = _queueService.AddRequest(new QueueRequest(_testObject));
        //    var response = _queueService.GetResponseAsync(queueId, _cancellationTokenSource.Token).Result;

        //    //Assert.AreEqual(TESTNAME, ((TestObject)response.Output).Name);
        //}

        [Test()]
        public void QueueService_CallbackException_TimeoutExceededTest()
        {
            var taskStart = _queueService.StartHandler((qi) =>
            {             
                throw new Exception();
            }, _cancellationTokenSource.Token);
            var taskStartStatus = taskStart.Status;
            while (taskStartStatus != System.Threading.Tasks.TaskStatus.Running) { taskStartStatus = taskStart.Status; }
            var queueId = _queueService.AddRequest(new QueueRequest(_testObject) { Timeout = new TimeSpan(0,0,1) });
            var response = _queueService.GetResponseAsync(queueId, _cancellationTokenSource.Token).Result;

            Assert.IsTrue(response.Errors.Count > 0);
        }

        [Test()]
        public void QueueService_CallbackException_QueueRequestQueuedTest()
        {
            var taskStart = _queueService.StartHandler((qi) =>
            {
                return _testObject;
            }, _cancellationTokenSource.Token);
            var taskStartStatus = taskStart.Status;
            while (taskStartStatus != System.Threading.Tasks.TaskStatus.Running) { taskStartStatus = taskStart.Status; }
            var queueId = _queueService.AddRequest(new QueueRequest(_testObject) { Timeout = new TimeSpan(0, 3, 0) });
            QueueRequest queueRequest = null;
            int timer = 0;
            while(queueRequest == null && timer < 1000)
            {
                queueRequest = _queueService.GetRequest(queueId);
                timer++;
            }

            Assert.AreEqual(0, queueRequest.Errors.Count);
        }

        [Test()]
        public void QueueService_CallbackException_QueueRequestStatusTest()
        {
            var taskStart = _queueService.StartHandler((qi) =>
            {
                throw new Exception();
            }, _cancellationTokenSource.Token);
            var taskStartStatus = taskStart.Status;
            while (taskStartStatus != System.Threading.Tasks.TaskStatus.Running) { taskStartStatus = taskStart.Status; }
            var queueId = _queueService.AddRequest(new QueueRequest(_testObject) { Timeout = new TimeSpan(0, 3, 0) });
            QueueItemStatus queueItemStatus = QueueItemStatus.None;
            int timer = 0;
            while ((queueItemStatus == QueueItemStatus.None || queueItemStatus == QueueItemStatus.Queued))
            {
                queueItemStatus = _queueService.GetQueueItemStatus(queueId);
                timer++;
            }

            Assert.AreEqual(QueueItemStatus.Error, queueItemStatus);
        }
    }

    public class TestObject
    {
        public string Name { get; set; }
        public TestObject()
        {

        }
    }
}
