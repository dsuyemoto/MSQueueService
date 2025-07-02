using Imanage.Documents;
using RestSharp;
using System;

namespace Imanage.Repository
{
    internal class ImanageDocumentRequestGet : ImanageDocumentRequest
    {
        private GetFunction _function;

        public enum GetFunction
        {
            checkout,
            none
        }

        public ImanageDocumentRequestGet(
            string database,
            IRestClient restClient,
            ImanageTokenInfo tokenInfo,
            string number,
            string version,
            GetFunction function = GetFunction.none) : base(database, restClient, tokenInfo)
        {
            _number = number;
            _version = version;
            _function = function;
        }

        protected override IRestClient GetClient()
        {
            var servername = _database.Replace("-", "");
            _restClient.BaseUrl = new Uri("https://" + servername + ".lw.com/api/v2");

            return _restClient;
        }

        protected override IRestRequest GetRequest()
        {
            var url = "/customers/" + _tokenInfo.CustomerId +
                    "/libraries/" + _database +
                    "/documents/" + _database + "!" + _number + "." + _version;
            if (_function != GetFunction.none)
                url = url + "/" + _function.ToString();

            var request = base.GetRequest();
            request.AddHeader("x-auth-token", _tokenInfo.XAuthToken);
            request.Method = Method.GET;
            request.Resource = url;

            return request;
        }
    }
}
