using Moq;
using MSMQHandlerService;
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
    public class ServiceNowControllerTests
    {
        ServiceNowController _serviceNowController;
        IQueueServiceServiceNow _mockQueueServiceServiceNow;
        string _messageIdBase64;
        string _resultMessageIdBase64;
        string[][] _fields;
        ServiceNowGetTicketsQueue _serviceNowGetTicketQueue;
        string _passwordBase64;
        ServiceNowCreateTicketQueue _serviceNowCreateTicketQueue;
        ServiceNowUpdateTicketQueue _serviceNowUpdateTicketQueue;
        ServiceNowQueryTicketQueue _serviceNowQueryTicketQueue;
        ServiceNowCreateAttachmentQueue _serviceNowCreateAttachmentQueue;
        ServiceNowGetUserQueue _serviceNowGetUserQueue;
        ServiceNowGetGroupQueue _serviceNowGetGroupQueue;

        string _sourceContentBase64;
        string _imanageMessageIdBase64;
        string _ticketMessageIdBase64;

        const string MESSAGEID = "MessageId";
        const string RESULTMESSAGEID = "ResultMessageId";
        const string SYSID = "sys_id";
        const string TICKETSYSID = "12345";
        const string URL = "http://www.google.com";
        const string TABLENAME = "Tablename";
        const string PASSWORD = "Password";
        const string USERNAME = "Username";
        const string COMMUNICATIONSFIELDNAME = "CommunicationsFieldName";
        const string FILENAME = "Filename";
        const string SOURCEFILEPATH = "SourceFilePath";
        const string MIMETYPE = "mimetype";
        const string USERSYSID = "UserSysId";
        const string GROUPSYSID = "GroupSysId";
        const string FILE_NAME = "file_name";

        public ServiceNowControllerTests()
        {
            _messageIdBase64 = ControllerHelpers.Base64ConvertTo(MESSAGEID);
            _resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(RESULTMESSAGEID);
            _fields = new string[][] { new string[] { SYSID, TICKETSYSID } };
            _passwordBase64 = ControllerHelpers.Base64ConvertTo(PASSWORD);
            _sourceContentBase64 = Convert.ToBase64String(new byte[1]);
            _imanageMessageIdBase64 = ControllerHelpers.Base64ConvertTo(MESSAGEID);
            _ticketMessageIdBase64 = ControllerHelpers.Base64ConvertTo(MESSAGEID);
        }

        [SetUp]
        public void Setup()
        {
            var serviceNowResultTicket = new ServiceNowResultTicketQueue()
            {
                ErrorResult = new ErrorResultQueue() { Message = "" },
                Tickets = new ServiceNowTicketQueue[] { new ServiceNowTicketQueue() { Fields = _fields } }
            };
            _mockQueueServiceServiceNow = Mock.Of<IQueueServiceServiceNow>();
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.GetTicketQueueAsync(
                    It.IsAny<ServiceNowGetTicketsQueue>(),
                    It.IsAny<CancellationToken>()))
                .Callback((ServiceNowGetTicketsQueue s, CancellationToken t) => _serviceNowGetTicketQueue = s)
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.GetMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .Returns(Task.Run(() => (object)serviceNowResultTicket));
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.CreateTicketQueueAsync(
                    It.IsAny<ServiceNowCreateTicketQueue>(),
                    It.IsAny<CancellationToken>()))
                .Callback((ServiceNowCreateTicketQueue s, CancellationToken t) => _serviceNowCreateTicketQueue = s)
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.UpdateTicketQueueAsync(
                    It.IsAny<ServiceNowUpdateTicketQueue>(),
                    It.IsAny<CancellationToken>()))
                .Callback((ServiceNowUpdateTicketQueue s, CancellationToken t) => _serviceNowUpdateTicketQueue = s)
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.QueryTicketQueueAsync(
                    It.IsAny<ServiceNowQueryTicketQueue>(),
                    It.IsAny<CancellationToken>()))
                .Callback((ServiceNowQueryTicketQueue s, CancellationToken t) => _serviceNowQueryTicketQueue = s)
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.CreateAttachmentQueueAsync(
                    It.IsAny<ServiceNowCreateAttachmentQueue>(),
                    It.IsAny<CancellationToken>()))
                .Callback((ServiceNowCreateAttachmentQueue s, CancellationToken t) => _serviceNowCreateAttachmentQueue = s)
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.GetUserQueueAsync(
                    It.IsAny<ServiceNowGetUserQueue>(),
                    It.IsAny<CancellationToken>()))
                .Callback((ServiceNowGetUserQueue s, CancellationToken t) => _serviceNowGetUserQueue = s)
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.QueryUserQueueAsync(
                    It.IsAny<ServiceNowQueryUserQueue>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.QueryGroupQueueAsync(
                    It.IsAny<ServiceNowQueryGroupQueue>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => RESULTMESSAGEID));
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.GetGroupQueueAsync(
                    It.IsAny<ServiceNowGetGroupQueue>(),
                    It.IsAny<CancellationToken>()))
                .Callback((ServiceNowGetGroupQueue s, CancellationToken t) => _serviceNowGetGroupQueue = s)
                .Returns(Task.Run(() => RESULTMESSAGEID));
            _serviceNowController = new ServiceNowController(_mockQueueServiceServiceNow);
        }

        [Test()]
        public void GetTicketAsync_OkResult_AreEqualTest()
        {
            var actionResult = _serviceNowController.GetTicketAsync(
                _messageIdBase64,
                new CancellationToken()).Result;
            var okResult = actionResult as OkNegotiatedContentResult<ServiceNowResultTicketDTO>;

            Assert.AreEqual(TICKETSYSID, okResult.Content.Tickets[0].Fields[SYSID]);
        }

        [Test()]
        public void GetTicketAsync_CreatedResult_AreEqualTest()
        {
            _serviceNowController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/tickets?sysid=" + TICKETSYSID)
            };
            _serviceNowController.Configuration = new HttpConfiguration();

            var actionResult = _serviceNowController.GetTicketAsync(
                new ServiceNowGetTicketDTO()
                {
                    SysId = TICKETSYSID,
                    InstanceUrl = URL,
                    _MaxRetries = 5,
                    PasswordBase64 = _passwordBase64,
                    _ResultNames = SYSID,
                    TableName = TABLENAME,
                    Username = USERNAME
                },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(URL + "/api/tickets/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
            Assert.AreEqual(TICKETSYSID, _serviceNowGetTicketQueue.Fields[0][1]);
            Assert.AreEqual(URL, _serviceNowGetTicketQueue.InstanceUrl);
            Assert.AreEqual(5, _serviceNowGetTicketQueue._MaxRetries);
            Assert.AreEqual(_passwordBase64, _serviceNowGetTicketQueue.PasswordBase64);
            Assert.AreEqual(SYSID, _serviceNowGetTicketQueue.Fields[0][0]);
            Assert.AreEqual(TABLENAME, _serviceNowGetTicketQueue.TableName);
            Assert.AreEqual(USERNAME, _serviceNowGetTicketQueue.Username);
        }

        [Test()]
        public void PostTicketAsync_CreatedResult_AreEqualTest()
        {
            _serviceNowController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/tickets")
            };
            _serviceNowController.Configuration = new HttpConfiguration();

            var actionResult = _serviceNowController.PostTicketAsync(
                new ServiceNowCreateTicketDTO()
                {
                    Fields = ModelHelpers.ArrayToDictionary(_fields),
                    InstanceUrl = URL,
                    _MaxRetries = 5,
                    PasswordBase64 = _passwordBase64,
                    _ResultNames = SYSID,
                    TableName = TABLENAME,
                    Username = USERNAME
                },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/tickets/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(TICKETSYSID, _serviceNowCreateTicketQueue.Fields[0][1]);
            Assert.AreEqual(URL, _serviceNowCreateTicketQueue.InstanceUrl);
            Assert.AreEqual(5, _serviceNowCreateTicketQueue._MaxRetries);
            Assert.AreEqual(_passwordBase64, _serviceNowCreateTicketQueue.PasswordBase64);
            Assert.AreEqual(SYSID, _serviceNowCreateTicketQueue.Fields[0][0]);
            Assert.AreEqual(TABLENAME, _serviceNowCreateTicketQueue.TableName);
            Assert.AreEqual(USERNAME, _serviceNowCreateTicketQueue.Username);
        }

        [Test()]
        public void PutTicketAsync_CreatedResult_AreEqualTest()
        {
            _serviceNowController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/tickets/" + _messageIdBase64)
            };
            _serviceNowController.Configuration = new HttpConfiguration();

            var actionResult = _serviceNowController.PutTicketAsync(
                _messageIdBase64,
                new ServiceNowUpdateTicketDTO()
                {
                    Fields = ModelHelpers.ArrayToDictionary(_fields),
                    InstanceUrl = URL,
                    _MaxRetries = 5,
                    PasswordBase64 = _passwordBase64,
                    _ResultNames = SYSID,
                    TableName = TABLENAME,
                    Username = USERNAME,
                    _InsertImanageLink = new ServiceNowInsertImanageLinkDTO()
                    {
                        CommunicationsFieldName = COMMUNICATIONSFIELDNAME,
                        EmailProperties = new ImanageEmailPropertiesDTO(),
                        ImanageMessageIdBase64 = _messageIdBase64
                    }
                },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/tickets/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(TICKETSYSID, _serviceNowUpdateTicketQueue.Fields[0][1]);
            Assert.AreEqual(URL, _serviceNowUpdateTicketQueue.InstanceUrl);
            Assert.AreEqual(5, _serviceNowUpdateTicketQueue._MaxRetries);
            Assert.AreEqual(_passwordBase64, _serviceNowUpdateTicketQueue.PasswordBase64);
            Assert.AreEqual(SYSID, _serviceNowUpdateTicketQueue.Fields[0][0]);
            Assert.AreEqual(TABLENAME, _serviceNowUpdateTicketQueue.TableName);
            Assert.AreEqual(USERNAME, _serviceNowUpdateTicketQueue.Username);
        }

        [Test()]
        public void PostQueryTicketAsync_CreatedResult_AreEqualTest()
        {
            _serviceNowController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/tickets/query")
            };
            _serviceNowController.Configuration = new HttpConfiguration();

            var actionResult = _serviceNowController.PostQueryTicketAsync(
                new ServiceNowQueryTicketDTO()
                {
                    Fields = ModelHelpers.ArrayToDictionary(_fields),
                    InstanceUrl = URL,
                    _MaxRetries = 5,
                    PasswordBase64 = _passwordBase64,
                    _ResultNames = SYSID,
                    TableName = TABLENAME,
                    Username = USERNAME
                },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/tickets/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(TICKETSYSID, _serviceNowQueryTicketQueue.Fields[0][1]);
            Assert.AreEqual(URL, _serviceNowQueryTicketQueue.InstanceUrl);
            Assert.AreEqual(5, _serviceNowQueryTicketQueue._MaxRetries);
            Assert.AreEqual(_passwordBase64, _serviceNowQueryTicketQueue.PasswordBase64);
            Assert.AreEqual(SYSID, _serviceNowQueryTicketQueue.Fields[0][0]);
            Assert.AreEqual(TABLENAME, _serviceNowQueryTicketQueue.TableName);
            Assert.AreEqual(USERNAME, _serviceNowQueryTicketQueue.Username);
        }

        [Test()]
        public void PostAttachmentAsync_CreatedResult_AreEqualTest()
        {
            _serviceNowController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/attachments")
            };
            _serviceNowController.Configuration = new HttpConfiguration();

            var actionResult = _serviceNowController.PostAttachmentsAsync(
                new ServiceNowCreateAttachmentDTO()
                {
                    SourceContent = new ServiceNowSourceContentDTO()
                    {
                        _SourceFilePath = SOURCEFILEPATH,
                        _ImanageMessageIdBase64 = _imanageMessageIdBase64,
                        _BytesBase64 = _sourceContentBase64
                    },
                    InstanceUrl = URL,
                    _MaxRetries = 5,
                    PasswordBase64 = _passwordBase64,
                    _ResultNames = SYSID,
                    TableName = TABLENAME,
                    _TicketMessageIdBase64 = _ticketMessageIdBase64,
                    _TicketSysId = TICKETSYSID,
                    Username = USERNAME,
                    FileName = FILENAME,
                    _MimeType = MIMETYPE
                },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/attachments/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(_imanageMessageIdBase64, _serviceNowCreateAttachmentQueue.SourceContent.ImanageMessageIdBase64);
            Assert.AreEqual(URL, _serviceNowCreateAttachmentQueue.InstanceUrl);
            Assert.AreEqual(5, _serviceNowCreateAttachmentQueue._MaxRetries);
            Assert.AreEqual(_passwordBase64, _serviceNowCreateAttachmentQueue.PasswordBase64);
            Assert.AreEqual(TICKETSYSID, _serviceNowCreateAttachmentQueue.TicketSysId);
            Assert.AreEqual(_sourceContentBase64, _serviceNowCreateAttachmentQueue.SourceContent.BytesBase64);
            Assert.AreEqual(FILENAME, _serviceNowCreateAttachmentQueue.FileName);
            Assert.AreEqual(MIMETYPE, _serviceNowCreateAttachmentQueue.MimeType);
            Assert.AreEqual(SOURCEFILEPATH, _serviceNowCreateAttachmentQueue.SourceContent.SourceFilePath);
            Assert.AreEqual(TABLENAME, _serviceNowCreateAttachmentQueue.TableName);
            Assert.AreEqual(_ticketMessageIdBase64, _serviceNowCreateAttachmentQueue.TicketMessageIdBase64);
            Assert.AreEqual(USERNAME, _serviceNowCreateAttachmentQueue.Username);
        }

        [Test()]
        public void GetAttachmentAsync_OkResult_AreEqualTest()
        {
            var fields = new string[][] { new string[] { FILE_NAME,FILENAME } };
            Mock.Get(_mockQueueServiceServiceNow)
               .Setup(s => s.GetMessageAsync(
                   It.IsAny<string>(),
                   It.IsAny<CancellationToken>(),
                   It.IsAny<bool>()))
               .Returns(Task.Run(() => (object)new ServiceNowResultAttachmentQueue()
               {
                   Attachments = new ServiceNowAttachmentQueue[] { 
                       new ServiceNowAttachmentQueue()
                       {
                           Fields = fields
                       } },
                   ErrorResult = new ErrorResultQueue() { Message = "" }
               }));
            _serviceNowController = new ServiceNowController(_mockQueueServiceServiceNow);

            var actionResult = _serviceNowController.GetAttachmentsAsync(
                _messageIdBase64,
                new CancellationToken()).Result;
            var okResult = actionResult as OkNegotiatedContentResult<ServiceNowResultAttachmentDTO>;
            var resultFields = Helpers.ArrayToDictionary(okResult.Content.Attachments[0].Fields);

            Assert.AreEqual(FILENAME, resultFields[FILE_NAME]);
            Assert.IsEmpty(okResult.Content.ErrorResult.Message);
        }

        [Test()]
        public void GetUserAsync_CreatedResult_AreEqualTest()
        {
            _serviceNowController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/users?sysid=" + TICKETSYSID)
            };
            _serviceNowController.Configuration = new HttpConfiguration();

            var actionResult = _serviceNowController.GetUserAsync(
                new ServiceNowGetUserDTO()
                {
                    SysId = USERSYSID,
                    InstanceUrl = URL,
                    _MaxRetries = 5,
                    PasswordBase64 = _passwordBase64,
                    _ResultNames = SYSID,
                    Username = USERNAME
                },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/users/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(SYSID, _serviceNowGetUserQueue.Fields[0][0]);
            Assert.AreEqual(USERSYSID, _serviceNowGetUserQueue.Fields[0][1]);
            Assert.AreEqual(URL, _serviceNowGetUserQueue.InstanceUrl);
            Assert.AreEqual(5, _serviceNowGetUserQueue._MaxRetries);
            Assert.AreEqual(_passwordBase64, _serviceNowGetUserQueue.PasswordBase64);
            Assert.AreEqual(SYSID, _serviceNowGetUserQueue._ResultNames);
            Assert.AreEqual(USERNAME, _serviceNowGetUserQueue.Username);
        }

        [Test()]
        public void GetUserAsync_OkResult_AreEqualTest()
        {
            Mock.Get(_mockQueueServiceServiceNow)
               .Setup(s => s.GetMessageAsync(
                   It.IsAny<string>(),
                   It.IsAny<CancellationToken>(),
                   It.IsAny<bool>()))
               .Returns(Task.Run(() => (object)new ServiceNowResultUserQueue()
               {
                   Users = new ServiceNowUserQueue[] {
                       new ServiceNowUserQueue() { Fields = new string[][]{ new string[] { SYSID, USERSYSID } } }
                   },
                   ErrorResult = new ErrorResultQueue() { Message = "" }
               }));
            _serviceNowController = new ServiceNowController(_mockQueueServiceServiceNow);

            var actionResult = _serviceNowController.GetUserAsync(
                _messageIdBase64,
                new CancellationToken()).Result;
            var okResult = actionResult as OkNegotiatedContentResult<ServiceNowResultUserDTO>;

            Assert.IsTrue(okResult.Content.Users[0].Fields.ContainsKey(SYSID));
            Assert.AreEqual(USERSYSID, okResult.Content.Users[0].Fields[SYSID]);
            Assert.IsEmpty(okResult.Content.ErrorResult.Message);
        }

        [Test()]
        public void PostQueryUser_CreatedResult_AreEqualTest()
        {
            _serviceNowController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/users/query")
            };
            _serviceNowController.Configuration = new HttpConfiguration();

            var actionResult = _serviceNowController.PostQueryUser(
                new ServiceNowQueryUserDTO(), 
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/users/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
        }


        [Test()]
        public void GetGroupAsync_OkResult_AreEqualTest()
        {
            Mock.Get(_mockQueueServiceServiceNow)
               .Setup(s => s.GetMessageAsync(
                   It.IsAny<string>(),
                   It.IsAny<CancellationToken>(),
                   It.IsAny<bool>()))
               .Returns(Task.Run(() => (object)new ServiceNowResultGroupQueue()
               {
                   Groups = new ServiceNowGroupQueue[] {
                       new ServiceNowGroupQueue() { Fields = new string[][]{ new string[] { SYSID, GROUPSYSID } } }
                   },
                   ErrorResult = new ErrorResultQueue() { Message = "" }
               }));
            _serviceNowController = new ServiceNowController(_mockQueueServiceServiceNow);

            var actionResult = _serviceNowController.GetGroupAsync(
                _messageIdBase64,
                new CancellationToken()).Result;
            var okResult = actionResult as OkNegotiatedContentResult<ServiceNowResultGroupDTO>;

            Assert.IsTrue(okResult.Content.Groups[0].Fields.ContainsKey(SYSID));
            Assert.AreEqual(GROUPSYSID, okResult.Content.Groups[0].Fields[SYSID]);
            Assert.IsEmpty(okResult.Content.ErrorResult.Message);
        }

        [Test()]
        public void PostQueryGroup_CreatedResult_AreEqualTest()
        {
            _serviceNowController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/groups/query")
            };
            _serviceNowController.Configuration = new HttpConfiguration();

            var actionResult = _serviceNowController.PostQueryGroup(
                new ServiceNowQueryGroupDTO(),
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/groups/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
        }

        [Test()]
        public void GetGroupAsync_CreatedResult_AreEqualTest()
        {
            _serviceNowController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/groups?sysid=" + TICKETSYSID)
            };
            _serviceNowController.Configuration = new HttpConfiguration();

            var actionResult = _serviceNowController.GetGroupAsync(
                new ServiceNowGetGroupDTO()
                {
                    SysId = GROUPSYSID,
                    InstanceUrl = URL,
                    _MaxRetries = 5,
                    PasswordBase64 = _passwordBase64,
                    _ResultNames = SYSID,
                    Username = USERNAME
                },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/groups/" + _resultMessageIdBase64, createdResult.Location.AbsoluteUri);
            Assert.AreEqual(_resultMessageIdBase64, createdResult.Content.MessageIdBase64);
            Assert.AreEqual(SYSID, _serviceNowGetGroupQueue.Fields[0][0]);
            Assert.AreEqual(GROUPSYSID, _serviceNowGetGroupQueue.Fields[0][1]);
            Assert.AreEqual(URL, _serviceNowGetGroupQueue.InstanceUrl);
            Assert.AreEqual(5, _serviceNowGetGroupQueue._MaxRetries);
            Assert.AreEqual(_passwordBase64, _serviceNowGetGroupQueue.PasswordBase64);
            Assert.AreEqual(SYSID, _serviceNowGetGroupQueue._ResultNames);
            Assert.AreEqual(USERNAME, _serviceNowGetGroupQueue.Username);
        }
    }
}