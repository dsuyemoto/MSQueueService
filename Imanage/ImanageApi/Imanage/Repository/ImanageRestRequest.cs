using RestSharp;
using System.Net;

namespace Imanage
{
    public class ImanageRestRequest
    {
        protected string _database;
        protected IRestClient _restClient;

        public ImanageRestRequest(string database, IRestClient restClient)
        {
            _database = database;
            _restClient = restClient;
        }

        public IRestResponse Execute()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return GetClient().Execute(GetRequest());
        }

        protected virtual IRestClient GetClient()
        {
            return _restClient;
        }

        protected virtual IRestRequest GetRequest()
        {
            var request = new RestRequest();

            return request;
        }
    }
}
