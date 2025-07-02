using RestSharp;
using System.Collections.Generic;

namespace ServiceNow
{
    internal class ServiceNowRequestTicketPost : ServiceNowRequest
    {
        public ServiceNowRequestTicketPost(
            string tableName, 
            Dictionary<string, string> snFields,
            IRestClient restClient,
            string instanceUrl) : base(restClient, instanceUrl, tableName)
        {
            _requestUrl = $"/table/{tableName}?{SYSPARMEXCLUDEREFERENCELINK}=true";
            _method = Method.POST;
            _body = snFields;
        }

        public override SnResultBase GetResponse(IRestResponse restRespone)
        {
            return new SnResultTable(restRespone, _instanceUrl, _tableName);
        }
    }
}
