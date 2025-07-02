using LoggerHelper;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using QueueService;
using System;
using System.Threading;
using System.Threading.Tasks;
using static QueueService.QueueServiceBase;

namespace MSMQ.IntegrationTests
{
    public class MSMQIntegrationTest
    {
        static CancellationTokenSource _cancellationTokenSource;
        static ServiceNowCreateTicketQueue _serviceNowCreateTicketQueue;
        static ServiceNowUpdateTicketQueue _serviceNowUpdateTicketQueue;
        static ServiceNowResultTicketQueue _serviceNowResultTicketQueue;
        static ServiceNowTicketQueue _serviceNowTicketQueue;
        static Func<object, CancellationToken, Task<object>> _testServiceNowFunction;
        static Func<object, CancellationToken, Task<object>> _testExceptionFunction;
        static QueueServiceHandlerProcessQueues _MSMQHandler;
        static IQueueServiceServiceNow _queueServiceServiceNow;
        static IQueueService _queueService;
        static ILogger _logger;

        const string TESTQUEUENAME = "TestQueue";
        const string TESTRESPONSEQUEUENAME = "TestQueueResponse";
        const string TESTFAILEDQUEUENAME = "TestFailedQueue";
        const string BODYEXCEP = "BODY Exception";
        const string TICKETNUMBER1 = "TicketNumber1";
        const string TICKETNUMBER2 = "TicketNumber2";
        const string TABLENAME = "Tablename";
        const string MESSAGEID = "messageid";
        const string MESSAGEID2 = "messageid2";

        private static void Initialize()
        {

            _cancellationTokenSource = new CancellationTokenSource();
            _logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    "DEBUG", 
                    "MSMQIntegrationTests", 
                    new NlogTargetConfigurationBase[] { new NlogConsoleTargetConfiguration() }));

            _serviceNowCreateTicketQueue = new ServiceNowCreateTicketQueue()
            {
                TableName = TABLENAME,
                
            };
            _serviceNowUpdateTicketQueue = new ServiceNowUpdateTicketQueue()
            {
                TableName = TABLENAME
            };
            _serviceNowTicketQueue = new ServiceNowTicketQueue()
            {
                Fields = new string[][] { new string[] { "Key", "Value" } }
            };
            _testServiceNowFunction = (m, t) =>
            {
                Thread.Sleep(5000);
                return Task.Run(() => (object)new ServiceNowResultTicketQueue()
                {
                    Tickets = new ServiceNowTicketQueue[] { _serviceNowTicketQueue }
                }, _cancellationTokenSource.Token);
            };
            _testExceptionFunction = (m, t) =>
            {
                _logger.Error("Exception called");
                return null;
            };
            _serviceNowResultTicketQueue = new ServiceNowResultTicketQueue();
            _queueServiceServiceNow = new QueueServiceServiceNow(QueueType.MSMQ, true, _logger);
            _queueService = new QueueServiceServiceNow(QueueType.MSMQ, true, _logger);

            StartHandler();
        }

        private static void StartHandler()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _MSMQHandler = new QueueServiceHandlerProcessQueues(
                _queueService,
                _testServiceNowFunction,
                _logger);
        }

        public static void TestHandlers()
        {
            Initialize();

            try
            {
                var messageId1 = _queueServiceServiceNow.CreateTicketQueueAsync(
                    _serviceNowCreateTicketQueue,
                    _cancellationTokenSource.Token).Result;

                _logger.Info(messageId1);

                _logger.Info("Waiting for response...");

                var body1 = _queueServiceServiceNow.GetMessageAsync(messageId1, _cancellationTokenSource.Token).Result;
                var message1 = (ServiceNowResultTicketQueue)body1;
                if (message1.Tickets[0].Fields[0][0] != "Key") throw new Exception("Test Unsuccessful");

                _cancellationTokenSource.Cancel();

                var messageId2 = _queueServiceServiceNow.UpdateTicketQueueAsync(
                    _serviceNowUpdateTicketQueue,
                    _cancellationTokenSource.Token).Result;
                
                StartHandler();

                var body2 = _queueServiceServiceNow.GetMessageAsync(messageId2, _cancellationTokenSource.Token).Result;
                var message2 = (ServiceNowResultTicketQueue)body2;
                if (message2.Tickets[0].Fields[0][0] != "Key") throw new Exception("Test Unsuccessful");

                _cancellationTokenSource.Cancel();

                var messageIdExc = _queueServiceServiceNow.CreateTicketQueueAsync(
                    _serviceNowCreateTicketQueue,
                    _cancellationTokenSource.Token);

                StartHandler();
                var body3 = _queueServiceServiceNow.GetMessageAsync(messageId1, _cancellationTokenSource.Token).Result;
                var message3 = (ServiceNowResultTicketQueue)body3;
                if (message3.Tickets[0].Fields[0][0] != "Key") throw new Exception("Test Unsuccessful");

                _logger.Info("Tests Successful");

                _cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public static void TestMessageRetrieval()
        {
            Initialize();

            var messageId1 = _queueServiceServiceNow.CreateTicketQueueAsync(
                _serviceNowCreateTicketQueue,
                _cancellationTokenSource.Token).Result;

            var task = Task.Run(() =>
            {
                object result = null;
                result = _queueServiceServiceNow.GetMessageAsync(messageId1, _cancellationTokenSource.Token).Result;
                _logger.Debug(result);

                return result;
            });

            Task.Run(() =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var message = _queueService.WaitOnQueue(new TimeSpan(0, 0, 1));
                        _queueService.ProcessQueue(_testServiceNowFunction, _cancellationTokenSource.Token, 1);
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            });

            var body1 = task.Result;
            Thread.Sleep(10000);

            _cancellationTokenSource.Cancel();
            _logger.Info("Cancelling token");
            
            var message1 = (ServiceNowResultTicketQueue)body1;
            _logger.Info(message1);
            if (message1.Tickets[0].Fields[0][0] != "Key") throw new Exception("Test Unsuccessful");
            _logger.Info("Test Successful");
        }

        public static void TestHandlerRetry()
        {
            Initialize();

            _MSMQHandler = new QueueServiceHandlerProcessQueues(
                _queueService,
                _testServiceNowFunction,
                _logger);

        }
    }
}
