using Moq;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using NUnit.Framework;
using QueueServiceWebApp.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace QueueServiceWebApp.Controllers.Tests
{
    [TestFixture()]
    public class EwsControllerTests
    {
        IQueueServiceEws _mockQueueServiceEws;
        EwsController _ewsController;
        string _messageIdBase64;
        ErrorResultQueue _errorResultQueue;
        EwsEmailQueue[] _ewsEmails;
        EwsEmailQueue _ewsEmail;
        byte[] _content;
        EwsFolderQueue _ewsFolder;
        string _resultMessageIdBase64;

        const string MESSAGEID = "messageid";
        const string RESULTMESSAGEID = "resultmessageid";
        const string URL = "http://www.google.com";
        const string CCNAMES = "CcNames";
        const string TONAMES = "ToNames";
        const string FROM = "From";
        const string RECEIVEDDATE = "ReceivedDate";
        const string SENTDATE = "SentDate";
        const string SUBJECT = "Subject";
        const string UNIQUEID = "12345";
        const string FOLDERNAME = "FolderName";
        const string PARENTFOLDERUNIQUEID = "ParentFolderUniqueId";

        public EwsControllerTests()
        {
            _content = new byte[1];
            _ewsFolder = new EwsFolderQueue() 
            {
                Name = FOLDERNAME,
                ParentFolderUniqueId = PARENTFOLDERUNIQUEID,
                UniqueId = UNIQUEID
            };
            _messageIdBase64 = ControllerHelpers.Base64ConvertTo(MESSAGEID);
            _resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(RESULTMESSAGEID);
            _errorResultQueue = new ErrorResultQueue() { Message = "" };
            _ewsEmail = new EwsEmailQueue()
            {
                CcNames = CCNAMES,
                EwsFolder = _ewsFolder,
                FromName = FROM,
                ReceivedDate = RECEIVEDDATE,
                SentDate = SENTDATE,
                Subject = SUBJECT,
                ToNames = TONAMES,
                UniqueId = UNIQUEID
            };

            _ewsEmails = new EwsEmailQueue[] { _ewsEmail };
        }

        [SetUp]
        public void Setup()
        {
            _mockQueueServiceEws = Mock.Of<IQueueServiceEws>();
            Mock.Get(_mockQueueServiceEws)
                .Setup(e => e.GetEmailQueueAsync(
                    It.IsAny<EwsGetEmailQueue>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceEws)
                .Setup(e => e.GetMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .Returns(Task.Run(() => (object)new EwsResultEmailQueue()
                {
                    Emails = _ewsEmails,
                    ErrorResult = _errorResultQueue
                }));
            Mock.Get(_mockQueueServiceEws)
                .Setup(e => e.GetFolderQueueAsync(It.IsAny<EwsGetFolderQueue>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceEws)
                .Setup(e => e.GetEmailsQueueAsync(It.IsAny<EwsGetEmailsQueue>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceEws)
                .Setup(e => e.DeleteFolderQueueAsync(
                    It.IsAny<EwsDeleteFolderQueue>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => RESULTMESSAGEID));
            _ewsController = new EwsController(_mockQueueServiceEws);
        }

        [Test()]
        public void GetEmailAsync_OkResult_AreEqualTest()
        {
            var actionResult = _ewsController.GetEmailAsync(_messageIdBase64, new CancellationToken()).Result;
            var okResult = actionResult as OkNegotiatedContentResult<EwsResultEmailDTO>;

            Assert.AreEqual(CCNAMES, okResult.Content.Emails[0].CcNames);
            Assert.AreEqual(FOLDERNAME, okResult.Content.Emails[0].EwsFolder.Name);
            Assert.AreEqual(PARENTFOLDERUNIQUEID, okResult.Content.Emails[0].EwsFolder.ParentFolderUniqueId);
            Assert.AreEqual(UNIQUEID, okResult.Content.Emails[0].EwsFolder.UniqueId);
            Assert.AreEqual(FROM, okResult.Content.Emails[0].FromName);
            Assert.AreEqual(RECEIVEDDATE, okResult.Content.Emails[0].ReceivedDate);
            Assert.AreEqual(SENTDATE, okResult.Content.Emails[0].SentDate);
            Assert.AreEqual(SUBJECT, okResult.Content.Emails[0].Subject);
            Assert.AreEqual(TONAMES, okResult.Content.Emails[0].ToNames);
            Assert.AreEqual(UNIQUEID, okResult.Content.Emails[0].UniqueId);
            Assert.IsEmpty(okResult.Content.ErrorResult.Message);
        }

        [Test()]
        public void GetFolderAsync_OkResult_AreEqualTest()
        {
            Mock.Get(_mockQueueServiceEws).Setup(e => e.GetMessageAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
                .Returns(Task.Run(() => (object)new EwsResultFolderQueue()
                {
                    Folder = _ewsFolder,
                    ErrorResult = _errorResultQueue
                }));
            _ewsController = new EwsController(_mockQueueServiceEws);

            var actionResult = _ewsController.GetFolderAsync(_messageIdBase64, new CancellationToken()).Result;
            var okResult = actionResult as OkNegotiatedContentResult<EwsResultFolderDTO>;

            Assert.AreEqual(FOLDERNAME, okResult.Content.Folder.Name);
            Assert.AreEqual(PARENTFOLDERUNIQUEID, okResult.Content.Folder.ParentFolderUniqueId);
            Assert.AreEqual(UNIQUEID, okResult.Content.Folder.UniqueId);
            Assert.IsEmpty(okResult.Content.ErrorResult.Message);
        }

        [Test()]
        public void GetFolderAsync_CreatedResult_AreEqualTest()
        {
            _ewsController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/folders?test=none")
            };
            _ewsController.Configuration = new HttpConfiguration();
            var actionResult = _ewsController.GetFolderAsync(
                new EwsGetFolderDTO(),
                new EwsCredsDTO(),
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(URL + $"/api/folders/{_resultMessageIdBase64}", createdResult.Location.AbsoluteUri);
        }

        [Test()]
        public void GetEmailsAsync_CreatedResult_AreEqualTest()
        {
            _ewsController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + $"/api/folders/{_messageIdBase64}/emails?test=none")
            };
            _ewsController.Configuration = new HttpConfiguration();

            var actionResult = _ewsController.GetEmailsAsync(
                _messageIdBase64,
                new EwsCredsDTO(),
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(URL + "/api/emails/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
        }

        [Test()]
        public void DeleteFolderAsync_CreatedResult_AreEqualTest()
        {
            _ewsController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + $"/api/folders/{_messageIdBase64}")
            };
            _ewsController.Configuration = new HttpConfiguration();

            var actionResult = _ewsController.DeleteFolderAsync(
                _messageIdBase64,
                new EwsCredsDTO(), 
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(URL + "/api/folders/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
        }
    }
}