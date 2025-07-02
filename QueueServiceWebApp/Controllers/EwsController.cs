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
    [RoutePrefix("api/ews")]
    public class EwsController : ApiController
    {
        IQueueServiceEws _queueServiceEws;
        ILogger _logger;

        public EwsController(IQueueServiceEws queueServiceEws)
        {
            _queueServiceEws = queueServiceEws;
            _logger = LoggerFactory.GetLogger(new DummyLogConfiguration("", ""));
        }

        public EwsController()
        {
            _logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    Properties.Settings.Default.LogLevel,
                    "QueueServiceWebApp",
                    new NlogTargetConfigurationBase[] {
                        new NlogFileTargetConfiguration(
                            Properties.Settings.Default.LogFilePath,
                            Properties.Settings.Default.LogFileSizeMB,
                            "EwsController") }
                    ));

            _queueServiceEws = new QueueServiceEws(QueueType.MSMQ, true, _logger);
        }

        [Route("emails/{messageidbase64}")]
        public async Task<IHttpActionResult> GetEmailAsync(string messageidbase64, CancellationToken token)
        {
            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Debug("messageId", messageId);
            if (messageId == null) return BadRequest();

            try
            {
                var result = await _queueServiceEws.GetMessageAsync(messageId, token);
                if (result == null) return NotFound();
                var ewsResultEmailQueue = (EwsResultEmailQueue)result;

                return Ok(new EwsResultEmailDTO(ewsResultEmailQueue));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("folders/{messageidbase64}")]
        public async Task<IHttpActionResult> GetFolderAsync(string messageidbase64, CancellationToken token)
        {
            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Debug("messageId", messageId);
            if (messageId == null) return BadRequest();

            try
            {
                var result = await _queueServiceEws.GetMessageAsync(messageId, token);
                if (result == null) return NotFound();

                return Ok(new EwsResultFolderDTO((EwsResultFolderQueue)result));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("folders")]
        public async Task<IHttpActionResult> GetFolderAsync(
            [FromUri]EwsGetFolderDTO ewsFolderGetDTO, 
            [FromUri]EwsCredsDTO ewsCredsDTO,
            CancellationToken token)
        {
            if (ewsFolderGetDTO == null || ewsCredsDTO == null) return BadRequest();

            ewsFolderGetDTO.Creds = ewsCredsDTO;
            _logger.Info(ewsFolderGetDTO);

            var messageId = string.Empty;
            try
            {
                messageId = await _queueServiceEws.GetFolderQueueAsync(new EwsGetFolderQueue()
                {
                    FolderPath = ewsFolderGetDTO.FolderPath,
                    Creds = new EwsCredsQueue()
                    {
                        AutodiscoverEmailAddress = ewsFolderGetDTO.Creds.AutodiscoverEmailAddress,
                        PasswordBase64 = ewsFolderGetDTO.Creds.PasswordBase64,
                        Username = ewsFolderGetDTO.Creds.Username
                    },
                    MailboxEmailAddress = ewsFolderGetDTO.MailboxEmailAddress,
                    MaxWaitTimeSeconds = ewsFolderGetDTO.MaxWaitTimeSeconds,
                }, token);
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

        [Route("folders/{messageidbase64}/emails")]
        public async Task<IHttpActionResult> GetEmailsAsync(
            string messageidbase64, 
            [FromUri]EwsCredsDTO ewsCredsDTO,
            CancellationToken token)
        {
            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Debug("messageId", messageId);
            if (messageId == null) return BadRequest();

            try
            {
                messageId = await _queueServiceEws.GetEmailsQueueAsync(
                    new EwsGetEmailsQueue() { 
                        FolderMessageIdBase64 = messageidbase64, 
                        Creds = new EwsCredsQueue() { 
                            AutodiscoverEmailAddress = ewsCredsDTO.AutodiscoverEmailAddress,
                            PasswordBase64 = ewsCredsDTO.PasswordBase64,
                            Username = ewsCredsDTO.Username
                        } },
                    token
                    );
                _logger.Info("messageId", messageId);

                var responseMessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);

                return Created(
                    new Uri(
                        Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.Query, "")
                        .Replace($"folders/{messageidbase64}/", "") +
                        "/" + responseMessageIdBase64),
                    new QueueMessage() { MessageIdBase64 = responseMessageIdBase64 }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("folders/{messageidbase64}")]
        public async Task<IHttpActionResult> DeleteFolderAsync(
            string messageidbase64, 
            [FromUri]EwsCredsDTO ewsCredsDTO,
            CancellationToken token)
        {
            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Debug("messageId", messageId);
            if (messageId == null || ewsCredsDTO == null) return BadRequest();

            try
            {
                messageId = await _queueServiceEws.DeleteFolderQueueAsync(
                    new EwsDeleteFolderQueue()
                    {
                        MessageIdBase64 = messageidbase64,
                        Creds = new EwsCredsQueue()
                        {
                            AutodiscoverEmailAddress = ewsCredsDTO.AutodiscoverEmailAddress,
                            PasswordBase64 = ewsCredsDTO.PasswordBase64,
                            Username = ewsCredsDTO.Username
                        }
                    },
                    token);

                _logger.Info("DeleteFolderMessageId", messageId);

                var resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);
                return Created(
                    new Uri(Request.RequestUri.AbsoluteUri.Replace(messageidbase64, "") +
                        resultMessageIdBase64),
                    new QueueMessage() { MessageIdBase64 = resultMessageIdBase64 }
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