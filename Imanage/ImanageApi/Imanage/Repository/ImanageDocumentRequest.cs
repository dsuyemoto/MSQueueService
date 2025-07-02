using Imanage.Documents;
using RestSharp;

namespace Imanage
{
    public class ImanageDocumentRequest : ImanageRestRequest
    {
        protected ImanageTokenInfo _tokenInfo;
        protected string _number;
        protected string _version;
        protected byte[] _content;
        protected string _folderId;

        public ImanageDocumentRequest(
            string database, 
            IRestClient restClient,
            ImanageTokenInfo tokenInfo) : base(database, restClient)
        {
            _tokenInfo = tokenInfo;
        }
    }
}
