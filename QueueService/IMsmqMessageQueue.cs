using System;
using System.Messaging;
using System.Threading;

namespace QueueService
{
    public interface IMsmqMessageQueue
    {
        string MessageQueueName { get; }
        IMessageFormatter Formatter { get; }
        string Path { get;  }
        bool IsTransactional { get; }
        bool UseJournalQueue { get; }
        object PeekByCorrelationId(string messageId);
        object PeekByCorrelationId(string correlationId, TimeSpan timeout);
        object ReceiveByCorrelationId(string correlationId);
        Message Receive(TimeSpan timeout);
        void Send(MsmqMessage message);
        void SetPermissions(MsmqQueuePermission queuePermission);
        void Refresh();
        MessageQueue GetMessageQueue();
        void Purge(TimeSpan expirationLength);
        IAsyncResult BeginReceive();
        MsmqMessage EndReceive(IAsyncResult asyncResult);
        void AddReceiveCompleted(ReceiveCompletedEventHandler receiveCompletedEventHandler);
        void RemoveReceiveCompleted(ReceiveCompletedEventHandler receiveCompletedEventHandler);
    }
}