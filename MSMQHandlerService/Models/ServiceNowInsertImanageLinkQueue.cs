namespace MSMQHandlerService.Models
{
    public class ServiceNowInsertImanageLinkQueue
    {
        public string CommunicationsFieldName { get; set; }
        public string ImanageMessageIdBase64 { get; set; }
        public ImanageEmailPropertiesQueue EmailProperties { get; set; }

        public ServiceNowInsertImanageLinkQueue()
        {

        }
    }
}