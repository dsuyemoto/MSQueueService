using LoggerHelper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService
{
    public class QueueServiceHandlerProcessQueues : QueueServiceHandlerBase
    {
        int _retries;
        CancellationTokenSource _cancellationTokenSource;
        ILogger _logger;
        Func<object, CancellationToken, Task<object>> _callbackFunction;

        public QueueServiceHandlerProcessQueues(
            IQueueService queueService,
            Func<object, CancellationToken, Task<object>> callbackFunction,
            ILogger logger,
            int retries = 5)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            _logger.Info("Initializing handler " + queueService.RequestQueueName);

            _queueService = queueService;
            _callbackFunction = callbackFunction;
            _cancellationTokenSource = new CancellationTokenSource();
            _retries = retries;
        }

        public override void Start()
        {
            _logger.Info("Starting " + _queueService.RequestQueueName + " handler");

            _queueService.ProcessQueue(_callbackFunction, _cancellationTokenSource.Token, _retries);
        }

        public override void Stop()
        {
            _logger.Info("Stopping " + _queueService.RequestQueueName + " handler");

            _cancellationTokenSource.Cancel();
        } 
    }
}
