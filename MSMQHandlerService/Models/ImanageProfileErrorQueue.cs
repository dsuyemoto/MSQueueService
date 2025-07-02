using Imanage;

namespace MSMQHandlerService.Models
{
    public class ImanageProfileErrorQueue
    {
        public string AttributeId { get; set; }
        public string Message { get; set; }

        public ImanageProfileErrorQueue()
        {

        }

        public ImanageProfileErrorQueue(ImanageProfileError imanageProfileError)
        {
            AttributeId = imanageProfileError.AttributeId;
            Message = imanageProfileError.Message;
        }
    }
}