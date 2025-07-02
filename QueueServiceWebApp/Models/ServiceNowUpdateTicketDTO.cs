using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowUpdateTicketDTO : ServiceNowBaseDTO
    {
        public Dictionary<string, string> Fields { get; set; }
        public ServiceNowInsertImanageLinkDTO _InsertImanageLink { get; set; }
        public string TableName { get; set; }

        public ServiceNowUpdateTicketDTO()
        {
        }
    }
}