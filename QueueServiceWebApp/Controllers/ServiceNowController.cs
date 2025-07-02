using LoggerHelper;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using QueueServiceWebApp.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using static QueueService.QueueServiceBase;

namespace QueueServiceWebApp.Controllers
{
    [RoutePrefix("api/servicenow")]
    public class ServiceNowController : ApiController
    {
        ILogger _logger;
        IQueueServiceServiceNow _queueServiceServiceNow;

        public ServiceNowController()
        {
            _logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    Properties.Settings.Default.LogLevel,
                    "QueueServiceWebApp",
                    new NlogTargetConfigurationBase[] {
                        new NlogFileTargetConfiguration(
                            Properties.Settings.Default.LogFilePath,
                            Properties.Settings.Default.LogFileSizeMB,
                            "ServiceNowController") }
                    ));

            _queueServiceServiceNow = new QueueServiceServiceNow(QueueType.MSMQ, true, _logger) ;
        }

        public ServiceNowController(IQueueServiceServiceNow queueServiceServiceNow)
        {
            _queueServiceServiceNow = queueServiceServiceNow;
            _logger = LoggerFactory.GetLogger(new DummyLogConfiguration("", ""));
        }

        [Route("tickets/{messageidbase64}")]
        public async Task<IHttpActionResult> GetTicketAsync(string messageidbase64, CancellationToken token)
        {
            if (!ControllerHelpers.CheckIfBase64(messageidbase64)) return BadRequest();

            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Info("messageId", messageId);

            try
            {
                var result = await _queueServiceServiceNow.GetMessageAsync(messageId, token);
                if (result == null) return NotFound();

                var serviceNowResultTicketQueue = (ServiceNowResultTicketQueue)result;
                _logger.Debug(serviceNowResultTicketQueue);

                return Ok(new ServiceNowResultTicketDTO(serviceNowResultTicketQueue));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("tickets")]
        public async Task<IHttpActionResult> GetTicketAsync(
            [FromUri]ServiceNowGetTicketDTO serviceNowGetTicketDTO,
            CancellationToken token)
        {
            if (serviceNowGetTicketDTO == null) return BadRequest();

            _logger.Info(serviceNowGetTicketDTO);

            var messageId = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(serviceNowGetTicketDTO.SysId) ||
                    string.IsNullOrEmpty(serviceNowGetTicketDTO.TableName)) return BadRequest();
                
                messageId = await _queueServiceServiceNow.GetTicketQueueAsync(
                        new ServiceNowGetTicketsQueue()
                        {
                            Fields = new string[][] { new string[] { "sys_id", serviceNowGetTicketDTO.SysId } },
                            TableName = serviceNowGetTicketDTO.TableName,
                            InstanceUrl = serviceNowGetTicketDTO.InstanceUrl,
                            _MaxRetries = serviceNowGetTicketDTO._MaxRetries,
                            PasswordBase64 = serviceNowGetTicketDTO.PasswordBase64,
                            _ResultNames = serviceNowGetTicketDTO._ResultNames,
                            Username = serviceNowGetTicketDTO.Username,
                        },
                        token);
                _logger.Info("messageId", messageId);

                var resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);

                return Created(
                    new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.Query, "") + 
                        "/" + resultMessageIdBase64),
                    new QueueMessage() { MessageIdBase64 = resultMessageIdBase64 }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("tickets")]
        public async Task<IHttpActionResult> PostTicketAsync(
            [FromBody]ServiceNowCreateTicketDTO serviceNowCreateTicketDTO,
            CancellationToken token)
        {
            if (serviceNowCreateTicketDTO == null) return BadRequest();

            _logger.Info(serviceNowCreateTicketDTO);

            var messageId = string.Empty;
            try
            {
                messageId = await _queueServiceServiceNow.CreateTicketQueueAsync(
                        new ServiceNowCreateTicketQueue()
                        {
                            Fields = ModelHelpers.DictionaryToArray(serviceNowCreateTicketDTO.Fields),
                            InstanceUrl = serviceNowCreateTicketDTO.InstanceUrl,
                            _MaxRetries = serviceNowCreateTicketDTO._MaxRetries,
                            PasswordBase64 = serviceNowCreateTicketDTO.PasswordBase64,
                            _ResultNames = serviceNowCreateTicketDTO._ResultNames,
                            TableName = serviceNowCreateTicketDTO.TableName,
                            Username = serviceNowCreateTicketDTO.Username
                        },
                        token);
                _logger.Info("messageId", messageId);

                var resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);

                return Created(
                    new Uri(Request.RequestUri + "/" + resultMessageIdBase64),
                    new QueueMessage() { MessageIdBase64 = resultMessageIdBase64 }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }   
        }

        [Route("tickets/{messageidbase64}")]
        public async Task<IHttpActionResult> PutTicketAsync(
            string messageidbase64, 
            [FromBody]ServiceNowUpdateTicketDTO serviceNowUpdateTicketDTO,
            CancellationToken token)
        {
            if (!ControllerHelpers.CheckIfBase64(messageidbase64)) return BadRequest();

            _logger.Info(serviceNowUpdateTicketDTO);
            if (serviceNowUpdateTicketDTO == null) return BadRequest();

            var messageId = string.Empty;
            try
            {
                ServiceNowInsertImanageLinkQueue insertImanageLink = null;
                var emailProperties = new ImanageEmailPropertiesQueue();

                if (serviceNowUpdateTicketDTO._InsertImanageLink != null) 
                {
                    var imanageLink = serviceNowUpdateTicketDTO._InsertImanageLink;

                    if (serviceNowUpdateTicketDTO._InsertImanageLink.EmailProperties != null)
                    {
                        emailProperties.CcNames = imanageLink.EmailProperties.CcNames;
                        emailProperties.FromName = imanageLink.EmailProperties.FromName;
                        emailProperties.ReceivedDate = imanageLink.EmailProperties.ReceivedDate;
                        emailProperties.SentDate = imanageLink.EmailProperties.SentDate;
                        emailProperties.Subject = imanageLink.EmailProperties.Subject;
                        emailProperties.ToNames = imanageLink.EmailProperties.ToNames;
                    }
                    insertImanageLink = new ServiceNowInsertImanageLinkQueue()
                    {
                        CommunicationsFieldName = imanageLink.CommunicationsFieldName,
                        EmailProperties = emailProperties,
                        ImanageMessageIdBase64 = imanageLink.ImanageMessageIdBase64
                    };
                }                     

                var serviceNowTicketUpdateQueue = new ServiceNowUpdateTicketQueue()
                {
                    Fields = ModelHelpers.DictionaryToArray(serviceNowUpdateTicketDTO.Fields),
                    InsertImanageLink = insertImanageLink,
                    InstanceUrl = serviceNowUpdateTicketDTO.InstanceUrl,
                    _MaxRetries = serviceNowUpdateTicketDTO._MaxRetries,
                    PasswordBase64 = serviceNowUpdateTicketDTO.PasswordBase64,
                    _ResultNames = serviceNowUpdateTicketDTO._ResultNames,
                    TableName = serviceNowUpdateTicketDTO.TableName,
                    Username = serviceNowUpdateTicketDTO.Username
                };
                serviceNowTicketUpdateQueue.MessageIdBase64 = messageidbase64;

                messageId = await _queueServiceServiceNow.UpdateTicketQueueAsync(
                    serviceNowTicketUpdateQueue,
                    token);
                _logger.Info("messageId", messageId);

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

        [Route("tickets/query")]
        public async Task<IHttpActionResult> PostQueryTicketAsync(
            ServiceNowQueryTicketDTO serviceNowQueryTicketDTO,
            CancellationToken token)
        {
            if (serviceNowQueryTicketDTO == null) return BadRequest();

            _logger.Info(serviceNowQueryTicketDTO);

            var messageId = string.Empty;
            try
            {
                messageId = await _queueServiceServiceNow.QueryTicketQueueAsync(
                    new ServiceNowQueryTicketQueue()
                    {
                        Fields = ModelHelpers.DictionaryToArray(serviceNowQueryTicketDTO.Fields),
                        InstanceUrl = serviceNowQueryTicketDTO.InstanceUrl,
                        _MaxRetries = serviceNowQueryTicketDTO._MaxRetries,
                        PasswordBase64 = serviceNowQueryTicketDTO.PasswordBase64,
                        _ResultNames = serviceNowQueryTicketDTO._ResultNames,
                        TableName = serviceNowQueryTicketDTO.TableName,
                        Username = serviceNowQueryTicketDTO.Username,
                    },
                    token);

                _logger.Info("messageId", messageId);

                var resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);

                return Created(
                    new Uri(Request.RequestUri.AbsoluteUri.Replace("query","") + resultMessageIdBase64),
                    new QueueMessage() { MessageIdBase64 = resultMessageIdBase64 }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("attachments/{messageidbase64}")]
        public async Task<IHttpActionResult> GetAttachmentsAsync(string messageidbase64, CancellationToken token)
        {
            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);

            if (messageId == null) return BadRequest();
           
            try
            {
                var result = await _queueServiceServiceNow.GetMessageAsync(messageId, token);
                if (result == null) return NotFound();

                var serviceNowResultAttachmentDTO = 
                    new ServiceNowResultAttachmentDTO((ServiceNowResultAttachmentQueue)result);
                _logger.Info(serviceNowResultAttachmentDTO, messageId);

                return Ok(serviceNowResultAttachmentDTO);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("attachments")]
        public async Task<IHttpActionResult> PostAttachmentsAsync(
            [FromBody]ServiceNowCreateAttachmentDTO serviceNowCreateAttachmentDTO,
            CancellationToken token)
        {
            _logger.Info(serviceNowCreateAttachmentDTO);

            if (serviceNowCreateAttachmentDTO == null) return BadRequest();

            var messageId = string.Empty;
            try
            {
                var sourceContent = new ServiceNowSourceContentQueue();
                if (serviceNowCreateAttachmentDTO.SourceContent != null) 
                {
                    sourceContent.BytesBase64 = serviceNowCreateAttachmentDTO.SourceContent._BytesBase64;
                    sourceContent.ImanageMessageIdBase64 = serviceNowCreateAttachmentDTO.SourceContent._ImanageMessageIdBase64;
                    sourceContent.EwsMessageIdBase64 = serviceNowCreateAttachmentDTO.SourceContent._EwsMessageIdBase64;
                    if (serviceNowCreateAttachmentDTO.SourceContent._EwsCreds != null)
                    {
                        sourceContent.EwsCreds = new EwsCredsQueue()
                        {
                            AutodiscoverEmailAddress = serviceNowCreateAttachmentDTO.SourceContent._EwsCreds.AutodiscoverEmailAddress,
                            Username = serviceNowCreateAttachmentDTO.SourceContent._EwsCreds.Username,
                            PasswordBase64 = serviceNowCreateAttachmentDTO.SourceContent._EwsCreds.PasswordBase64
                        };
                    }
                    sourceContent.SourceFilePath = serviceNowCreateAttachmentDTO.SourceContent._SourceFilePath;
                }
                var serviceNowCreateAttachmentQueue = new ServiceNowCreateAttachmentQueue()
                    {
                        SourceContent = sourceContent,
                        TicketMessageIdBase64 = serviceNowCreateAttachmentDTO._TicketMessageIdBase64,
                        TicketSysId = serviceNowCreateAttachmentDTO._TicketSysId,
                        TableName = serviceNowCreateAttachmentDTO.TableName,
                        FileName = serviceNowCreateAttachmentDTO.FileName,
                        MimeType = serviceNowCreateAttachmentDTO._MimeType,
                        InstanceUrl = serviceNowCreateAttachmentDTO.InstanceUrl,
                        Username = serviceNowCreateAttachmentDTO.Username,
                        PasswordBase64 = serviceNowCreateAttachmentDTO.PasswordBase64,
                        _ResultNames = serviceNowCreateAttachmentDTO._ResultNames,
                        _MaxRetries = serviceNowCreateAttachmentDTO._MaxRetries,
                    };
                _logger.Debug(serviceNowCreateAttachmentQueue);

                messageId = await _queueServiceServiceNow.CreateAttachmentQueueAsync(
                    serviceNowCreateAttachmentQueue,
                    token);
                _logger.Info("messageId", messageId);

                var resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);

                return Created(
                    new Uri(Request.RequestUri + "/" + resultMessageIdBase64),
                    new QueueMessage() { MessageIdBase64 = resultMessageIdBase64 }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("users")]
        public async Task<IHttpActionResult> GetUserAsync(
            [FromUri]ServiceNowGetUserDTO serviceNowGetUserDTO,
            CancellationToken token)
        {
            if (serviceNowGetUserDTO == null ||
                string.IsNullOrEmpty(serviceNowGetUserDTO.SysId))
                return BadRequest();

            _logger.Info(serviceNowGetUserDTO);

            var messageId = string.Empty;
            try
            {
                messageId = await _queueServiceServiceNow.GetUserQueueAsync(
                    new ServiceNowGetUserQueue()
                    {
                        Fields = new string[][] {
                            new string[] {
                                ServiceNowService.SnFields.sys_id.ToString(),
                                serviceNowGetUserDTO.SysId
                            } },
                        _ResultNames = serviceNowGetUserDTO._ResultNames,
                        InstanceUrl = serviceNowGetUserDTO.InstanceUrl,
                        Username = serviceNowGetUserDTO.Username,
                        PasswordBase64 = serviceNowGetUserDTO.PasswordBase64,
                    },
                    token);
                _logger.Info("messageId", messageId);

                var resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);

                return Created(
                    new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.Query, "") +
                        "/" + resultMessageIdBase64),
                    new QueueMessage() { MessageIdBase64 = resultMessageIdBase64 }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("users/{messageidbase64}")]
        public async Task<IHttpActionResult> GetUserAsync(string messageidbase64, CancellationToken token)
        {
            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Debug("messageId", messageId);

            if (messageidbase64 == null) return BadRequest();

            try
            {
                var result = await _queueServiceServiceNow.GetMessageAsync(messageId, token);
                if (result == null) return NotFound();

                var serviceNowResultUserQueue = (ServiceNowResultUserQueue)result;
                _logger.Debug(serviceNowResultUserQueue, messageId);

                ErrorResultDTO errorResult = null;
                if (serviceNowResultUserQueue.ErrorResult != null)
                    errorResult = new ErrorResultDTO() { 
                        Message = serviceNowResultUserQueue.ErrorResult.Message 
                    };

                if (serviceNowResultUserQueue.Users == null) return NotFound();

                var serviceNowUserDTOs = new List<ServiceNowUserDTO>();
                foreach (var user in serviceNowResultUserQueue.Users) {
                    serviceNowUserDTOs.Add(
                        new ServiceNowUserDTO() {
                            Fields = ModelHelpers.ArrayToDictionary(user.Fields)
                        });
                }

                var serviceNowResultUserDTO = new ServiceNowResultUserDTO()
                {
                    ErrorResult = errorResult,
                    Users = serviceNowUserDTOs
                };
                _logger.Info(serviceNowResultUserDTO, messageId);

                return Ok(serviceNowResultUserDTO);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("users/query")]
        public async Task<IHttpActionResult> PostQueryUser(
            ServiceNowQueryUserDTO serviceNowQueryUserDTO,
            CancellationToken token)
        {
            if (serviceNowQueryUserDTO == null) return BadRequest();

            _logger.Info(serviceNowQueryUserDTO);
            var messageId = string.Empty;

            try
            {
                messageId = await _queueServiceServiceNow.QueryUserQueueAsync(
                        new ServiceNowQueryUserQueue()
                        {
                            Fields = ModelHelpers.DictionaryToArray(serviceNowQueryUserDTO.Fields),
                            InstanceUrl = serviceNowQueryUserDTO.InstanceUrl,
                            _MaxRetries = serviceNowQueryUserDTO._MaxRetries,
                            PasswordBase64 = serviceNowQueryUserDTO.PasswordBase64,
                            _ResultNames = serviceNowQueryUserDTO._ResultNames,
                            Username = serviceNowQueryUserDTO.Username
                        },
                        token);
                _logger.Info("messageId", messageId);

                return Created(
                    new Uri(Request.RequestUri.AbsoluteUri.Replace("query", "") + 
                        ControllerHelpers.Base64ConvertTo(messageId)),
                    new QueueMessage() { MessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId) }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("groups/{messageidbase64}")]
        public async Task<IHttpActionResult> GetGroupAsync(string messageidbase64, CancellationToken token)
        {
            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Debug("messageId", messageId);

            if (messageidbase64 == null) return BadRequest();

            try
            {
                var result = await _queueServiceServiceNow.GetMessageAsync(messageId, token);
                if (result == null) return NotFound();

                var serviceNowResultGroupQueue = (ServiceNowResultGroupQueue)result;
                _logger.Debug(serviceNowResultGroupQueue, messageId);

                ErrorResultDTO errorResult = null;
                if (serviceNowResultGroupQueue.ErrorResult != null)
                    errorResult = new ErrorResultDTO()
                    {
                        Message = serviceNowResultGroupQueue.ErrorResult.Message
                    };

                if (serviceNowResultGroupQueue.Groups == null) return NotFound();

                var serviceNowGroupDTOs = new List<ServiceNowGroupDTO>();
                foreach (var user in serviceNowResultGroupQueue.Groups)
                {
                    serviceNowGroupDTOs.Add(
                        new ServiceNowGroupDTO()
                        {
                            Fields = ModelHelpers.ArrayToDictionary(user.Fields)
                        });
                }

                var serviceNowResultUserDTO = new ServiceNowResultGroupDTO()
                {
                    ErrorResult = errorResult,
                    Groups = serviceNowGroupDTOs
                };
                _logger.Info(serviceNowResultUserDTO, messageId);

                return Ok(serviceNowResultUserDTO);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("groups/query")]
        public async Task<IHttpActionResult> PostQueryGroup(
            ServiceNowQueryGroupDTO serviceNowQueryGroupDTO,
            CancellationToken token)
        {
            if (serviceNowQueryGroupDTO == null) return BadRequest();

            _logger.Info(serviceNowQueryGroupDTO);
            var messageId = string.Empty;

            try
            {
                messageId = await _queueServiceServiceNow.QueryGroupQueueAsync(
                        new ServiceNowQueryGroupQueue()
                        {
                            Fields = ModelHelpers.DictionaryToArray(serviceNowQueryGroupDTO.Fields),
                            InstanceUrl = serviceNowQueryGroupDTO.InstanceUrl,
                            _MaxRetries = serviceNowQueryGroupDTO._MaxRetries,
                            PasswordBase64 = serviceNowQueryGroupDTO.PasswordBase64,
                            _ResultNames = serviceNowQueryGroupDTO._ResultNames,
                            Username = serviceNowQueryGroupDTO.Username
                        },
                        token);
                _logger.Info("messageId", messageId);

                return Created(
                    new Uri(Request.RequestUri.AbsoluteUri.Replace("query", "") +
                        ControllerHelpers.Base64ConvertTo(messageId)),
                    new QueueMessage() { MessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId) }
                    );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("groups")]
        public async Task<IHttpActionResult> GetGroupAsync(
            [FromUri] ServiceNowGetGroupDTO serviceNowGetGroupDTO,
            CancellationToken token)
        {
            if (serviceNowGetGroupDTO == null) return BadRequest();

            _logger.Info(serviceNowGetGroupDTO);

            var messageId = string.Empty;
            try
            {
                var sysId = new string[] { };
                if (!string.IsNullOrEmpty(serviceNowGetGroupDTO.SysId))
                    sysId = new string[] 
                    { 
                        ServiceNowService.SnFields.sys_id.ToString(),
                        serviceNowGetGroupDTO.SysId
                    };
                messageId = await _queueServiceServiceNow.GetGroupQueueAsync(
                    new ServiceNowGetGroupQueue()
                    {
                        Fields = new string[][] { sysId },
                        _ResultNames = serviceNowGetGroupDTO._ResultNames,
                        InstanceUrl = serviceNowGetGroupDTO.InstanceUrl,
                        Username = serviceNowGetGroupDTO.Username,
                        PasswordBase64 = serviceNowGetGroupDTO.PasswordBase64,
                    },
                    token);
                _logger.Info("messageId", messageId);

                var resultMessageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);

                return Created(
                    new Uri(Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.Query, "") +
                        "/" + resultMessageIdBase64),
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