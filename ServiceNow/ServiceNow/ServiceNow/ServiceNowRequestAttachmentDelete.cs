using RestSharp;

namespace ServiceNow
{
    internal class ServiceNowRequestAttachmentDelete : ServiceNowRequest
    {
        public ServiceNowRequestAttachmentDelete(
            string sysId,
            IRestClient restClient,
            string instanceUrl,
            string tableName) : base(restClient, instanceUrl, tableName)
        {
            _requestUrl = $"/{ATTACHMENT}/{sysId}";
            _method = Method.DELETE;
        }

        public override SnResultBase GetResponse(IRestResponse response)
        {
            return new SnResultTable(response, _instanceUrl, _tableName);
        }
    }
}
