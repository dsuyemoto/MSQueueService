namespace MSMQHandlerService.Models
{
    public class ServiceNowQueryTicketQueue : ServiceNowBase
    {
        public string[][] Fields { get; set; }
        public string TableName { get; set; }

        public ServiceNowQueryTicketQueue()
        {

        }
    }
}