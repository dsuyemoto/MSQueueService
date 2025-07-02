using Imanage.Documents;
using Moq;
using NUnit.Framework;
using RestSharp;
using RestSharp.Serialization.Json;
using System.Net;

namespace Imanage.Tests.Repository
{
    [TestFixture()]
    public class ImanageRestTests
    {
        IRestClient _mockRestClient;
        IRestResponse _mockRestResponse;
        ImanageTokenInfo _imanageTokenInfo;
        IImanageRest _imanageRest;
        ImanageSetDocumentsPropertiesInput _imanageSetDocumentsPropertiesInput;
        DocumentProfileItemsSet _documentProfileItemsSet;

        const string TOKEN = "Token";
        const string CUSTOMERID = "CustomerId";
        const string DATABASE = "US-DOCS";
        const string SESSION = "US-DOCS";
        const string FOLDERID = "9445726";
        const string NUMBER = "114818989";
        const string VERSION = "version";
        const string DESCRIPTION = "Test Update";

        [SetUp()]
        public void Setup()
        {
            _mockRestClient = Mock.Of<IRestClient>();
            _imanageTokenInfo = new ImanageTokenInfo() { XAuthToken = TOKEN, CustomerId = CUSTOMERID };
            var serializer = new JsonSerializer();
            var json = serializer.Serialize(
                new DocumentResponseSingle() { 
                    data = new DocumentResponseSingleData() { Number = NUMBER, Version = VERSION } }
                );
            _mockRestResponse = Mock.Of<IRestResponse>();
            Mock.Get(_mockRestResponse).Setup(r => r.StatusCode).Returns(HttpStatusCode.OK);
            Mock.Get(_mockRestResponse).Setup(r => r.Content).Returns(json);
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<RestRequest>())).Returns(_mockRestResponse);
            _documentProfileItemsSet = new DocumentProfileItemsSet() 
            {
                Description = DESCRIPTION
            };
            GetImanageSetDocumentsPropertiesInput();
            _imanageRest = new ImanageRest(_imanageTokenInfo, _mockRestClient);
        }

        [Test()]
        public void UpdateDocuments_Execute_CalledOnceTest()
        {
            var imanageOutput = _imanageRest.UpdateDocuments(_imanageSetDocumentsPropertiesInput);

            Mock.Get(_mockRestClient).Verify(r => r.Execute(It.IsAny<RestRequest>()), Times.Once);
        }

        [Test()]
        public void UpdateDocuments_DeclareAsRecordTrue_ExecuteCalledTwiceTest()
        {
            _documentProfileItemsSet = new DocumentProfileItemsSet() 
            {
                Description = DESCRIPTION,
                DeclareAsRecord = true 
            };
            GetImanageSetDocumentsPropertiesInput();

            _imanageRest.UpdateDocuments(_imanageSetDocumentsPropertiesInput);

            Mock.Get(_mockRestClient).Verify(c => c.Execute(It.IsAny<RestRequest>()), Times.Exactly(2));
        }

        [Test()]
        public void UpdateDocuments_DeclareAsRecordFalse_ExecuteCalledOnceTest()
        {
            _documentProfileItemsSet = new DocumentProfileItemsSet()
            {
                Description = DESCRIPTION,
                DeclareAsRecord = false
            };
            GetImanageSetDocumentsPropertiesInput();

            _imanageRest.UpdateDocuments(_imanageSetDocumentsPropertiesInput);

            Mock.Get(_mockRestClient).Verify(c => c.Execute(It.IsAny<RestRequest>()), Times.Once);
        }

        [Test()]
        public void UpdateDocuments_Error_AreEqualTest()
        {
            var response = new RestResponse() { 
                StatusCode = HttpStatusCode.BadRequest,
                Content = "Test Message"
            };
            Mock.Get(_mockRestClient).Setup(c => c.Execute(It.IsAny<IRestRequest>())).Returns(response);            
            
            _imanageRest = new ImanageRest(_imanageTokenInfo, _mockRestClient);
            var result = _imanageRest.UpdateDocuments(_imanageSetDocumentsPropertiesInput);

            Assert.AreEqual("Test Message", result.Documents[0].ImanageError.Message);
        }

        private void GetImanageSetDocumentsPropertiesInput()
        {
            _imanageSetDocumentsPropertiesInput = new ImanageSetDocumentsPropertiesInput(
                    new ImanageFolderObjectId(SESSION, DATABASE, FOLDERID),
                    new ImanageDocumentSet[]
                    {
                        new ImanageDocumentSet(
                            null,
                            _documentProfileItemsSet,
                            new ImanageSecurityObject(
                                new ImanageAclItem[]
                                {
                                    new ImanageAclItem(
                                        "CIC",
                                        ImanageAclItem.ImanageAccessRight.ALL,
                                        ImanageAclItem.ImanageAclItemType.USER
                                        )
                                },
                                ImanageSecurityObject.SecurityType.PRIVATE
                                ),
                            new ImanageDocumentObjectId(SESSION, DATABASE, NUMBER, VERSION))
                    },
                    new DocumentProfileItems.ProfileAttributeId[] { DocumentProfileItems.ProfileAttributeId.Description },
                    new ImanageInput.OutputMaskName[]
                    {
                        ImanageInput.OutputMaskName.Profile,
                        ImanageInput.OutputMaskName.Security
                    });
        }
    }
}
