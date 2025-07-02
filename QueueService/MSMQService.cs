using LoggerHelper;
using System;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService
{
    public class MSMQService : IQueueService
    {
        readonly object _requestLock = new object();
        readonly object _responseLock = new object();
        readonly object _processCallbackLock = new object();
        bool _isRecoverable;
        IMsmqMessageQueue _requestQueue;
        IMsmqMessageQueue _responseQueue;
        IMsmqMessageQueue _failedQueue;
        IMsmqMessageQueue _requestJournalQueue;
        IMsmqMessageQueue _responseJournalQueue;
        IMsmqMessageQueue _failedJournalQueue;
        IMessageFormatter _requestFormatter;
        IMessageFormatter _responseFormatter;
        ILogger _logger;

        const string IISIUSRS = "IIS_IUSRS";
        const string ADMINISTRATORS = "Administrators";
        const string LOCALSERVICE = "Local Service";
        const string JOURNALQUEUE = "\\Journal$";

        public string RequestQueueName { get; }
        public string ResponseQueueName { get; }
        public string FailedQueueName { get; }
        public int RunningTasks { get; set; }

        public MSMQService(
            IMsmqMessageQueue requestQueue, 
            IMsmqMessageQueue responseQueue, 
            IMsmqMessageQueue failedQueue,
            IMsmqMessageQueue requestJournalQueue,
            IMsmqMessageQueue responseJournalQueue,
            IMsmqMessageQueue failedJournalQueue,
            IMessageFormatter requestFormatter,
            IMessageFormatter responseFormatter,
            bool isRecoverable)
        {
            _requestQueue = requestQueue;
            _responseQueue = responseQueue;
            _failedQueue = failedQueue;
            _requestJournalQueue = requestJournalQueue;
            _responseJournalQueue = responseJournalQueue;
            _failedJournalQueue = failedJournalQueue;
            _requestFormatter = requestFormatter;
            _responseFormatter = responseFormatter;
            _isRecoverable = isRecoverable;
        }

        public MSMQService(
            string requestQueueName,
            string responseQueueName, 
            string failedQueueName,
            Type[] requestFormatterTypes,
            Type[] responseFormatterTypes, 
            bool isRecoverable,
            ILogger logger,
            string msmqServerName = ".")
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            RequestQueueName = requestQueueName;
            ResponseQueueName = responseQueueName;
            FailedQueueName = failedQueueName;

            _requestFormatter = new XmlMessageFormatter(requestFormatterTypes);
            _responseFormatter = new XmlMessageFormatter(responseFormatterTypes);
            _isRecoverable = isRecoverable;
            var queuePermissions = new MsmqQueuePermission[]
                {
                    new MsmqQueuePermission(IISIUSRS, MsmqMessageQueue.MsmqRights.FullControl),
                    new MsmqQueuePermission(ADMINISTRATORS, MsmqMessageQueue.MsmqRights.FullControl),
                    new MsmqQueuePermission(LOCALSERVICE, MsmqMessageQueue.MsmqRights.FullControl)
                };
            _requestQueue = new MsmqMessageQueue(
                RequestQueueName, 
                requestFormatterTypes,
                queuePermissions, 
                msmqServerName);
            _responseQueue = new MsmqMessageQueue(
                ResponseQueueName, 
                responseFormatterTypes, 
                queuePermissions, 
                msmqServerName);
            _failedQueue = new MsmqMessageQueue(
                FailedQueueName, 
                requestFormatterTypes,
                queuePermissions, 
                msmqServerName);
            _requestJournalQueue = new MsmqMessageQueue(
                RequestQueueName + JOURNALQUEUE, 
                requestFormatterTypes,
                queuePermissions,
                msmqServerName,
                true);
            _responseJournalQueue = new MsmqMessageQueue(
                ResponseQueueName + JOURNALQUEUE, 
                responseFormatterTypes, 
                queuePermissions, 
                msmqServerName,
                true);
            _failedJournalQueue = new MsmqMessageQueue(
                FailedQueueName + JOURNALQUEUE,
                requestFormatterTypes,
                queuePermissions,
                msmqServerName,
                true);

            _logger.Debug(
                "Initializing requestqueue: " + RequestQueueName +
                " responsequeue: " + ResponseQueueName +
                " failedqueue: " + FailedQueueName +
                " servername: " + msmqServerName);
        }

        public async Task<string> RequestAsync(MsmqMessage messageWrapper, CancellationToken token)
        {
            _logger.Debug(messageWrapper.Body);

            messageWrapper.Formatter = _requestFormatter;
            messageWrapper.Recoverable = _isRecoverable;
            messageWrapper.AppSpecific = 0;
            messageWrapper.ResponseQueue = _responseQueue;

            await Task.Run(() => {
                lock (_requestLock)
                {
                    _requestQueue.Send(messageWrapper);
                }}, token);

            _logger.Info(
                "Sent Message [Queue: " + RequestQueueName +
                " MessageId: " + messageWrapper.Id +
                " CorrelationId: " + messageWrapper.CorrelationId + "]"
                );

            return messageWrapper.Id;
        }

        public object Response(string correlationId, bool deleteMessage)
        {
            _logger.Trace("messageid: " + correlationId + " deletemessage: " + deleteMessage);

            object messageBody = null;

            _logger.Trace("Retrieving Message CorrelationId: " + correlationId + " Queue: " + ResponseQueueName);
            try
            {
                if (deleteMessage)
                {
                    messageBody = _responseQueue.ReceiveByCorrelationId(correlationId);
                }
                else
                {
                    messageBody = _responseQueue.PeekByCorrelationId(correlationId);
                }
            }
            catch (InvalidOperationException ioe)
            {
                _logger.Trace(
                    "InvalidOperationException: " + ioe.Message +
                    " Queue: " + ResponseQueueName +
                    " CorrelationId: " + correlationId
                    );
            }
            catch (MessageQueueException mqe)
            {
                _logger.Trace(
                    "MessageQueueException: " + mqe.MessageQueueErrorCode +
                    " Queue: " + ResponseQueueName +
                    " CorrelationId: " + correlationId
                    );
                if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                    throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            _logger.Debug(messageBody);

            return messageBody;
        }

        public async Task<object> ResponseAsync(string correlationId, bool deleteMessage, CancellationToken token)
        {
            _logger.Debug("messageid: " + correlationId + " deletemessage: " + deleteMessage);

            return await Task.Run(() => {
                lock (_responseLock)
                {
                    object messageBody = null;
                    while (messageBody == null && !token.IsCancellationRequested)
                        messageBody = Response(correlationId, deleteMessage);
                    
                    return messageBody;
                }
            });
        }

        public object WaitOnQueue(TimeSpan receiveTimeout)
        {
            Message message = null;

            try
            {
                _logger.Trace("Waiting on request queue: " + RequestQueueName);
                message = _requestQueue.Receive(receiveTimeout);
                Thread.Sleep(new TimeSpan(0, 0, 10));
            }
            catch (InvalidOperationException io)
            {
                _logger.Trace(io.Message);
            }
            catch (MessageQueueException mqe)
            {
                if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                    _logger.Error(mqe.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            if (message != null)
                _logger.Debug("Retrieved message: " + message.Id + " on messagequeue: " + RequestQueueName);

            return message;
        }

        private async Task ProcessCallbackAsync(
            object message,
            Func<object, CancellationToken, Task<object>> callbackFunction, 
            CancellationToken token, 
            int maxRetries = 5)
        {
            if (message == null) return;

            try
            {
                var queueMessage = (Message)message;

                _logger.Info("Processing message: " + queueMessage.Id + " on messagequeue: " + RequestQueueName);
                var messageBody = queueMessage.Body;
                var callbackResult = await callbackFunction(messageBody, token);

                var correlationId = queueMessage.Id;
                if (queueMessage.CorrelationId != "00000000-0000-0000-0000-000000000000\\0")
                    correlationId = queueMessage.CorrelationId;

                if ((callbackResult != null && queueMessage.ResponseQueue != null))
                {
                    _logger.Debug(callbackResult);

                    lock (_processCallbackLock)
                    {
                        _responseQueue.Send(new MsmqMessage(callbackResult)
                        {
                            Label = correlationId,
                            CorrelationId = correlationId,
                            Formatter = _responseFormatter,
                            Recoverable = _isRecoverable
                        });
                    }
                    _logger.Info("Message processed: " + correlationId + " on messagequeue: " + RequestQueueName);
                }
                else if (queueMessage.AppSpecific > maxRetries)
                {
                    lock (_processCallbackLock)
                    {
                        _failedQueue.Send(new MsmqMessage(messageBody)
                        {
                            Label = correlationId,
                            CorrelationId = correlationId,
                            Formatter = _requestFormatter,
                            Recoverable = _isRecoverable,
                            AppSpecific = 0
                        });
                    }

                    _logger.Info("Message failed: " + correlationId + " on messagequeue: " + RequestQueueName);
                }
                else
                {
                    var appSpecific = queueMessage.AppSpecific;
                    appSpecific++;

                    await RequestAsync(new MsmqMessage(messageBody)
                    {
                        Label = correlationId,
                        CorrelationId = correlationId,
                        Formatter = _requestFormatter,
                        Recoverable = _isRecoverable,
                        AppSpecific = appSpecific
                    }, token);

                    _logger.Info("Message requeued: " + correlationId + " on messagequeue: " + RequestQueueName);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void ProcessQueue(
            Func<object, CancellationToken, Task<object>> callbackFunction, 
            CancellationToken token,
            int maxRetries = 5)
        {
            try
            {
                _logger.Trace("Processing request queue: " + RequestQueueName);
                _requestQueue.AddReceiveCompleted(new ReceiveCompletedEventHandler((s, a) => {
                    try
                    {
                        var mq = (MessageQueue)s;
                        var message = mq.EndReceive(a.AsyncResult);

                        var task = ProcessCallbackAsync(message, callbackFunction, token, maxRetries);   
                    }
                    catch (InvalidOperationException io)
                    {
                        _logger.Trace(io.Message);
                    }
                    catch (MessageQueueException mqe)
                    {
                        if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                            _logger.Error(mqe.Message);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                    finally
                    {
                        if (token.IsCancellationRequested)
                        {
                            _logger.Debug("Process queue " + _requestQueue.MessageQueueName + " handler stopped");
                        }
                        else
                        {
                            _requestQueue.BeginReceive();
                        }
                    }
                }));

                _requestQueue.BeginReceive();
            }
            catch (MessageQueueException mqe)
            {
                _logger.Error(mqe.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void PurgeQueues(TimeSpan expirationLength)
        {
            _requestQueue.Purge(expirationLength);
            _responseQueue.Purge(expirationLength);
            _failedQueue.Purge(expirationLength);
            _requestJournalQueue.Purge(expirationLength);
            _responseJournalQueue.Purge(expirationLength);
            _failedJournalQueue.Purge(expirationLength);
        }

        public async Task<object> GetMessageAsync(string messageId, CancellationToken token, bool deleteMessage = false)
        {
            return await ResponseAsync(messageId, deleteMessage, token);
        }
    }
}