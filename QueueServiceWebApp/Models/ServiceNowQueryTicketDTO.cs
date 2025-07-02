using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowQueryTicketDTO : ServiceNowBaseDTO
    {
        public Dictionary<string, string> Fields { get; set; }
        public string TableName { get; set; }

        public ServiceNowQueryTicketDTO()
        {

        }
    }
}