using System.Messaging;

namespace QueueService
{
    public class MsmqMessage
    {
        public IMsmqMessageQueue ResponseQueue {
            get { return new MsmqMessageQueue(Message.ResponseQueue); }
            set { Message.ResponseQueue = value.GetMessageQueue(); }
        }
        public IMessageFormatter Formatter {
            get { return Message.Formatter; }
            set { Message.Formatter = value; }
        }
        public bool Recoverable {
            get { return Message.Recoverable; }
            set { Message.Recoverable = value; }
        }
        public string CorrelationId {
            get { return Message.CorrelationId; }
            set { Message.CorrelationId = value; }
        }
        public string Id => Message.Id;
        public Message Message { get; }

        public string Label {
            get { return Message.Label; }
            set { Message.Label = value; }
        }

        public int AppSpecific {
            get { return Message.AppSpecific; }
            set { Message.AppSpecific = value; }
        }

        public object Body {
            get { return Message.Body; }
            set { Message.Body = value; }
        }

        public MsmqMessage(Message message)
        {
            Message = message;
        }

        public MsmqMessage(object body)
        {
            Message = new Message(body);
        }
    }
}
