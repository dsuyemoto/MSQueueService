using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowQueryGroupDTO : ServiceNowBaseDTO
    {
        public Dictionary<string, string> Fields { get; set; }

        public ServiceNowQueryGroupDTO()
        {

        }
    }
}