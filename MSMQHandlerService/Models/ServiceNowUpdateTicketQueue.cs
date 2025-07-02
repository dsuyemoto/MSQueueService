namespace MSMQHandlerService.Models
{
    public class ServiceNowUpdateTicketQueue : ServiceNowBase
    {
        public string MessageIdBase64 { get; set; }
        public string[][] Fields { get; set; }
        public ServiceNowInsertImanageLinkQueue InsertImanageLink { get; set; }
        public string TableName { get; set; }

        public ServiceNowUpdateTicketQueue()
        {

        }
    }
}