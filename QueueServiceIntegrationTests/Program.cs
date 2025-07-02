using LoggerHelper;
using MSMQHandlerService.Services;
using System;

namespace QueueService.Integration.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LoggerFactory.GetLogger(new DummyLogConfiguration("", ""));

            var serviceNowQueueService = new QueueServiceServiceNow(QueueServiceBase.QueueType.MSMQ, true, logger);

            var handler = new QueueServiceHandlerProcessQueues(
                serviceNowQueueService,
                new ServiceNowService(logger).HandlerCallbackAsync,
                logger);
            var purgeHandler = new QueueServiceHandlerPurgeQueues(serviceNowQueueService, new TimeSpan(), new TimeSpan(), logger);

            handler.Start();
            purgeHandler.Start();
            Console.Read();
            handler.Stop();
            purgeHandler.Stop();
        }
    }
}
