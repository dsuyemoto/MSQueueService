namespace MSMQHandlerService.Models
{
    public class ServiceNowResultAttachmentsQueue
    {
        public ServiceNowAttachmentQueue[] Attachments { get; set; }
        public ErrorResultQueue[] ErrorResults { get; set; }
        public string TableName { get; set; }
        public string InstanceUrl { get; set; }

        public ServiceNowResultAttachmentsQueue()
        {

        }
    }
}
