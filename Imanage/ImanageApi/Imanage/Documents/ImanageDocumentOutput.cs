using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Imanage
{
    public class ImanageDocumentOutput : ImanageDocument, IImanageDocumentOutput
    {
        public DocumentProfileItemsOutput DocumentProfileItems { get; set; }
        public ImanageError ImanageError { get; private set; }
        public string FileName { get; private set; }
        public ImanageDocumentNrl Nrl => GetNrl();

        public ImanageDocumentOutput(
            DocumentProfileItemsOutput documentProfileItems, 
            ImanageDocumentObjectId imanageDocumentObjectId,
            ImanageError imanageError)
        {
            DocumentProfileItems = documentProfileItems;
            DocumentObjectId = imanageDocumentObjectId;
            Database = imanageDocumentObjectId.Database;
            FileName = documentProfileItems.Description;
            ImanageError = imanageError;
        }

        public ImanageDocumentOutput(
            string database,
            string session,
            IRestResponse response)
        {
            Database = database;

            var data = GetData(response);

            if (data != null)
            {
                DocumentObjectId = new ImanageDocumentObjectId(
                    session,
                    database,
                    data.Number,
                    data.Version);
                FileName = data.Name;
                DocumentProfileItems = new DocumentProfileItemsOutput(data);
            }
        }

        private ImanageDocumentNrl GetNrl() 
        {
            return ImanageHelpers.CreateNrlLink(DocumentObjectId, FileName);
        }

        private DocumentResponseSingleData GetData(IRestResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                if (response.Content != null)
                {
                    var responseDocument = JsonConvert.DeserializeObject<DocumentResponseSingle>(response.Content);

                    if (responseDocument.warnings != null && responseDocument.warnings.Length > 0)
                    {
                        var imanageProfileErrors = new List<ImanageProfileError>();
                        foreach (var warning in responseDocument.warnings)
                            imanageProfileErrors.Add(
                                new ImanageProfileError() {
                                    AttributeId = warning.field, 
                                    Message = warning.error });

                        ImanageError = new ImanageError() { ImanageProfileErrors = imanageProfileErrors.ToArray() };
                    }

                    return responseDocument.data;
                }
            }
            else if (
                response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.NotFound)
            {
                ImanageError = new ImanageError() { Message = response.Content };

                return null;
            }

            ImanageError = new ImanageError() { Message = response.ErrorMessage };

            return null;
        }
    }
}
