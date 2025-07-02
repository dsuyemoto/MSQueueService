using Imanage.Documents;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;

namespace Imanage
{
    internal class ImanageDocumentRequestPatch : ImanageDocumentRequest
    {
        PatchFunction _function;
        ImanageDocumentProfilePatch _imanageDocumentProfile;

        public enum PatchFunction
        {
            none
        }

        public ImanageDocumentRequestPatch(            
            string database,
            IRestClient restClient,            
            ImanageTokenInfo tokenInfo,
            ImanageDocumentProfilePatch imanageDocumentProfile,
            string number,
            string version,
            PatchFunction function = PatchFunction.none) 
            : base(database, restClient, tokenInfo)
        {
            _imanageDocumentProfile = imanageDocumentProfile;
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
            if (_function != PatchFunction.none)
                url = url + "/" + _function.ToString();

            var request = base.GetRequest();
            request.AddHeader("x-auth-token", _tokenInfo.XAuthToken);
            request.Method = Method.PATCH;
            request.Resource = url;
            request.AddJsonBody(_imanageDocumentProfile);

            return RestRequestExtensions.UseNewtonsoftJson(request);
        }
    }
}