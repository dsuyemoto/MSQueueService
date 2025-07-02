using Moq;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using NUnit.Framework;
using QueueServiceWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace QueueServiceWebApp.Controllers.Tests
{
    [TestFixture()]
    public class ImanageControllerTests
    {
        IQueueServiceImanage _mockQueueServiceImanage;
        ImanageController _imanageController;
        ImanageDocumentQueue _imanageDocumentQueue;
        ImanageResultDocumentQueue _imanageResultDocumentQueue;
        ErrorResultQueue _errorResultQueue;
        string _resultMessageIdBase64;
        string _commentBase64;
        string _contentByteBase64;
        string _descriptionBase64;
        ImanageEmailPropertiesQueue _imanageEmailPropertiesQueue;
        ImanageErrorQueue _imanageErrorQueue;
        ImanageDocumentNrlQueue _imanageDocumentNrlQueue;
        string _imanageNrlContentBytesBase64;
        string _passwordBase64;
        string _messageIdBase64;
        IHttpPostedFileWrapper _mockImanageFile;
        ImanageCreateDocumentQueue _imanageCreateDocumentQueue;

        const string FOLDERID = "FolderId";
        const string MESSAGEID = "MessageId";
        const string RESULTMESSAGEID = "ResultMessageId";
        const string AUTHOR = "Author";
        const string CLASS = "Class";
        const string COMMENT = "Comment";
        const string DATABASE = "Database";
        const string DESCRIPTION = "Description";
        const string CCNAMES = "CcNames";
        const string FROMNAME = "FromName";
        const string RECEIVEDDATE = "ReceivedDate";
        const string SENTDATE = "SentDate";
        const string SUBJECT = "Subject";
        const string TONAMES = "ToNames";
        const string FILENAME = "FileName.txt";
        const string NUMBER = "123456";
        const string OPERATOR = "Operator";
        const string PASSWORD = "Password";
        const string USERNAME = "Username";
        const string SESSION = "Session";
        const string TYPE = "Type";
        const string VERSION = "Version";
        const string URL = "http://www.google.com";
        const string TEMPFILEPATH = "c:\\Temp";
        const string GUID = "GUID";
        const string EXTENSION = "EML";

        public ImanageControllerTests()
        {
            _resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(RESULTMESSAGEID);
            _messageIdBase64 = ControllerHelpers.Base64ConvertTo(MESSAGEID);
            _commentBase64 = ControllerHelpers.Base64ConvertTo(COMMENT);
            _contentByteBase64 = Convert.ToBase64String(new byte[1]);
            _descriptionBase64 = ControllerHelpers.Base64ConvertTo(DESCRIPTION);
            _imanageEmailPropertiesQueue = new ImanageEmailPropertiesQueue()
            {
                CcNames = CCNAMES,
                FromName = FROMNAME,
                ReceivedDate = RECEIVEDDATE,
                SentDate = SENTDATE,
                Subject = SUBJECT,
                ToNames = TONAMES
            };
            _imanageErrorQueue = new ImanageErrorQueue()
            {
                Message = "",
                ProfileErrors = new ImanageProfileErrorQueue[] { }
            };
            _imanageNrlContentBytesBase64 = Convert.ToBase64String(new byte[2]);
            _imanageDocumentNrlQueue = new ImanageDocumentNrlQueue()
            {
                ContentBytesBase64 = _imanageNrlContentBytesBase64,
                FileName = FILENAME
            };
            _passwordBase64 = ControllerHelpers.Base64ConvertTo(PASSWORD);
            _imanageDocumentQueue = new ImanageDocumentQueue()
            {
                Author = AUTHOR,
                CommentBase64 = _commentBase64,
                ContentBytesBase64 = _contentByteBase64,
                Database = DATABASE,
                DescriptionBase64 = _descriptionBase64,
                EmailProperties = _imanageEmailPropertiesQueue,
                ImanageError = _imanageErrorQueue,
                ImanageNrl = _imanageDocumentNrlQueue,
                Number = NUMBER,
                Operator = OPERATOR,
                SecurityPasswordBase64 = _passwordBase64,
                SecurityUsername = USERNAME,
                Session = SESSION,
                Version = VERSION
            };
            var imanageDocumentQueueList = new List<ImanageDocumentQueue>();
            imanageDocumentQueueList.Add(_imanageDocumentQueue);
            _errorResultQueue = new ErrorResultQueue() { Message = "" };
            _imanageResultDocumentQueue = new ImanageResultDocumentQueue()
            {
                Documents = imanageDocumentQueueList.ToArray(),
                ErrorResult = _errorResultQueue,
                FolderId = FOLDERID
            };
        }

        [SetUp]
        public void Setup()
        {
            _mockQueueServiceImanage = Mock.Of<IQueueServiceImanage>();
            Mock.Get(_mockQueueServiceImanage)
                .Setup(i => i.GetDocumentQueueAsync(
                    It.IsAny<ImanageGetDocumentQueue>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceImanage)
                .Setup(i => i.GetMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .Returns(Task.Run(() => (object)_imanageResultDocumentQueue));
            Mock.Get(_mockQueueServiceImanage)
                .Setup(i => i.CreateDocumentQueueAsync(
                    It.IsAny<ImanageCreateDocumentQueue>(),
                    It.IsAny<CancellationToken>()))
                .Callback((ImanageCreateDocumentQueue i, CancellationToken t) => _imanageCreateDocumentQueue = i)
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceImanage)
                .Setup(i => i.UpdateDocumentQueueAsync(
                    It.IsAny<ImanageUpdateDocumentQueue>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => RESULTMESSAGEID));
            _mockImanageFile = Mock.Of<IHttpPostedFileWrapper>();
            Mock.Get(_mockImanageFile).Setup(f => f.SaveAs(It.IsAny<string>()));
            Mock.Get(_mockImanageFile).Setup(f => f.FileName).Returns(FILENAME);
            _imanageController = new ImanageController(
                _mockQueueServiceImanage,
                _mockImanageFile,
                TEMPFILEPATH,
                GUID);
        }

        [Test()]
        public void GetDocumentAsync_OkResult_AreEqualTest()
        {
            var actionResult = _imanageController.GetDocumentAsync(
                _messageIdBase64,
                new CancellationToken()).Result;
            var okResult = actionResult as OkNegotiatedContentResult<ImanageResultDocumentDTO>;

            Assert.AreEqual(AUTHOR, okResult.Content.Documents[0].Author);
            Assert.AreEqual(_commentBase64, okResult.Content.Documents[0].CommentBase64);
            Assert.AreEqual(DATABASE, okResult.Content.Documents[0].Database);
            Assert.AreEqual(_descriptionBase64, okResult.Content.Documents[0].DescriptionBase64);
            Assert.IsEmpty(okResult.Content.Documents[0].ImanageError.Message);
            Assert.IsNotNull(okResult.Content.Documents[0].ImanageError.ProfileErrors);
            Assert.AreEqual(_imanageNrlContentBytesBase64, okResult.Content.Documents[0].ImanageNrl.ContentBytesBase64);
            Assert.AreEqual(FILENAME, okResult.Content.Documents[0].ImanageNrl.FileName);
            Assert.AreEqual(NUMBER, okResult.Content.Documents[0].Number);
            Assert.AreEqual(OPERATOR, okResult.Content.Documents[0].Operator);
            Assert.AreEqual(SESSION, okResult.Content.Documents[0].Session);
            Assert.AreEqual(VERSION, okResult.Content.Documents[0].Version);
        }

        [Test()]
        public void GetDocumentAsync_CreatedResult_AreEqualTest()
        {
            _imanageController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/documents?test=none")
            };
            _imanageController.Configuration = new HttpConfiguration();

            var actionResult = _imanageController.GetDocumentAsync(
                new ImanageGetDocumentDTO(),
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(URL + $"/api/documents/{_resultMessageIdBase64}", createdResult.Location.AbsoluteUri);
        }

        [Test]
        public void PostDocumentAsync_Document_AreEqualTest()
        {
            var contentbytes = new byte[] { 10, 11, 12 };
            StreamContent contentpart = new StreamContent(new MemoryStream(contentbytes));
            contentpart.Headers.Add("Content-Disposition", @"form-data; name=fieldName; filename=filename.txt");
            contentpart.Headers.Add("Content-Type", "text/plain");
            var content = new MultipartContent("form-data");
            content.Add(contentpart);
            _imanageController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/documents"),
                Content = content
            };
            _imanageController.Configuration = new HttpConfiguration();

            var actionResult = _imanageController.PostDocumentAsync(
                new ImanageCreateDocumentDTO() { Document = new ImanageDocumentCreateDTO() { _Extension = EXTENSION } },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual($"{TEMPFILEPATH}\\{GUID}_{FILENAME}", _imanageCreateDocumentQueue.SourceFilePath);
            Assert.AreEqual(ControllerHelpers.Base64ConvertTo(FILENAME), _imanageCreateDocumentQueue.Document.DescriptionBase64);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(URL + $"/api/documents/{_resultMessageIdBase64}", createdResult.Location.AbsoluteUri);
            Assert.AreEqual(EXTENSION, _imanageCreateDocumentQueue.Document.Extension);
        }

        [Test()]
        public void PutDocumentAsyncTest()
        {
            _imanageController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + $"/api/documents/{_messageIdBase64}")
            };
            _imanageController.Configuration = new HttpConfiguration();

            var actionResult = _imanageController.PutDocumentAsync(
                _messageIdBase64,
                new ImanageUpdateDocumentDTO() { Document = new ImanageDocumentUpdateDTO() },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + $"/api/documents/{_resultMessageIdBase64}", createdResult.Location.AbsoluteUri);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
        }
    }
}