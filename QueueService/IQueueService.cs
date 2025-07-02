using System;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService
{
    public interface IQueueService
    {
        string RequestQueueName { get; }
        string ResponseQueueName { get; }
        Task<string> RequestAsync(MsmqMessage message, CancellationToken token);
        Task<object> ResponseAsync(string id, bool deleteMessage, CancellationToken token);
        object WaitOnQueue(TimeSpan timeout);
        void PurgeQueues(TimeSpan expirationLength);
        Task<object> GetMessageAsync(string messageId, CancellationToken token, bool deleteMessage = false);
        void ProcessQueue(
            Func<object, CancellationToken, Task<object>> callbackFunction,
            CancellationToken token,
            int maxRetries);
    }
}