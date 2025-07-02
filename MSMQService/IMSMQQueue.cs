using System;

namespace MSMQ
{
    public interface IMSMQQueue
    {
        void StartHandler(Func<object, object> callbackFunction);
        void StopHandler();
        string Send(object messageBody, TimeSpan timeout);
        object ReceiveByCorrelationId(string correlationId);
        bool IsHandlerRunning();
        string RequestQueueName { get; }
        string RequestQueuePath { get; }
        string ResponseQueueName { get; }
        string ResponseQueuePath { get; }
    }
}