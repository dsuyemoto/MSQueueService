namespace MSMQHandlerService.Models
{
    public class ServiceNowSourceContentQueue
    {
        public string SourceFilePath { get; set; }
        public string ImanageMessageIdBase64 { get; set; }
        public string EwsMessageIdBase64 { get; set; }
        public EwsCredsQueue EwsCreds { get; set; }
        public string BytesBase64 { get; set; }

        public ServiceNowSourceContentQueue()
        {

        }
    }
}