namespace MSMQHandlerService.Models
{
    public class ServiceNowCreateTicketQueue : ServiceNowBase
    {
        public string[][] Fields { get; set; }
        public string TableName { get; set; }

        public ServiceNowCreateTicketQueue()
        {

        }
    }
}