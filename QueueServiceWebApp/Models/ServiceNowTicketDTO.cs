using MSMQHandlerService.Models;
using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowTicketDTO
    {
        public Dictionary<string, string> Fields { get; set; }

        public ServiceNowTicketDTO()
        {

        }

        public ServiceNowTicketDTO(ServiceNowTicketQueue serviceNowTicketQueue)
        {
            Fields = ModelHelpers.ArrayToDictionary(serviceNowTicketQueue.Fields);
        }
    }
}