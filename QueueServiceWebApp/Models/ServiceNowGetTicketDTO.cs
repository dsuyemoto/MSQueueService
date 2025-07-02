using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowGetTicketDTO : ServiceNowBaseDTO
    {
        public string TableName { get; set; }
        public string SysId { get; set; }

        public ServiceNowGetTicketDTO()
        {

        }
    }
}