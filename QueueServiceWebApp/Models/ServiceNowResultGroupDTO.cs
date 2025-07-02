using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowResultGroupDTO
    {
        public List<ServiceNowGroupDTO> Groups { get; set; }
        public ErrorResultDTO ErrorResult { get; set; }

        public ServiceNowResultGroupDTO()
        {

        }
    }
}