using RestSharp;
using System.Collections.Generic;
using static ServiceNow.ServiceNowRest;

namespace ServiceNow
{
    internal class ServiceNowRequestCommunicationsPost : ServiceNowRequest
    {
        public const string COMMUNICATIONS = "u_communications";

        public ServiceNowRequestCommunicationsPost(
            string sysId,
            string tableName,
            string fieldValue, 
            IRestClient restClient,
            string instanceUrl) : base(restClient, instanceUrl, tableName)
        {
            var body = new Dictionary<string, string>();
            body.Add("element_id", sysId);
            body.Add("element", COMMUNICATIONS);
            body.Add("name", tableName);
            body.Add("value", fieldValue);
            _body = body;
            _requestUrl = $"/{RestAction.table}/{RestAction.sys_journal_field}?{SYSPARMEXCLUDEREFERENCELINK}=true";
            _method = Method.POST;
        }

        public override SnResultBase GetResponse(IRestResponse response)
        {
            return new SnResultTable(response, _instanceUrl, _tableName);
        }
    }
}
