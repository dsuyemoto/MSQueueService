using ServiceNow;
using System.Collections.Generic;

namespace MSMQHandlerService.Models
{
    public class ServiceNowResultUserQueue
    {
        public ServiceNowUserQueue[] Users { get; set; }
        public ErrorResultQueue ErrorResult { get; set; }

        public ServiceNowResultUserQueue()
        {

        }

        public ServiceNowResultUserQueue(SnResultBase snResultBase)
        {
            if (snResultBase == null) return;

            var users = new List<ServiceNowUserQueue>();
            var fieldsList = new List<Dictionary<string, object>>();

            if (snResultBase is SnResultsTable)
                fieldsList = ((SnResultsTable)snResultBase).SnFieldsList;
            else
                fieldsList.Add(((SnResultTable)snResultBase).SnFields);

            foreach (var fields in fieldsList)
                users.Add(new ServiceNowUserQueue() { Fields = Helpers.DictionaryToArray(fields) });

            Users = users.ToArray();
            ErrorResult = new ErrorResultQueue(snResultBase.Error);
        }
    }
}