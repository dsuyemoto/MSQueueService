using Imanage.Documents;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Text.RegularExpressions;

namespace Imanage
{
    internal class ImanageDocumentRequestPost : ImanageDocumentRequest
    {
        PostFunction _function;
        ImanageDocumentProfilePost _imanageDocumentProfile;

        public enum PostFunction
        {
            none,
            post
        }

        public ImanageDocumentRequestPost(
            string database,
            IRestClient restClient,
            ImanageTokenInfo tokenInfo,
            ImanageDocumentProfilePost imanageDocumentProfile,
            string folderId, 
            byte[] content,
            PostFunction function = PostFunction.none) 
            : base (database, restClient, tokenInfo)
        {
            _imanageDocumentProfile = imanageDocumentProfile;
            _folderId = CheckFolderId(folderId, database);
            _function = function;
            _content = content;
        }

        protected override IRestClient GetClient()
        {
            var servername = _database.Replace("-", "");
            _restClient.BaseUrl = new Uri("https://" + servername + ".lw.com/api/v1");
            return _restClient;
        }

        protected override IRestRequest GetRequest()
        {
            var url = "/folders/" + _folderId + "/documents";

            if (_function != PostFunction.none)
                url = url + "/" + _function.ToString();

            var request = base.GetRequest();
            request.AddHeader("x-auth-token", _tokenInfo.XAuthToken);
            request.Method = Method.POST;
            request.Resource = url;
            request.AddJsonBody(
                new
                {
                    warnings_for_required_and_disabled_fields = true,
                    doc_profile = _imanageDocumentProfile
                });
            request.AddFile(
                "file",
                _content, 
                "DO NOT REMOVE: Placeholder for RestSharp to add as a file",
                "");

            return RestRequestExtensions.UseNewtonsoftJson(request);
        }

        private static string CheckFolderId(string folderId, string database)
        {
            if (string.IsNullOrEmpty(folderId)) throw new Exception("FolderId is empty");

            var regexFolderId = new Regex(@"[A-Za-z-]+!\d+", RegexOptions.IgnoreCase);
            var folderIdMatches = regexFolderId.Match(folderId);
            if (folderIdMatches.Length == 0)
            {
                var regexFolderNumber = new Regex(@"\d+");
                var folderNameMatches = regexFolderNumber.Match(folderId);

                if (folderNameMatches.Length == 0) throw new Exception("FolderId is incorrect");

                folderId = database + "!" + folderId;
            }

            return folderId;
        }
    }
}