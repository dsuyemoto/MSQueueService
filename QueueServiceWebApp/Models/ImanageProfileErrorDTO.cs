using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class ImanageProfileErrorDTO
    {
        public string AttributeId { get; set; }
        public string Message { get; set; }

        public ImanageProfileErrorDTO()
        {

        }

        public ImanageProfileErrorDTO(ImanageProfileErrorQueue imanageProfileErrorQueue)
        {
            AttributeId = imanageProfileErrorQueue.AttributeId;
            Message = imanageProfileErrorQueue.Message;
        }
    }
}