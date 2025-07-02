using LoggerHelper;
using Moq;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using NUnit.Framework;
using PureConnect;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Tests.Services
{
    [TestFixture()]
    public class CicServiceTests
    {
        IQueueServiceServiceNow _mockQueueServiceServiceNow;
        IQueueServiceCic _mockQueueServiceCic;
        IININInteractionsResource _mockIninInteractionsResource;
        CancellationTokenSource _cancellationTokenSource;
        CicInteractionUpdateQueue _cicInteractionUpdate;
        CicInteractionGetQueue _cicInteractionGetQueue;
        CicService _cicService;
        string _messageIdBase64;
        CicCredsQueue _cicCredsQueue;
        ServiceNowResultTicketQueue _serviceNowResultTicketQueue;
        ICacheService _mockCacheService;
        Dictionary<string, string> _attributes;
        ILogger _mockLogger;

        const string MESSAGEID = "TESTMessageId";
        const string INTERACTIONID = "12345";
        const string SOURCENAME = "SourceName";
        const string SOURCEVALUE = "SourceValue";
        const string USERNAME = "Username";
        const string SERVERNAME = "ServerName";
        const string PASSWORDBASE64 = "UGFzc3dvcmQ=";
        const string CICNAME = "cic_name";
        const string ATTRIBUTENAMES = "attributenames";
        const HttpStatusCode STATUSCODECREATED = HttpStatusCode.Created;
        const HttpStatusCode STATUSCODEOK = HttpStatusCode.OK;
        const string ATTRIBUTENAME = "attributename";
        const string ATTRIBUTEVALUE = "attributevalue";

        public CicServiceTests()
        {
            _cicCredsQueue = new CicCredsQueue()
            {
                Servername = SERVERNAME,
                Username = USERNAME,
                PasswordBase64 = PASSWORDBASE64
            };
            _messageIdBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(MESSAGEID));
            _cancellationTokenSource = new CancellationTokenSource();
            var snFields = new string[][] { new string[] { SOURCENAME, SOURCEVALUE } };
            var serviceNowTicketQueue = new ServiceNowTicketQueue() { Fields = snFields };
            _serviceNowResultTicketQueue = new ServiceNowResultTicketQueue() 
            {
                Tickets = new ServiceNowTicketQueue[] { serviceNowTicketQueue } 
            };
            _attributes = new Dictionary<string, string>();
            _attributes.Add(ATTRIBUTENAME, ATTRIBUTEVALUE);
        }

        [SetUp()]
        public void Setup()
        {
            var cicInteractionResultQueue = new CicInteractionResultQueue() { InteractionId = INTERACTIONID };

            _mockLogger = Mock.Of<ILogger>();
            _mockQueueServiceServiceNow = Mock.Of<IQueueServiceServiceNow>();
            Mock.Get(_mockQueueServiceServiceNow)
                .Setup(s => s.GetTicketQueueAsync(It.IsAny<ServiceNowGetTicketsQueue>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => MESSAGEID));
            Mock.Get(_mockQueueServiceServiceNow).Setup(s => s.GetMessageAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
                .Returns(Task.Run(() => (object)_serviceNowResultTicketQueue));
            _mockQueueServiceCic = Mock.Of<IQueueServiceCic>();
            Mock.Get(_mockQueueServiceCic).Setup(c => c.GetMessageAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
                .Returns(Task.Run(() => (object)cicInteractionResultQueue));            
            _mockIninInteractionsResource = Mock.Of<IININInteractionsResource>();
            Mock.Get(_mockIninInteractionsResource).Setup(i => i.UpdateInteractionAttributesAsync(
                It.IsAny<ININUpdateInteractionAttributesRequestParameters>(),
                It.IsAny<ININInteractionAttributesUpdateDataContract>()))
                .Returns(Task.Run(() => {
                    return new ININUpdateInteractionAttributesResponse()
                    {
                        InteractionId = INTERACTIONID,
                        IsSuccessful = true,
                        StatusCode = STATUSCODECREATED
                    }; }));
            Mock.Get(_mockIninInteractionsResource).Setup(i => i.GetInteractionAttributesAsync(
                It.IsAny<ININGetInteractionAttributesRequestParameters>()))
                .Returns(Task.Run(() =>
                {
                    return new ININGetInteractionAttributesResponse()
                    {
                        IsSuccessful = true,
                        StatusCode = STATUSCODEOK,
                        InteractionAttributesDataContract = new ININInteractionAttributesDataContract()
                        {
                            Attributes = _attributes
                        }
                    };
                })) ;
            Mock.Get(_mockIninInteractionsResource).Setup(i => i.Connect(
                It.IsAny<string>(),
                It.IsAny<string>(), 
                It.IsAny<string>()));
            _mockCacheService = Mock.Of<ICacheService>();
            Mock.Get(_mockCacheService)
                .Setup(c => c.GetConnectionAsync(
                    It.IsAny<object>(),
                    It.IsAny<Func<object>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => (object)_mockIninInteractionsResource));
            _cicInteractionUpdate = new CicInteractionUpdateQueue()
            {
                MessageIdBase64 = _messageIdBase64,
                Creds = _cicCredsQueue,
                SourceAttributes = new CicServiceNowSourceQueue() {
                    MessageIdBase64 =_messageIdBase64,
                    CicToSnNameMappings = new string[][] { new string[] { CICNAME, SOURCENAME } }
                }
            };
            _cicInteractionGetQueue = new CicInteractionGetQueue()
            {
                AttributeNames = ATTRIBUTENAMES,
                InteractionId = INTERACTIONID,
                Creds = _cicCredsQueue
            };
            _cicService = new CicService(_mockQueueServiceCic, _mockQueueServiceServiceNow, _mockCacheService, _mockLogger);
        }

        [Test()]
        public void CicInteractionGetQueue_InteractionId_AreEqualTest()
        {
            var cicInteractionResult = (CicInteractionResultQueue)_cicService.HandlerCallbackAsync(
                _cicInteractionGetQueue,
                _cancellationTokenSource.Token
                ).Result;

            Assert.AreEqual(INTERACTIONID, cicInteractionResult.InteractionId);
        }

        [Test()]
        public void CicInteractionGetQueue_IsSuccessful_IsTrueTest()
        {
            var cicInteractionResult = (CicInteractionResultQueue)_cicService.HandlerCallbackAsync(
                _cicInteractionGetQueue,
                _cancellationTokenSource.Token
                ).Result;

            Assert.IsTrue(cicInteractionResult.IsSuccessful);
        }

        [Test()]
        public void CicInteractionGetQueue_Attributes_AreEqualTest()
        {
            var attributes = new string[][] { new string[] { ATTRIBUTENAME, ATTRIBUTEVALUE } };
            _cicInteractionUpdate = new CicInteractionUpdateQueue()
            {
                Creds = _cicCredsQueue,
                Attributes = attributes
            };

            var cicInteractionResult = (CicInteractionResultQueue)_cicService.HandlerCallbackAsync(
                _cicInteractionGetQueue, 
                _cancellationTokenSource.Token
                ).Result;

            Assert.AreEqual(attributes, cicInteractionResult.Attributes);
        }

        [Test()]
        public void CicInteractionUpdateQueue_InteractionId_AreEqualTest()
        {
            var cicInteractionResult = (CicInteractionResultQueue)_cicService.HandlerCallbackAsync(
                _cicInteractionUpdate,
                _cancellationTokenSource.Token
                ).Result;

            Assert.AreEqual(INTERACTIONID, cicInteractionResult.InteractionId);
        }

        [Test]
        public void CicInteractionUpdateQueue_IsSuccessFul_IsTrueTest()
        {
            var result = (CicInteractionResultQueue)_cicService.HandlerCallbackAsync(
                _cicInteractionUpdate,
                _cancellationTokenSource.Token
                ).Result;

            Assert.IsTrue(result.IsSuccessful);
        }

        [Test()]
        public void CicInteractionUpdateQueue_Attribues_AreEqualTest()
        {
            var attributes = new string[][] { new string[] { "AttributeName", "AttributeValue" } };
            _cicInteractionUpdate = new CicInteractionUpdateQueue()
            {
                Creds = _cicCredsQueue,
                Attributes = attributes
            };

            var result = (CicInteractionResultQueue)_cicService.HandlerCallbackAsync(
                _cicInteractionUpdate,
                _cancellationTokenSource.Token
                ).Result;

            Assert.AreEqual(attributes, result.Attributes);
        }

    }
}
