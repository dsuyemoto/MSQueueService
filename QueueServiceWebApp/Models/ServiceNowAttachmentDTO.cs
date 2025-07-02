using MSMQHandlerService.Models;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowAttachmentDTO
    {
        public object[][] Fields { get; set; }

        public ServiceNowAttachmentDTO()
        {

        }

        public ServiceNowAttachmentDTO(ServiceNowAttachmentQueue serviceNowAttachmentQueue)
        {
            Fields = serviceNowAttachmentQueue.Fields;
        }
    }
}