using ServiceNow;
using System.Collections.Generic;

namespace MSMQHandlerService.Models
{
    public class ServiceNowResultGroupQueue
    {
        public ServiceNowGroupQueue[] Groups { get; set; }
        public ErrorResultQueue ErrorResult { get; set; }

        public ServiceNowResultGroupQueue()
        {

        }

        public ServiceNowResultGroupQueue(SnResultBase snResultBase)
        {
            if (snResultBase == null) return;

            var groups = new List<ServiceNowGroupQueue>();
            var fieldsList = new List<Dictionary<string, object>>();

            if (snResultBase is SnResultsTable) 
                fieldsList = ((SnResultsTable)snResultBase).SnFieldsList;
            else
                fieldsList.Add(((SnResultTable)snResultBase).SnFields);

            foreach (var fields in fieldsList)
                groups.Add(new ServiceNowGroupQueue() { Fields = Helpers.DictionaryToArray(fields) });

            Groups = groups.ToArray();
            ErrorResult = new ErrorResultQueue(snResultBase.Error);
        }
    }
}
