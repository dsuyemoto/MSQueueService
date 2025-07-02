namespace MSMQHandlerService.Models
{
    public class ServiceNowGetTicketsQueue : ServiceNowBase
    {
        public string[][] Fields { get; set; }
        public string TableName { get; set; }

        public ServiceNowGetTicketsQueue()
        {

        }
    }
}