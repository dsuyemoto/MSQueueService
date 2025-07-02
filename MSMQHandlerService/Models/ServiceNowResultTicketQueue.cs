using ServiceNow;
using System.Collections.Generic;

namespace MSMQHandlerService.Models
{
    public class ServiceNowResultTicketQueue
    {
        public ServiceNowTicketQueue[] Tickets { get; set; }
        public ErrorResultQueue ErrorResult { get; set; }

        public ServiceNowResultTicketQueue()
        {

        }

        public ServiceNowResultTicketQueue(SnResultBase snResultBase)
        {
            if (snResultBase == null) return;

            var tickets = new List<ServiceNowTicketQueue>();
            var fieldsList = new List<Dictionary<string, object>>();

            if (snResultBase is SnResultsTable)
                fieldsList = ((SnResultsTable)snResultBase).SnFieldsList;
            else
                fieldsList.Add(((SnResultTable)snResultBase).SnFields);

            foreach (var fields in fieldsList)
                tickets.Add(new ServiceNowTicketQueue() { Fields = Helpers.DictionaryToArray(fields) });

            Tickets = tickets.ToArray();
            ErrorResult = new ErrorResultQueue(snResultBase.Error);
        }
    }
}