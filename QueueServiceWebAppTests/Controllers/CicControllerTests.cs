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
    public class CicControllerTests
    {
        IQueueServiceCic _queueServiceCic;
        CicController _cicController;
        string _messageIdBase64;       
        string[][] _attributes;
        ErrorResultQueue _errorResultQueue;

        const string MESSAGEID = "messageid";
        const string URL = "http://www.google.com";
        const string INTERACTIONID = "123456";

        public CicControllerTests()
        {
            _messageIdBase64 = ControllerHelpers.Base64ConvertTo(MESSAGEID);
            _attributes = new string[][] { new string[] { "attributename", "attributevalue" } };
            _errorResultQueue = new ErrorResultQueue() { Message = "" };
        }

        [SetUp]
        public void Setup()
        {
            _queueServiceCic = Mock.Of<IQueueServiceCic>();
            Mock.Get(_queueServiceCic)
                .Setup(c => c.GetInteractionQueueAsync(
                    It.IsAny<CicInteractionGetQueue>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => MESSAGEID));
            Mock.Get(_queueServiceCic)
                .Setup(c => c.GetMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<bool>()))
                .Returns(Task.Run(() => (object)new CicInteractionResultQueue()
                {
                    Attributes = _attributes,
                    ErrorResult = _errorResultQueue,
                    InteractionId = INTERACTIONID,
                    IsSuccessful = true
                }));
            Mock.Get(_queueServiceCic)
                .Setup(c => c.UpdateInteractionQueueAsync(
                    It.IsAny<CicInteractionUpdateQueue>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => MESSAGEID));
            _cicController = new CicController(_queueServiceCic);
        }

        [Test()]
        public void GetInteractionAsync_Ok_AreEqualTest()
        {
            var actionResult = _cicController.GetInteractionAsync(
                _messageIdBase64,
                new CancellationToken()).Result;
            var okResult = actionResult as OkNegotiatedContentResult<CicInteractionResultDTO>;

            Assert.AreEqual(ModelHelpers.ArrayToDictionary(_attributes), okResult.Content.Attributes);
            Assert.AreEqual(INTERACTIONID, okResult.Content.InteractionId);
            Assert.IsTrue(okResult.Content.IsSuccessful);
            Assert.AreEqual(new ErrorResultDTO(_errorResultQueue).Message, okResult.Content.ErrorResult.Message);
        }

        [Test()]
        public void GetInteractionAsync_Location_AreEqualTest()
        {
            _cicController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/interactions?test=none")
            };
            _cicController.Configuration = new HttpConfiguration();

            var actionResult = _cicController.GetInteractionAsync(
                new CicInteractionGetDTO(),
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/interactions/" + _messageIdBase64, createdResult.Location.AbsoluteUri);
        }

        [Test()]
        public void PutInteractionAsyncTest()
        {
            _cicController.Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(URL + "/api/interactions?test=none")
            };
            _cicController.Configuration = new HttpConfiguration();
            var actionResult = _cicController.PutInteractionAsync(
                _messageIdBase64, 
                new CicInteractionUpdateDTO() { ServiceNowSource = new CicServiceNowSourceDTO() },
                new CancellationToken()).Result;
            var createdResult = actionResult as CreatedNegotiatedContentResult<QueueMessage>;

            Assert.AreEqual(URL + "/api/interactions/" + _messageIdBase64, createdResult.Location.AbsoluteUri);
        }
    }
}