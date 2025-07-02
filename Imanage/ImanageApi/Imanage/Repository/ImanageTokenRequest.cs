using RestSharp;
using System;

namespace Imanage
{
    public class ImanageTokenRequest : ImanageRestRequest
    {
        string _networkUsername;
        string _networkPassword;

        public ImanageTokenRequest(
            string networkUsername, 
            string networkPassword,
            string database,
            IRestClient restClient) : base (database, restClient)
        {
            _networkUsername = networkUsername;
            _networkPassword = networkPassword;
        }

        protected override IRestClient GetClient()
        {
            var servername = _database.Replace("-", "");
            _restClient.BaseUrl = new Uri("https://" + servername + ".lw.com/api/v1");

            return _restClient;
        }

        protected override IRestRequest GetRequest()
        {
            var request = new RestRequest("/session/network-login", Method.PUT);
            request.AddJsonBody(new { user_id = _networkUsername, password = _networkPassword });

            return request;
        }
    }
}
