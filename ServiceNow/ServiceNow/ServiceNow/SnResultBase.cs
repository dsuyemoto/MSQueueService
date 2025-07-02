using RestSharp;

namespace ServiceNow
{
    public abstract class SnResultBase
    {
        public bool IsSuccessful { get; set; }
        public SnErrorResult Error { get; set; }
        public string StatusCode { get; set; }
        public string InstanceUrl { get; set; }
        public string TableName { get; set; }
        public string Status { get; set; }

        public SnResultBase(IRestResponse restResponse, string instanceUrl, string tableName)
        {
            GetSnFields(restResponse);
            if (restResponse.ErrorException != null)
                Error = new SnErrorResult(restResponse.ErrorException);
            StatusCode = restResponse.StatusCode.ToString();
            IsSuccessful = restResponse.IsSuccessful;
            InstanceUrl = instanceUrl;
            TableName = tableName;
        }

        protected abstract void GetSnFields(IRestResponse restResponse);
    }
}
