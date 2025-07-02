using MSMQHandlerService.Models;
using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowResultAttachmentDTO
    {
        public ServiceNowAttachmentDTO[] Attachments { get; set; }
        public ErrorResultDTO ErrorResult { get; set; }
        public string TableName { get; set; }
        public string InstanceUrl { get; set; }

        public ServiceNowResultAttachmentDTO()
        {

        }

        public ServiceNowResultAttachmentDTO(ServiceNowResultAttachmentQueue serviceNowResultAttachmentQueue)
        {
            if (serviceNowResultAttachmentQueue == null) return;

            var attachments = new List<ServiceNowAttachmentDTO>();

            if (serviceNowResultAttachmentQueue.Attachments != null)
                foreach (var attachment in serviceNowResultAttachmentQueue.Attachments)
                    attachments.Add(new ServiceNowAttachmentDTO(attachment));

            Attachments = attachments.ToArray();

            if (serviceNowResultAttachmentQueue.ErrorResult != null)
                ErrorResult = new ErrorResultDTO(serviceNowResultAttachmentQueue.ErrorResult);

            TableName = serviceNowResultAttachmentQueue.TableName;
            InstanceUrl = serviceNowResultAttachmentQueue.InstanceUrl;
        }
    }
}