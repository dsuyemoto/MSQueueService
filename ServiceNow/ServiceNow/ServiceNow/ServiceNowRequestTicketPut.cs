using RestSharp;
using System;
using System.Collections.Generic;

namespace ServiceNow
{
    internal class ServiceNowRequestTicketPut : ServiceNowRequest
    {
        public ServiceNowRequestTicketPut(
            string tableName, 
            string tableSysId,
            Dictionary<string, string> snFields, 
            IRestClient restClient,
            string instanceUrl) : base (restClient, instanceUrl, tableName)
        {
            _tableName = tableName;
            _body = snFields;
            _method = Method.PUT;
            _requestUrl = $"/table/{tableName}/{tableSysId}?{SYSPARMEXCLUDEREFERENCELINK}=true";
            if (snFields.Count == 0) throw new Exception("Parameters were not provided");
        }

        public override SnResultBase GetResponse(IRestResponse response)
        {
            return new SnResultTable(response, _instanceUrl, _tableName);
        }
    }
}
