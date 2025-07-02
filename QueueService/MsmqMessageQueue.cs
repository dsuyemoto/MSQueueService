using System;
using System.Messaging;

namespace QueueService
{
    public class MsmqMessageQueue : IMsmqMessageQueue
    {
        const string PRIVATEQUEUE = "private$";
        MessageQueue _messageQueue;

        public string MessageQueueName { get { return _messageQueue.Label; } }
        public IMessageFormatter Formatter { get { return _messageQueue.Formatter; } }
        public string Path { get { return _messageQueue.Path; } }
        public bool IsTransactional { get { return _messageQueue.Transactional; } }
        public bool UseJournalQueue { get { return _messageQueue.UseJournalQueue; } }

        public enum MsmqRights
        {
            FullControl
        }

        public MsmqMessageQueue(MessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public MsmqMessageQueue(
            string name,
            Type[] formatterTypes,
            MsmqQueuePermission[] queuePermissions,
            string serverName = ".",
            bool isSystemQueue = false,
            bool isTransactional = true,
            bool useJournalQueue = true)
        {
            var path = serverName + "\\" + (PRIVATEQUEUE + "\\" + name).ToLower();
            if (!MessageQueue.Exists(path.ToLower()))
            {
                _messageQueue = MessageQueue.Create(path, isTransactional);
                _messageQueue.Label = MessageQueueName;
            }
            else
            {
                _messageQueue = new MessageQueue(path);
            }

            if (!isSystemQueue)
            {
                _messageQueue.MessageReadPropertyFilter = new MessagePropertyFilter()
                {
                    AppSpecific = true,
                    Body = true,
                    Priority = true,
                    ResponseQueue = true,
                    Id = true,
                    CorrelationId = true,
                    UseJournalQueue = useJournalQueue
                };
                _messageQueue.UseJournalQueue = useJournalQueue;
                foreach (var queuePermission in queuePermissions)
                    SetPermissions(queuePermission);
                _messageQueue.Formatter = new XmlMessageFormatter(formatterTypes);
            }
        }

        public MsmqMessageQueue(
            string name,
            IMessageFormatter messageFormatter,
            MsmqQueuePermission[] queuePermissions,
            string serverName = ".", 
            bool isTransactional = true,
            bool useJournalQueue = true)
        {
            var path = serverName + "\\" + (PRIVATEQUEUE + "\\" + name).ToLower();
            if (!MessageQueue.Exists(path.ToLower()))
            {
                _messageQueue = MessageQueue.Create(path, isTransactional);
                _messageQueue.Label = MessageQueueName;
            }
            else
            {
                _messageQueue = new MessageQueue(path);
            }

            _messageQueue.MessageReadPropertyFilter = new MessagePropertyFilter()
            {
                AppSpecific = true,
                Body = true,
                Priority = true,
                ResponseQueue = true,
                Id = true,
                CorrelationId = true,
                UseJournalQueue = useJournalQueue
            };
            _messageQueue.Formatter = messageFormatter;
            _messageQueue.UseJournalQueue = useJournalQueue;
            foreach (var queuePermission in queuePermissions)
                SetPermissions(queuePermission);
        }

        public object PeekByCorrelationId(string correlationId)
        {
            using (var mq = _messageQueue)
            {
                var message = mq.PeekByCorrelationId(correlationId);
                if (message != null) return message.Body;
            }

            return null;
        }

        public object PeekByCorrelationId(string correlationId, TimeSpan timeout)
        {
            using (var mq = _messageQueue)
            {
                var message = mq.PeekByCorrelationId(correlationId, timeout);
                if (message != null) return message.Body;
            }

            return null;
        }

        public Message Receive(TimeSpan timeout)
        {
            using (var mq = _messageQueue)
                return mq.Receive(timeout);
        }

        public object ReceiveByCorrelationId(string correlationId)
        {
            object messageBody = null;

            using (var mq = _messageQueue)
            {
                if (mq.Transactional)
                {
                    using (var txn = new MessageQueueTransaction())
                    {
                        txn.Begin();

                        var message = mq.ReceiveByCorrelationId(correlationId, txn);
                        messageBody = message.Body;

                        txn.Commit();
                    }
                }
                else
                {
                    var message = mq.ReceiveByCorrelationId(correlationId);
                    messageBody = message.Body;
                }
            }

            return messageBody;
        }

        public void Send(MsmqMessage message)
        {
            using (var mq = _messageQueue)
            {
                if (mq.Transactional)
                {
                    using (var txn = new MessageQueueTransaction())
                    {
                        txn.Begin();
                        mq.Send(message.Message, txn);
                        txn.Commit();
                    }
                }
                else
                {
                    mq.Send(message.Message);
                }
            }
        }

        public void SetPermissions(MsmqQueuePermission queuePermission)
        {
            _messageQueue.SetPermissions(
                queuePermission.SecurityName,
                (MessageQueueAccessRights)Enum.Parse(typeof(MessageQueueAccessRights), queuePermission.Rights.ToString())
                );
        }

        public void Refresh()
        {
            _messageQueue.Refresh();
        }

        public MessageQueue GetMessageQueue()
        {
            return _messageQueue;
        }

        public void Purge(TimeSpan expirationLength)
        {
            using (var mq = _messageQueue)
            {
                mq.MessageReadPropertyFilter.ArrivedTime = true;

                using (var messageReader = mq.GetMessageEnumerator2())
                {
                    while (messageReader.MoveNext())
                    {
                        var message = messageReader.Current;
                        if (message.ArrivedTime.AddHours(expirationLength.TotalHours) < DateTime.Now)
                        {
                            mq.ReceiveById(message.Id);
                        }
                    }
                }
            }
        }

        public IAsyncResult BeginReceive()
        {
            return _messageQueue.BeginReceive();
        }

        public MsmqMessage EndReceive(IAsyncResult asyncResult)
        {
            return new MsmqMessage(_messageQueue.EndReceive(asyncResult));
        }

        public void AddReceiveCompleted(ReceiveCompletedEventHandler receiveCompletedEventHandler)
        {
            _messageQueue.ReceiveCompleted += receiveCompletedEventHandler;
        }

        public void RemoveReceiveCompleted(ReceiveCompletedEventHandler receiveCompletedEventHandler)
        {
            _messageQueue.ReceiveCompleted -= receiveCompletedEventHandler;
        }
    }
}
