namespace QueueServiceWebApp.Models
{
    public class ServiceNowInsertImanageLinkDTO
    {
        public string CommunicationsFieldName { get; set; }
        public string ImanageMessageIdBase64 { get; set; }
        public ImanageEmailPropertiesDTO EmailProperties { get; set; }

        public ServiceNowInsertImanageLinkDTO()
        {

        }
    }
}