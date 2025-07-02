using RestSharp;

namespace ServiceNow
{
    internal class ServiceNowRequestTicketDelete : ServiceNowRequest
    {
        public ServiceNowRequestTicketDelete(
            string tableName,
            string sysId,
            IRestClient restClient,
            string instanceUrl) : base(restClient, instanceUrl, tableName)
        {
            _tableName = tableName;
            _requestUrl = $"/table/{tableName}/{sysId}";
            _method = Method.DELETE;
        }

        public override SnResultBase GetResponse(IRestResponse response)
        {
            return new SnResultTable(response, _instanceUrl, _tableName);
        }
    }
}
