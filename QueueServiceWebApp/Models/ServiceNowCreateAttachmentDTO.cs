namespace QueueServiceWebApp.Models
{
    public class ServiceNowCreateAttachmentDTO : ServiceNowBaseDTO
    {
        public ServiceNowSourceContentDTO SourceContent { get; set; }
        public string _TicketMessageIdBase64 { get; set; }
        public string _TicketSysId { get; set; }
        public string TableName { get; set; }
        public string FileName { get; set; }
        public string _MimeType { get; set; }

        public ServiceNowCreateAttachmentDTO()
        {
            
        }
    }
}