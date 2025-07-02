using MSMQHandlerService.Models;
using System.Collections.Generic;

namespace QueueServiceWebApp.Models
{
    public class ServiceNowResultTicketDTO
    {
        public List<ServiceNowTicketDTO> Tickets { get; set; } 
        public ErrorResultDTO ErrorResult { get; set; }

        public ServiceNowResultTicketDTO()
        {
        }

        public ServiceNowResultTicketDTO(ServiceNowResultTicketQueue serviceNowResultTicketQueue)
        {
            Tickets = new List<ServiceNowTicketDTO>();
            if (serviceNowResultTicketQueue.Tickets != null)
                foreach (var ticket in serviceNowResultTicketQueue.Tickets)
                    Tickets.Add(new ServiceNowTicketDTO(ticket));
            ErrorResult = new ErrorResultDTO(serviceNowResultTicketQueue.ErrorResult);
        }
    }
}