using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowQueryUserDTO : ServiceNowBaseDTO
    {
        public Dictionary<string, string> Fields { get; set; }

        public ServiceNowQueryUserDTO()
        {

        }
    }
}