using RestSharp;
using System.Collections.Generic;

namespace ServiceNow
{
    internal class ServiceNowRequestTicketGet : ServiceNowRequest
    {
        string _sysId;

        public ServiceNowRequestTicketGet(
            string tableName,
            Dictionary<string, string> snFields,
            string[] resultNames,
            IRestClient restClient,
            string instanceUrl,
            string sysId = "") : base(restClient, instanceUrl, tableName)
        {
            _sysId = sysId;
            var sysIdUrl = string.Empty;
            if (!string.IsNullOrEmpty(sysId))
                sysIdUrl = $"/{sysId}";
            _requestUrl = $"/table/{tableName}" + sysIdUrl;
            _method = Method.GET;
            _parameters = snFields;
            _tableName = tableName;
            _resultNames = resultNames;
            _parameters.Add(SYSPARMEXCLUDEREFERENCELINK, "true");
        }

        public override SnResultBase GetResponse(IRestResponse response)
        {
            if (string.IsNullOrEmpty(_sysId))
                return new SnResultsTable(response, _instanceUrl, _tableName);
            else
                return new SnResultTable(response, _instanceUrl, _tableName);
        }
    }
}
