using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowCreateTicketDTO : ServiceNowBaseDTO
    {
        public string TableName { get; set; }
        public Dictionary<string, string> Fields { get; set; }

        public ServiceNowCreateTicketDTO()
        {
        }
    }
}