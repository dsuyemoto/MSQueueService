using ServiceNow;
using System;
using System.Collections.Generic;

namespace MSMQHandlerService.Models
{
    public class ServiceNowResultAttachmentQueue
    {
        public ServiceNowAttachmentQueue[] Attachments { get; set; }
        public ErrorResultQueue ErrorResult { get; set; }
        public string TableName { get; set; }
        public string InstanceUrl { get; set; }
        public string ContentBase64 { get; set; }

        public ServiceNowResultAttachmentQueue()
        {

        }

        public ServiceNowResultAttachmentQueue(SnResultBase snResultBase)
        {
            if (snResultBase == null) return;

            var attachments = new List<ServiceNowAttachmentQueue>();
            var fieldsList = new List<Dictionary<string, object>>();

            if (snResultBase is SnResultsTable)
                fieldsList = ((SnResultsTable)snResultBase).SnFieldsList;
            else
                fieldsList.Add(((SnResultTable)snResultBase).SnFields);

            foreach (var fields in fieldsList)
                attachments.Add(new ServiceNowAttachmentQueue() { Fields = Helpers.DictionaryToArray(fields) });

            Attachments = attachments.ToArray();
            if (snResultBase.Error != null)
                ErrorResult = new ErrorResultQueue(snResultBase.Error);
            InstanceUrl = snResultBase.InstanceUrl;
            TableName = snResultBase.TableName;
        }
    }
}