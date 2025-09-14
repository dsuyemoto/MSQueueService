using LoggerHelper;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using QueueServiceWebApp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using static QueueService.QueueServiceBase;

namespace QueueServiceWebApp.Controllers
{
    [RoutePrefix("api/cic")]
    public class CicController : ApiController
    {
        IQueueServiceCic _queueServiceCic;
        ILogger _logger;

        public CicController()
        {
            _logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    Properties.Settings.Default.LogLevel,
                    "QueueServiceWebApp",
                    new NlogTargetConfigurationBase[] {
                        new NlogFileTargetConfiguration(
                            Properties.Settings.Default.LogFilePath,
                            Properties.Settings.Default.LogFileSizeMB,
                            "CicController") }
                    ));
            try
            {
                _queueServiceCic = new QueueServiceCic(QueueType.MSMQ, true, _logger);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
            }
        }

        public CicController(IQueueServiceCic queueServiceCic)
        {
            _queueServiceCic = queueServiceCic;
            _logger = LoggerFactory.GetLogger(new DummyLogConfiguration("", ""));
        }

        [Route("interactions/{messageidbase64}")]
        public async Task<IHttpActionResult> GetInteractionAsync(string messageidbase64, CancellationToken token)
        {
            if (!ControllerHelpers.CheckIfBase64(messageidbase64)) return BadRequest();
            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Info("messageId", messageId);

            try
            {
                var result = await _queueServiceCic.GetMessageAsync(
                    ControllerHelpers.Base64ConvertFrom(messageidbase64),
                    token,
                    false);
                if (result == null) return NotFound();

                var cicInteractionResultQueue = (CicInteractionResultQueue)result;
                _logger.Info(cicInteractionResultQueue, messageId);

                return Ok(new CicInteractionResultDTO()
                {
                    IsSuccessful = cicInteractionResultQueue.IsSuccessful,
                    InteractionId = cicInteractionResultQueue.InteractionId,
                    Attributes = ModelHelpers.ArrayToDictionary(cicInteractionResultQueue.Attributes),
                    ErrorResult = new ErrorResultDTO()
                    {
                        Message = cicInteractionResultQueue.ErrorResult.Message
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("interactions")]
        public async Task<IHttpActionResult> GetInteractionAsync(
            [FromUri]CicInteractionGetDTO cicInteractionGetDTO,
            CancellationToken token)
        {
            if (cicInteractionGetDTO == null) return BadRequest();
            _logger.Info(cicInteractionGetDTO);

            var messageId = string.Empty;
            try
            {
                messageId = await _queueServiceCic.GetInteractionQueueAsync(
                        new CicInteractionGetQueue()
                        {
                            AttributeNames = cicInteractionGetDTO.AttributeNames,
                            Creds = new CicCredsQueue()
                            {
                                PasswordBase64 = cicInteractionGetDTO.PasswordBase64,
                                Servername = cicInteractionGetDTO.Servername,
                                Username = cicInteractionGetDTO.Username
                            },
                            InteractionId = cicInteractionGetDTO.InteractionId,
                            MaxRetries = cicInteractionGetDTO.MaxRetries,
                            MaxWaitTimeSeconds = cicInteractionGetDTO.MaxWaitTimeSeconds
                        }, 
                        token
                        );
                _logger.Info("messageId", messageId);

                var messageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);

                return Created(
                    new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.Query, "") + 
                        "/" + messageIdBase64),
                    new QueueMessage() { MessageIdBase64 = messageIdBase64 }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("interactions/{messageidbase64}")]
        public async Task<IHttpActionResult> PutInteractionAsync(
            string messageidbase64, 
            [FromBody]CicInteractionUpdateDTO cicInteractionUpdateDTO,
            CancellationToken token)
        {
            if (cicInteractionUpdateDTO == null || !ControllerHelpers.CheckIfBase64(messageidbase64)) 
                return BadRequest();
            _logger.Info(cicInteractionUpdateDTO);

            var messageId = string.Empty;
            try
            {
                messageId = await _queueServiceCic.UpdateInteractionQueueAsync(
                    new CicInteractionUpdateQueue() { 
                        Attributes = ModelHelpers.DictionaryToArray(cicInteractionUpdateDTO.Attributes),
                        Creds = new CicCredsQueue()
                        {
                            PasswordBase64 = cicInteractionUpdateDTO.PasswordBase64,
                            Servername = cicInteractionUpdateDTO.Servername,
                            Username = cicInteractionUpdateDTO.Username
                        },
                        MaxRetries = cicInteractionUpdateDTO.MaxRetries,
                        MaxWaitTimeSeconds = cicInteractionUpdateDTO.MaxWaitTimeSeconds,
                        MessageIdBase64 = messageidbase64,
                        SourceAttributes = new CicServiceNowSourceQueue()
                        {
                            CicToSnNameMappings = ModelHelpers.DictionaryToArray(cicInteractionUpdateDTO.ServiceNowSource.CicToSnNameMappings),
                            MessageIdBase64 = cicInteractionUpdateDTO.ServiceNowSource.MessageIdBase64
                        }
                    },
                    token);
                _logger.Info("messageId", messageId);

                var messageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);

                return Created(
                    new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.Query, "") +
                        "/" + messageIdBase64),
                    new QueueMessage() { MessageIdBase64 = messageIdBase64 }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }
    }
}
