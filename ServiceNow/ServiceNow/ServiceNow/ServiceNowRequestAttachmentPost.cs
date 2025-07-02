using RestSharp;
using System;
using System.Collections.Generic;
using static ServiceNow.ServiceNowBase;

namespace ServiceNow
{
    internal class ServiceNowRequestAttachmentPost : ServiceNowRequest
    {
        public ServiceNowRequestAttachmentPost(
            string tableName,
            string sysId,
            byte[] content,
            string fileName,
            string mimeType,
            Dictionary<string, string> snFields,
            IRestClient restClient,
            string instanceUrl) : base(restClient, instanceUrl, tableName)
        {
            if (content == null) throw new Exception("File content not provided");

            _contentBytes = content;
            _fileName = fileName;
            _mimeType = mimeType;
            _parameters = snFields;
            _parameters.Add(SnField.table_sys_id.ToString(), sysId);
            _parameters.Add(SnField.table_name.ToString(), tableName);
            _method = Method.POST;
            _requestUrl = $"/{ATTACHMENT}/upload";
        }

        public override SnResultBase GetResponse(IRestResponse response)
        {
            return new SnResultTable(response, _instanceUrl, _tableName);
        }
    }
}
