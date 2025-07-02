using RestSharp;
using System.Collections.Generic;

namespace ServiceNow
{
    internal class ServiceNowRequestUserGet : ServiceNowRequest
    {
        string _sysId;

        public ServiceNowRequestUserGet(
            Dictionary<string, string> snFields,
            string[] resultNames,
            IRestClient restClient,
            string instanceUrl,
            string tableName,
            string sysId = "") : base(restClient, instanceUrl, tableName)
        {
            _sysId = sysId;
            var sysIdUrl = string.Empty;
            if (!string.IsNullOrEmpty(sysId))
                sysIdUrl = $"/{sysId}";
            _requestUrl = $"/table/{SYSUSER}{sysIdUrl}";
            _method = Method.GET;
            _parameters = snFields;
            _parameters.Add(SYSPARMEXCLUDEREFERENCELINK, "true");
            _parameters.Add(SYSPARMLIMIT, "10");
            _resultNames = resultNames;
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
