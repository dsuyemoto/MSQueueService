using LoggerHelper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService
{
    public abstract class QueueServiceBase : IQueueService
    {
        protected ILogger _logger;
        protected IQueueService _queueService;
        protected TimeSpan _responseTimeout;

        public string RequestQueueName => _queueService.RequestQueueName;

        public string ResponseQueueName => _queueService.ResponseQueueName;

        public QueueServiceBase(ILogger logger)
        {
            if (logger != null)
                _logger = logger;
        }

        public enum QueueType
        {
            MSMQ,
            MSQueue
        }

        public async Task<string> RequestAsync(MsmqMessage message, CancellationToken token)
        {
            return await _queueService.RequestAsync(message, token);
        }

        public async Task<object> ResponseAsync(string id, bool deleteMessage, CancellationToken token)
        {
            return await _queueService.ResponseAsync(id, deleteMessage, token);
        }

        public async Task<object> GetMessageAsync(string messageId, CancellationToken token, bool deleteMessage = false)
        {
            return await ResponseAsync(messageId, false, token);
        }

        public object WaitOnQueue(TimeSpan timeout)
        {
            return _queueService.WaitOnQueue(timeout);
        }

        public void PurgeQueues(TimeSpan expirationLength)
        {
            _queueService.PurgeQueues(expirationLength);
        }

        public void ProcessQueue(
            Func<object, CancellationToken, Task<object>> callbackFunction,
            CancellationToken token,
            int maxRetries)
        {
            _queueService.ProcessQueue(callbackFunction, token, maxRetries);
        }
    }
}