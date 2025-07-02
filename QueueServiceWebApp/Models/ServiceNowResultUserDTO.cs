using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowResultUserDTO
    {
        public List<ServiceNowUserDTO> Users { get; set; }
        public ErrorResultDTO ErrorResult { get; set; }

        public ServiceNowResultUserDTO()
        {

        }
    }
}