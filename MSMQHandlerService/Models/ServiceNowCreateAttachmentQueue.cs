namespace MSMQHandlerService.Models
{
    public class ServiceNowCreateAttachmentQueue : ServiceNowBase
    {
        public ServiceNowSourceContentQueue SourceContent { get; set; }
        public string TicketMessageIdBase64 { get; set; }
        public string TicketSysId { get; set; }
        public string TableName { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }

        public ServiceNowCreateAttachmentQueue()
        {

        }
    }
}