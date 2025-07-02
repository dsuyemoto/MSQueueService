using LoggerHelper;
using System;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQ
{
    public class MSMQQueue : IMSMQQueue
    {
        readonly Type[] _requestMessageFormatterTypes;
        readonly Type[] _responseMessageFormatterTypes;
        readonly MessageQueue _requestMessageQueue;
        readonly MessageQueue _responseMessageQueue;
        readonly bool _isTransactional;
        readonly bool _useJournalQueue;
        readonly bool _isRecoverable;
        Task _handler;
        CancellationTokenSource _handlerTokenSource = new CancellationTokenSource();

        const string PRIVATEQUEUE = "private$\\";
             
        public string RequestQueueName { get; }
        public string RequestQueuePath { get; }
        public string ResponseQueueName { get; }
        public string ResponseQueuePath { get; }
        public TimeSpan ResponseTimeout { get; set; } = new TimeSpan(24, 0, 0);

        public MSMQQueue(
            string requestQueueName,
            Type[] requestMessageFormatterTypes,
            string responseQueueName = null,
            Type[] responseMessageFormatterTypes = null,
            string serverName = ".",
            bool isTransactional = true,
            bool useJournalQueue = true,
            bool isRecoverable = true
            )
        {
            _isTransactional = isTransactional;
            _useJournalQueue = useJournalQueue;
            _isRecoverable = isRecoverable;
            RequestQueuePath = serverName + "\\" + (PRIVATEQUEUE + requestQueueName).ToLower();          
            RequestQueueName = requestQueueName;           
            _requestMessageFormatterTypes = requestMessageFormatterTypes;                                 
            _requestMessageQueue = GetMessageQueue(RequestQueueName, RequestQueuePath, _requestMessageFormatterTypes);
            if (!string.IsNullOrEmpty(responseQueueName) && responseMessageFormatterTypes != null)
            {
                ResponseQueuePath = serverName + "\\" + (PRIVATEQUEUE + responseQueueName).ToLower();
                ResponseQueueName = responseQueueName;
                _responseMessageFormatterTypes = responseMessageFormatterTypes;
                _responseMessageQueue = GetMessageQueue(ResponseQueueName, ResponseQueuePath, _responseMessageFormatterTypes);
            }           
        }

        public bool IsHandlerRunning()
        {
            if (_handler == null) return false;

            return _handler.Status == TaskStatus.Running ? true : false;
        }

        public void StartHandler(Func<object, object> callbackFunction)
        {
            if (IsHandlerRunning()) return;

            if (_handlerTokenSource.IsCancellationRequested)
                _handlerTokenSource = new CancellationTokenSource();

            _handler = Task.Run(() =>
            {
                Logger.Info(Logger.LoggerType.Nlog, RequestQueueName + " Handler Started");
                while (!_handlerTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        using (var cursor = _requestMessageQueue.CreateCursor())
                        {
                            var message = _requestMessageQueue.Peek(TimeSpan.FromSeconds(.1), cursor, PeekAction.Current);
                            if (ProcessRequest(message, callbackFunction)) _requestMessageQueue.ReceiveById(message.Id);

                            while (true)
                            {
                                message = _requestMessageQueue.Peek(
                                    TimeSpan.FromSeconds(.1),
                                    cursor,
                                    PeekAction.Next
                                    );

                                if (ProcessRequest(message, callbackFunction)) _requestMessageQueue.ReceiveById(message.Id);
                            }
                        }
                    }
                    catch(InvalidOperationException)
                    {

                    }
                    catch
                    {
                        _handlerTokenSource.Cancel();
                    }
                }
                Logger.Info(Logger.LoggerType.Nlog, RequestQueueName + " Handler Stopped");
            }, _handlerTokenSource.Token);
        }

        public void StopHandler()
        {
            if (!IsHandlerRunning()) return;

            _handlerTokenSource.Cancel();
        }

        public string Send(object messageBody, TimeSpan timeout)
        { 
            var message = new Message(messageBody);            
            if (_responseMessageQueue != null)
                message.ResponseQueue = _responseMessageQueue;
            message.UseJournalQueue = _useJournalQueue;
            message.Recoverable = _isRecoverable;
            message.TimeToBeReceived = timeout;

            Send(_requestMessageQueue, message);

            return message.Id;
        }
        
        public object ReceiveByCorrelationId(string correlationId)
        {
            object messageBody = null;

            while (messageBody == null)
            {
                var message = new Message();
                try
                {
                    if (_isTransactional)
                    {
                        using (var txn = new MessageQueueTransaction())
                        {
                            txn.Begin();

                            message = _responseMessageQueue.ReceiveByCorrelationId(correlationId, txn);
                            messageBody = message.Body;

                            txn.Commit();
                        }
                    }
                    else
                    {
                        message = _responseMessageQueue.ReceiveByCorrelationId(correlationId);
                        messageBody = message.Body;
                    }
                }
                catch (InvalidOperationException)
                {
                }
                catch
                {
                    throw;
                }
            }

            return messageBody;
        }

        private MessageQueue GetMessageQueue(string queueName, string queuePath, Type[] messageFormatterTypes)
        {
            
            MessageQueue messageQueue = null;

            if (!MessageQueue.Exists(queuePath.ToLower()))
            {
                messageQueue = MessageQueue.Create(queuePath, _isTransactional);
                messageQueue.Label = queueName;              
            }
            else
            {
                messageQueue = new MessageQueue(queuePath);
            }

            messageQueue.MessageReadPropertyFilter = new MessagePropertyFilter() {
                AppSpecific = true,
                Body = true,
                Priority = true,
                ResponseQueue = true,
                Id = true,
                CorrelationId = true,
                UseJournalQueue = true
            };
            messageQueue.Formatter = new XmlMessageFormatter(messageFormatterTypes);

            return messageQueue;
        }

        private bool ProcessRequest(Message message, Func<object, object> callbackFunction)
        {
            if (message == null) return false;

            var callbackResult = callbackFunction(message.Body);

            if (callbackResult != null && message.ResponseQueue != null)
            {
                Send(message.ResponseQueue, new Message(callbackResult)
                {
                    CorrelationId = message.Id,
                    Formatter = new XmlMessageFormatter(_responseMessageFormatterTypes),
                    UseJournalQueue = _useJournalQueue,
                    Recoverable = _isRecoverable,
                    TimeToBeReceived = ResponseTimeout
                });

                return true;
            }

            return false;
        }
        
        private void Send(MessageQueue messageQueue, Message message)
        {
            if (_isTransactional)
            {
                using (var txn = new MessageQueueTransaction())
                {
                    txn.Begin();
                    messageQueue.Send(message, txn);
                    txn.Commit();
                }
            }
            else
            {
                messageQueue.Send(message);
            }
        }
    }
}
