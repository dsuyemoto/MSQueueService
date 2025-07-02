namespace QueueServiceWebApp.Models
{
    public class ServiceNowSourceContentDTO
    {
        public string _SourceFilePath { get; set; }
        public string _ImanageMessageIdBase64 { get; set; }
        public string _EwsMessageIdBase64 { get; set; }
        public EwsCredsDTO _EwsCreds { get; set; }
        public string _BytesBase64 { get; set; }

        public ServiceNowSourceContentDTO()
        {

        }
    }
}