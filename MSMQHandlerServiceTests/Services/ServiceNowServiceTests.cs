using LoggerHelper;
using Moq;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using QueueService;
using QueueServiceWebApp.Controllers;
using QueueServiceWebApp.Models;
using RestSharp;
using ServiceNow;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ServiceNow.ServiceNowBase;

namespace MSMQHandlerService.Tests.Services
{
    [TestFixture()]
    public class ServiceNowServiceTests
    {
        ILogger _mockLogger;
        IServiceNowService _serviceNowService;
        IQueueServiceImanage _mockQueueServiceImanage;
        IQueueServiceServiceNow _mockQueueServiceServiceNow;
        IQueueServiceEws _mockQueueServiceEws;
        CancellationTokenSource _cancellationTokenSource;
        IServiceNowRepository _mockServiceNowRepository;
        IFileService _mockFileService;
        ServiceNowCreateAttachmentQueue _serviceNowCreateAttachmentQueue;
        SnResultTable _snResultTableCreated;
        SnResultTable _snResultTableOk;
        SnResultTable _snResultTableNoContent;
        byte[] _content;
        string _contentBase64;
        string _messageIdBase64;
        string _passwordBase64;
        Dictionary<string, object> _result;

        const string MESSAGEID = "TESTMessageId";
        const string INSTANCEURL = "InstanceURL";
        const string TABLENAME = "TableName";
        const string USERNAME = "Username";
        const string PASSWORD = "Password";
        const string CONTENT = "Content";
        const string FILENAME = "Filename";
        const string SYSIDVALUE = "sysidvalue";
        const string SOURCEFILEPATH = "c:\\temp\\test.pdf";
        const string CONTENTTYPE = "text/plain";
        const string ERRORMESSAGE = "No source content found";
        const string FOLDERID = "FolderId";
        const string FILE_NAME = "file_name";
        const string CONTENT_TYPE = "content_type";
        const string SYS_ID = "sys_id";
        const string APPDOMAIN = "MSMQHandlerServiceTests";

        public ServiceNowServiceTests()
        {
            _result = new Dictionary<string, object>();
            _result.Add(SYS_ID, SYSIDVALUE);
            _result.Add(FILE_NAME, FILENAME);
            _result.Add(CONTENT_TYPE, CONTENTTYPE);
            _messageIdBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(MESSAGEID));
            _passwordBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(PASSWORD));
            _cancellationTokenSource = new CancellationTokenSource();
            _snResultTableCreated = new SnResultTable(
                new RestResponse()
                {
                    Content = JsonConvert.SerializeObject(new SnResponseTableDTO() { Result = _result }),
                    StatusCode = HttpStatusCode.Created
                }, 
                INSTANCEURL,
                TABLENAME);
            _snResultTableOk = new SnResultTable(
                new RestResponse()
                {
                    Content = JsonConvert.SerializeObject(new SnResponseTableDTO() { Result = _result }),
                    StatusCode = HttpStatusCode.OK
                },
                INSTANCEURL,
                TABLENAME);
            _snResultTableNoContent = new SnResultTable(
                new RestResponse()
                {
                    Content = JsonConvert.SerializeObject(new SnResponseTableDTO() { Result = _result }),
                    StatusCode = HttpStatusCode.NoContent
                },
                INSTANCEURL,
                TABLENAME);
        }

        [SetUp()]
        public void SetupTests()
        {
            _mockLogger = Mock.Of<ILogger>();
            _mockServiceNowRepository = Mock.Of<IServiceNowRepository>();
            Mock.Get(_mockServiceNowRepository).Setup(s => s.CreateTicket(It.IsAny<SnTicketCreate>()))
                .Returns(_snResultTableCreated);
            Mock.Get(_mockServiceNowRepository).Setup(s => s.UpdateTicket(It.IsAny<SnTicketUpdate>()))
                .Returns(_snResultTableOk);
            Mock.Get(_mockServiceNowRepository).Setup(s => s.GetTicket(It.IsAny<SnTicketGet>()))
                .Returns(_snResultTableOk);
            Mock.Get(_mockServiceNowRepository).Setup(s => s.CreateAttachment(It.IsAny<SnAttachmentCreate>()))
                .Returns(_snResultTableCreated);
            Mock.Get(_mockServiceNowRepository).Setup(s => s.DeleteAttachment(It.IsAny<SnAttachmentDelete>()))
                .Returns(_snResultTableNoContent);
            Mock.Get(_mockServiceNowRepository).Setup(s => s.GetGroup(It.IsAny<SnGroupGet>()))
                .Returns(_snResultTableOk);
            Mock.Get(_mockServiceNowRepository).Setup(s => s.GetUser(It.IsAny<SnUserGet>()))
                .Returns(_snResultTableOk);
            _content = Encoding.UTF8.GetBytes(CONTENT);
            _contentBase64 = Convert.ToBase64String(_content);
            var imanageNrl = new ImanageDocumentNrlQueue(_contentBase64, FILENAME);
            _serviceNowCreateAttachmentQueue = new ServiceNowCreateAttachmentQueue()
            {
                FileName = FILENAME,
                SourceContent = new ServiceNowSourceContentQueue()
                {
                    BytesBase64 = _contentBase64
                },
                InstanceUrl = INSTANCEURL,
                TableName = TABLENAME,
                Username = USERNAME,
                PasswordBase64 = _passwordBase64,
                TicketMessageIdBase64 = _messageIdBase64,
                MimeType = CONTENTTYPE
            };
            var imanageDocumentQueues = new List<ImanageDocumentQueue>();
            imanageDocumentQueues.Add(new ImanageDocumentQueue() { ImanageNrl = imanageNrl });
            _mockQueueServiceImanage = Mock.Of<IQueueServiceImanage>();
            Mock.Get(_mockQueueServiceImanage).Setup(q => q.GetMessageAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
                .Returns(Task.Run(() =>
                {
                    return (object)new ImanageResultDocumentQueue()
                    {
                        Documents = imanageDocumentQueues.ToArray(),
                        ErrorResult = new ErrorResultQueue() { Message = "" },
                        FolderId = FOLDERID
                    };
                }));
            _mockQueueServiceServiceNow = Mock.Of<IQueueServiceServiceNow>();
            var results = new Dictionary<string, string>();
            results.Add(SnField.sys_id.ToString(), SYSIDVALUE);
            var serviceNowTicketResultQueue = new ServiceNowResultTicketQueue();
            serviceNowTicketResultQueue.Tickets = new ServiceNowTicketQueue[] { 
                new ServiceNowTicketQueue()
                {
                    Fields = ModelHelpers.DictionaryToArray(results)
                }
            };
            Mock.Get(_mockQueueServiceServiceNow).Setup(s => s.GetMessageAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
                .Returns(Task.Run(() => { return (object)serviceNowTicketResultQueue; }));
            _mockFileService = Mock.Of<IFileService>();
            Mock.Get(_mockFileService)
                .Setup(f => f.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => _content));
            _mockQueueServiceEws = Mock.Of<IQueueServiceEws>();
            _serviceNowService = new ServiceNowService(
                _mockServiceNowRepository,
                _mockFileService, 
                _mockQueueServiceImanage,
                _mockQueueServiceServiceNow, 
                _mockLogger);
        }

        [Test()]
        public void CreateTicket_SnTicketOutput_AreEqualTest()
        {
            var serviceNowTicketCreateQueue = new ServiceNowCreateTicketQueue() { PasswordBase64 = _passwordBase64 };

            var serviceNowTicketResultQueue = (ServiceNowResultTicketQueue)_serviceNowService.HandlerCallbackAsync(
                serviceNowTicketCreateQueue, 
                _cancellationTokenSource.Token).Result;

            Assert.AreEqual(SYSIDVALUE, serviceNowTicketResultQueue.Tickets[0].Fields[0][1]);
        }

        [Test()]
        public void UpdateTicket_UpdateTicket_CalledOnceTest()
        {
            var serviceNowTicketUpdateQueue = new ServiceNowUpdateTicketQueue() {
                PasswordBase64 = _passwordBase64,
                MessageIdBase64 = _messageIdBase64
            };

            _serviceNowService.HandlerCallbackAsync(
                serviceNowTicketUpdateQueue,
                _cancellationTokenSource.Token).Wait();

            Mock.Get(_mockServiceNowRepository).Verify(s => s.UpdateTicket(It.IsAny<SnTicketUpdate>()), Times.Once);
        }

        [Test()]
        public void UpdateTicket_SnTicketOutput_AreEqualTest()
        {
            var serviceNowTicketUpdateQueue = new ServiceNowUpdateTicketQueue() {
                PasswordBase64 = _passwordBase64,
                MessageIdBase64 = _messageIdBase64
            };

            var serviceNowTicketResultQueue = (ServiceNowResultTicketQueue)_serviceNowService.HandlerCallbackAsync(
                serviceNowTicketUpdateQueue,
                _cancellationTokenSource.Token).Result;

            Assert.AreEqual(SYSIDVALUE, serviceNowTicketResultQueue.Tickets[0].Fields[0][1]);
        }

        [Test()]
        public void UpdateTicket_MessageIdBase64_IsValidTest()
        {
            string messageId = string.Empty;
            Mock.Get(_mockQueueServiceServiceNow).Setup(e => e.GetMessageAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
                .Callback<string, CancellationToken, bool>((m, t, b) => messageId = m);

            _serviceNowService.HandlerCallbackAsync(
                new ServiceNowUpdateTicketQueue() { MessageIdBase64 = _messageIdBase64 },
                _cancellationTokenSource.Token
                ).Wait();

            Assert.IsNotEmpty(messageId);
        }

        [Test()]
        public void ServiceNowGetTicketQueue_GetTicket_CalledOnceTest()
        {
            var serviceNowTicketGetQueue = new ServiceNowGetTicketsQueue() { PasswordBase64 = _passwordBase64 };

            _serviceNowService.HandlerCallbackAsync(
                serviceNowTicketGetQueue,
                _cancellationTokenSource.Token).Wait();

            Mock.Get(_mockServiceNowRepository).Verify(s => s.GetTicket(It.IsAny<SnTicketGet>()), Times.Once);
        }

        [Test]
        public void CreateAttachmentAsync_ImanageResult_AreEqualTest()
        {
            _serviceNowCreateAttachmentQueue.SourceContent.BytesBase64 = null;
            _serviceNowCreateAttachmentQueue.SourceContent.ImanageMessageIdBase64 = _messageIdBase64;
            var result = _serviceNowService.HandlerCallbackAsync(
                _serviceNowCreateAttachmentQueue,
                _cancellationTokenSource.Token).Result;
            var serviceNowResultAttachmentQueue = (ServiceNowResultAttachmentQueue)result;
            var fields = Helpers.ArrayToDictionary(serviceNowResultAttachmentQueue.Attachments[0].Fields);

            Assert.AreEqual(FILENAME, fields["file_name"]);
            Assert.AreEqual(CONTENTTYPE, fields["content_type"]);
            Assert.AreEqual(SYSIDVALUE, fields["sys_id"]);
            Assert.AreEqual(INSTANCEURL, serviceNowResultAttachmentQueue.InstanceUrl);
            Assert.AreEqual(TABLENAME, serviceNowResultAttachmentQueue.TableName);
            Assert.IsNull(serviceNowResultAttachmentQueue.ErrorResult.Detail);
            Assert.IsNull(serviceNowResultAttachmentQueue.ErrorResult.Message);
            Assert.IsNull(serviceNowResultAttachmentQueue.ErrorResult.Status);
        }

        [Test()]
        public void CreateAttachmentAsync_FileResult_AreEqualTest()
        {
            _serviceNowCreateAttachmentQueue.SourceContent.BytesBase64 = null;
            _serviceNowCreateAttachmentQueue.SourceContent.SourceFilePath = SOURCEFILEPATH;
            var result = _serviceNowService.HandlerCallbackAsync(
                _serviceNowCreateAttachmentQueue,
                _cancellationTokenSource.Token).Result;
            var serviceNowResultAttachmentQueue = (ServiceNowResultAttachmentQueue)result;
            var fields = Helpers.ArrayToDictionary(serviceNowResultAttachmentQueue.Attachments[0].Fields);

            Assert.AreEqual(FILENAME, fields[FILE_NAME]);
            Assert.AreEqual(CONTENTTYPE, fields[CONTENT_TYPE]);
            Assert.AreEqual(SYSIDVALUE, fields[SYS_ID]);
            Assert.AreEqual(INSTANCEURL, serviceNowResultAttachmentQueue.InstanceUrl);
            Assert.AreEqual(TABLENAME, serviceNowResultAttachmentQueue.TableName);
            Assert.IsNull(serviceNowResultAttachmentQueue.ErrorResult.Detail);
            Assert.IsNull(serviceNowResultAttachmentQueue.ErrorResult.Message);
            Assert.IsNull(serviceNowResultAttachmentQueue.ErrorResult.Status);
        }

        [Test()]
        public void CreateAttachmentAsync_ErrorResult_AreEqualTest()
        {
            _serviceNowCreateAttachmentQueue.SourceContent.BytesBase64 = null;

            var result = _serviceNowService.HandlerCallbackAsync(
                _serviceNowCreateAttachmentQueue, 
                _cancellationTokenSource.Token).Result;
            var serviceNowResultAttachmentQueue = (ServiceNowResultAttachmentQueue)result;

            Assert.AreEqual(ERRORMESSAGE, serviceNowResultAttachmentQueue.ErrorResult.Message);
        }

        [Test]
        public void CreateAttachmentAsync_TicketNumberNotFound_AreEqualTest()
        {
            _serviceNowCreateAttachmentQueue.TicketMessageIdBase64 = null;

            var result = _serviceNowService.HandlerCallbackAsync(
                _serviceNowCreateAttachmentQueue,
                _cancellationTokenSource.Token).Result;
            var serviceNowResultAttachmentQueue = (ServiceNowResultAttachmentQueue)result;

            Assert.AreEqual("Ticket number not found", serviceNowResultAttachmentQueue.ErrorResult.Message);
        }

        [Test()]
        public void CreateAttachmentAsync_Content_AreEqualTest()
        {
            var result = _serviceNowService.HandlerCallbackAsync(
                _serviceNowCreateAttachmentQueue,
                _cancellationTokenSource.Token).Result;
            var serviceNowResultAttachmentQueue = (ServiceNowResultAttachmentQueue)result;
            var fields = Helpers.ArrayToDictionary(serviceNowResultAttachmentQueue.Attachments[0].Fields);

            Assert.AreEqual(FILENAME, fields[FILE_NAME]);
            Assert.AreEqual(CONTENTTYPE, fields[CONTENT_TYPE]);
            Assert.AreEqual(SYSIDVALUE, fields[SYS_ID]);
            Assert.AreEqual(INSTANCEURL, serviceNowResultAttachmentQueue.InstanceUrl);
            Assert.AreEqual(TABLENAME, serviceNowResultAttachmentQueue.TableName);
            Assert.IsNull(serviceNowResultAttachmentQueue.ErrorResult.Detail);
            Assert.IsNull(serviceNowResultAttachmentQueue.ErrorResult.Message);
            Assert.IsNull(serviceNowResultAttachmentQueue.ErrorResult.Status);
        }

        [Test()]
        public void CreateAttachmentAsync_DeleteSysId_AreEqualTest()
        {
            _serviceNowService.HandlerCallbackAsync(
                _serviceNowCreateAttachmentQueue,
                _cancellationTokenSource.Token).Wait();

            Mock.Get(_mockServiceNowRepository).Verify(s => s.DeleteAttachment(It.IsAny<SnAttachmentDelete>()), Times.Once);
        }

        [Test()]
        public void GetTickets_ServiceNowResult_AreEqualTest()
        {
            var serviceNowTicketsGetQueue = new ServiceNowGetTicketsQueue() { PasswordBase64 = _passwordBase64 };

            var serviceNowTicketResultQueue = (ServiceNowResultTicketQueue)_serviceNowService.HandlerCallbackAsync(
                serviceNowTicketsGetQueue,
                _cancellationTokenSource.Token).Result;
            var fields = Helpers.ArrayToDictionary(serviceNowTicketResultQueue.Tickets[0].Fields);

            Assert.AreEqual(SYSIDVALUE, fields[SYS_ID]);
        }

        [Test()]
        public void GetGroup_ServiceNowResult_AreEqualTest()
        {
            var serviceNowGroupGetQueue = new ServiceNowGetGroupQueue() { PasswordBase64 = _passwordBase64 };

            var serviceNowGroupResultQueue = (ServiceNowResultGroupQueue)_serviceNowService.HandlerCallbackAsync(
                serviceNowGroupGetQueue,
                _cancellationTokenSource.Token).Result;

            Assert.AreEqual(SYSIDVALUE, serviceNowGroupResultQueue.Groups[0].Fields[0][1]);
        }

        [Test()]
        public void GetUser_ServiceNowResult_AreEqualTest()
        {
            var serviceNowUserGetQueue = new ServiceNowGetUserQueue() { PasswordBase64 = _passwordBase64 };

            var serviceNowUserResultQueue = (ServiceNowResultUserQueue)_serviceNowService.HandlerCallbackAsync(
                serviceNowUserGetQueue,
                _cancellationTokenSource.Token).Result;

            Assert.AreEqual(SYSIDVALUE, serviceNowUserResultQueue.Users[0].Fields[0][1]);
        }
    }
}
