using Imanage.Repository;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Net;
using static Imanage.ImanageDocumentRequestPatch;
using static Imanage.ImanageDocumentRequestPost;
using static Imanage.ImanageDocumentRequestPut;

namespace Imanage
{
    public class ImanageRest : IImanageRest
    {
        ImanageTokenInfo _tokenInfo;
        IRestClient _restClient;

        enum ImanageDocumentType
        {
            Document,
            Folder,
            Workspace
        }

        public ImanageRest(NetworkCredential networkCredential, string database)
        {
            Initialize(networkCredential.UserName, networkCredential.Password, database);
        }

        public ImanageRest(ImanageTokenInfo imanageTokenInfo, IRestClient restClient)
        {
            _tokenInfo = imanageTokenInfo;
            _restClient = restClient;
        }

        private void Initialize(string username, string password, string database)
        {
            _restClient = new RestClient();
            _tokenInfo = GetTokenInfo(username, password, database);      
        }

        public ImanageDocumentsOutput CreateDocuments(ImanageCreateDocumentsInput imanageCreateDocumentsInput)
        {
            var imanageDocumentOutputs = new List<ImanageDocumentOutput>();
            var errors = new List<string>();
          
            foreach (var imanageDocumentCreate in imanageCreateDocumentsInput.ImanageDocumentsCreate)
            {
                try
                {
                    var request = new ImanageDocumentRequestPost(
                        imanageDocumentCreate.Database,
                        _restClient,
                        _tokenInfo,
                        imanageDocumentCreate.DocumentProfileItems.GetRequestProfile(),
                        imanageCreateDocumentsInput.ImanageFolderObjectId.FolderId,
                        imanageDocumentCreate.Content,
                        PostFunction.none);

                    var response = request.Execute();

                    imanageDocumentOutputs.Add(
                        new ImanageDocumentOutput(
                            imanageDocumentCreate.Database,
                            imanageDocumentCreate.Database,
                            response));
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            return new ImanageDocumentsOutput(imanageDocumentOutputs.ToArray(), errors.ToArray());
        }

        public ImanageDocumentsOutput UpdateDocuments(ImanageSetDocumentsPropertiesInput imanageSetDocumentsPropertiesInput)
        {
            var imanageDocumentOutputs = new List<ImanageDocumentOutput>();
            var errors = new List<string>();

            foreach (var imanageDocumentSet in imanageSetDocumentsPropertiesInput.ImanageDocumentsSet)
            {
                try
                {
                    var request = new ImanageDocumentRequestPatch(
                        imanageDocumentSet.Database,
                        _restClient,
                        _tokenInfo,
                        imanageDocumentSet.DocumentProfileItems.GetRequestProfile(),
                        imanageDocumentSet.DocumentObjectId.Number,
                        imanageDocumentSet.DocumentObjectId.Version,
                        PatchFunction.none);

                    var response = request.Execute();

                    var imanageDocumentOutput = new ImanageDocumentOutput(
                        imanageDocumentSet.Database,
                        imanageDocumentSet.Database,
                        response);

                    if (imanageDocumentSet.DocumentProfileItems.DeclareAsRecord)
                    {
                        var putRequest = new ImanageDocumentRequestPut(
                            imanageDocumentSet.Database,
                            _restClient,
                            _tokenInfo,
                            null,
                            imanageDocumentSet.DocumentObjectId.Number,
                            imanageDocumentSet.DocumentObjectId.Version,
                            PutFunction.declare);

                        var putResponse = putRequest.Execute();

                        if (putResponse.StatusCode != HttpStatusCode.OK)
                            imanageDocumentOutput = new ImanageDocumentOutput(
                                imanageDocumentSet.Database,
                                imanageDocumentSet.Database,
                                putResponse);
                    }

                    imanageDocumentOutputs.Add(imanageDocumentOutput);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            return new ImanageDocumentsOutput(imanageDocumentOutputs.ToArray(), errors.ToArray());
        }

        public ImanageDocumentsOutput GetDocuments(ImanageGetDocumentsInput imanageGetDocumentsInput)
        {
            var imanageDocumentOutputs = new List<ImanageDocumentOutput>();
            var errors = new List<string>();

            foreach (var imanageDocumentObjectId in imanageGetDocumentsInput.ImanageDocumentObjectIds)
            {
                try
                {
                    var request = new ImanageDocumentRequestGet(
                        imanageDocumentObjectId.Database,
                        _restClient,
                        _tokenInfo,
                        imanageDocumentObjectId.Number,
                        imanageDocumentObjectId.Version);

                    var response = request.Execute();

                    imanageDocumentOutputs.Add(
                        new ImanageDocumentOutput(
                            imanageDocumentObjectId.Database,
                            imanageDocumentObjectId.Database,
                            response));
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            return new ImanageDocumentsOutput(imanageDocumentOutputs.ToArray(), errors.ToArray());
        }

        private ImanageTokenInfo GetTokenInfo(
            string networkusername,
            string networkpassword, 
            string database)
        {
            var request = new ImanageTokenRequest(networkusername, networkpassword, database, _restClient);

            var response = request.Execute();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = new JsonDeserializer();
                var imanageTokenInfo = json.Deserialize<ImanageTokenInfo>(response);

                return imanageTokenInfo;
            }
            else
            {
                throw new Exception("Could not retrieve customer id");
            }
        }
    }
}
