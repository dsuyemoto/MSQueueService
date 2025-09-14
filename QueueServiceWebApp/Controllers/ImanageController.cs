using LoggerHelper;
using MSMQHandlerService.Models;
using MSMQHandlerService.Services;
using QueueServiceWebApp.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static QueueService.QueueServiceBase;

namespace QueueServiceWebApp.Controllers
{
    [RoutePrefix("api/imanage")]
    public class ImanageController : ApiController
    {
        ILogger _logger;
        IQueueServiceImanage _queueServiceImanage;
        IHttpPostedFileWrapper _imanageFile;
        string _imanageFileCachePath;
        string _guid;

        public ImanageController()
        {
            _logger = LoggerFactory.GetLogger(
                new NlogLogConfiguration(
                    Properties.Settings.Default.LogLevel,
                    "QueueServiceWebApp",
                    new NlogTargetConfigurationBase[] {
                        new NlogFileTargetConfiguration(
                            Properties.Settings.Default.LogFilePath,
                            Properties.Settings.Default.LogFileSizeMB,
                            "ImanageController") }
                    ));
            try
            {
                _queueServiceImanage = new QueueServiceImanage(QueueType.MSMQ, true, _logger);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
            }

            if (HttpContext.Current != null)
                _imanageFileCachePath = HttpContext.Current.Server.MapPath("~/App_Data/uploads");

            if (!string.IsNullOrEmpty(Properties.Settings.Default.ImanageFileCache))
                _imanageFileCachePath = Properties.Settings.Default.ImanageFileCache;
        }

        public ImanageController(
            IQueueServiceImanage queueServiceImanage,
            IHttpPostedFileWrapper imanageFile,
            string imanageFileCachePath,
            string guid)
        {
            _queueServiceImanage = queueServiceImanage;
            _imanageFile = imanageFile;
            _imanageFileCachePath = imanageFileCachePath;
            _guid = guid;
            _logger = LoggerFactory.GetLogger(new DummyLogConfiguration("", ""));
        }

        [Route("documents/{messageidbase64}")]
        public async Task<IHttpActionResult> GetDocumentAsync(string messageidbase64, CancellationToken token)
        {
            if (!ControllerHelpers.CheckIfBase64(messageidbase64)) return BadRequest();
            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Info("messageId", messageId);

            try
            {
                var result = await _queueServiceImanage.GetMessageAsync(
                    ControllerHelpers.Base64ConvertFrom(messageidbase64),
                    token, 
                    false);

                if (result == null) return NotFound();

                var imanageResultDocumentQueue = (ImanageResultDocumentQueue)result;
                _logger.Info(imanageResultDocumentQueue, messageId);

                return Ok(new ImanageResultDocumentDTO(imanageResultDocumentQueue));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("documents")]
        public async Task<IHttpActionResult> GetDocumentAsync(
            [FromUri]ImanageGetDocumentDTO imanageGetDocumentDTO,
            CancellationToken token)
        {
            if (imanageGetDocumentDTO == null) return BadRequest();

            _logger.Info(imanageGetDocumentDTO);

            var messageId = string.Empty;
            try
            {
                var outputProfileItems = new string[] { };
                if (!string.IsNullOrEmpty(imanageGetDocumentDTO.OutputProfileItems))
                    outputProfileItems = imanageGetDocumentDTO.OutputProfileItems.Split(',');

                messageId = await _queueServiceImanage.GetDocumentQueueAsync(
                    new ImanageGetDocumentQueue()
                    {
                        Database = imanageGetDocumentDTO.Database,
                        Session = imanageGetDocumentDTO.Session,
                        SecurityUsername = imanageGetDocumentDTO.SecurityUsername,
                        SecurityPasswordBase64 = imanageGetDocumentDTO.SecurityPasswordBase64,
                        Number = imanageGetDocumentDTO.Number,
                        Version = imanageGetDocumentDTO.Version,
                        OutputProfileItems = outputProfileItems,
                        MaxRetries = imanageGetDocumentDTO.MaxRetries,
                        MaxWaitTimeSeconds = imanageGetDocumentDTO.MaxWaitTimeSeconds,
                    },
                    token);
                _logger.Debug("response messageId ", messageId);

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

        [Route("documents")]
        public async Task<IHttpActionResult> PostDocumentAsync(
            ImanageCreateDocumentDTO imanageCreateDocumentDTO,
            CancellationToken token)
        {
            _logger.Info(imanageCreateDocumentDTO);

            var messageId = string.Empty;
            try
            {               
                if (Request.Content.IsMimeMultipartContent())
                {
                    if (_imanageFile == null)
                    {
                        var files = HttpContext.Current.Request.Files;

                        if (files.Count == 0)
                        {
                            _logger.Debug("File count = 0");
                            return BadRequest();
                        }

                        _imanageFile = new Models.HttpPostedFileWrapper(files[0]);
                    }

                    if (HttpContext.Current != null)
                    {
                        imanageCreateDocumentDTO = (ImanageCreateDocumentDTO)ControllerHelpers.MapFormDataProperties(
                            typeof(ImanageCreateDocumentDTO),
                            HttpContext.Current.Request.Form);
                        _logger.Debug(imanageCreateDocumentDTO);
                    }
                    imanageCreateDocumentDTO._SourceFilePath = await UploadFileAsync(_imanageFile);
                    _logger.Debug(imanageCreateDocumentDTO._SourceFilePath);

                    if (string.IsNullOrEmpty(imanageCreateDocumentDTO.Document._DescriptionBase64))
                        imanageCreateDocumentDTO.Document._DescriptionBase64 
                            = ControllerHelpers.Base64ConvertTo(_imanageFile.FileName);
                    _logger.Debug(imanageCreateDocumentDTO.Document._DescriptionBase64);                    
                }
                else if (imanageCreateDocumentDTO == null)
                {
                    return BadRequest();
                }

                var ewsCreds = new EwsCredsQueue();                
                var sourceEmailQueue = new ImanageSourceEmailQueue();
                if (imanageCreateDocumentDTO._SourceEmail != null) 
                {
                    if (imanageCreateDocumentDTO._SourceEmail.Creds != null)
                    {
                        ewsCreds.AutodiscoverEmailAddress = imanageCreateDocumentDTO._SourceEmail.Creds.AutodiscoverEmailAddress;
                        ewsCreds.PasswordBase64 = imanageCreateDocumentDTO._SourceEmail.Creds.PasswordBase64;
                        ewsCreds.Username = imanageCreateDocumentDTO._SourceEmail.Creds.Username;
                    }
                    sourceEmailQueue.Content = imanageCreateDocumentDTO._SourceEmail.Content;
                    sourceEmailQueue.Creds = ewsCreds;
                    sourceEmailQueue.DeleteSourceFolder = imanageCreateDocumentDTO._SourceEmail._DeleteSourceFolder;
                    sourceEmailQueue.FolderName = imanageCreateDocumentDTO._SourceEmail.FolderName;
                    sourceEmailQueue.FolderUniqueId = imanageCreateDocumentDTO._SourceEmail.FolderUniqueId;
                    sourceEmailQueue.MessageIdBase64 = imanageCreateDocumentDTO._SourceEmail.MessageIdBase64;
                }
                var emailPropertiesQueue = new ImanageEmailPropertiesQueue();
                var documentQueue = new ImanageDocumentQueue();
                if (imanageCreateDocumentDTO.Document != null) 
                {
                    if (imanageCreateDocumentDTO.Document._EmailProperties != null)
                    {
                        emailPropertiesQueue.CcNames = imanageCreateDocumentDTO.Document._EmailProperties.CcNames;
                        emailPropertiesQueue.FromName = imanageCreateDocumentDTO.Document._EmailProperties.FromName;
                        emailPropertiesQueue.ReceivedDate = imanageCreateDocumentDTO.Document._EmailProperties.ReceivedDate;
                        emailPropertiesQueue.SentDate = imanageCreateDocumentDTO.Document._EmailProperties.SentDate;
                        emailPropertiesQueue.Subject = imanageCreateDocumentDTO.Document._EmailProperties.Subject;
                        emailPropertiesQueue.ToNames = imanageCreateDocumentDTO.Document._EmailProperties.ToNames;
                    }
                    documentQueue.Author = imanageCreateDocumentDTO.Document.Author;
                    documentQueue.CommentBase64 = imanageCreateDocumentDTO.Document._CommentBase64;
                    documentQueue.ContentBytesBase64 = imanageCreateDocumentDTO.Document._ContentBytesBase64;
                    documentQueue.Database = imanageCreateDocumentDTO.Document.Database;
                    documentQueue.DescriptionBase64 = imanageCreateDocumentDTO.Document._DescriptionBase64;
                    documentQueue.EmailProperties = emailPropertiesQueue;
                    documentQueue.Operator = imanageCreateDocumentDTO.Document.Operator;
                    documentQueue.SecurityPasswordBase64 = imanageCreateDocumentDTO.Document.SecurityPasswordBase64;
                    documentQueue.SecurityUsername = imanageCreateDocumentDTO.Document.SecurityUsername;
                    documentQueue.Session = imanageCreateDocumentDTO.Document.Session;
                    documentQueue.Extension = imanageCreateDocumentDTO.Document._Extension;
                }
                var imanageCreateDocumentQueue = new ImanageCreateDocumentQueue()
                {
                    Document = documentQueue,
                    SourceEmail = sourceEmailQueue,
                    FolderId = imanageCreateDocumentDTO.FolderId,
                    OutputProfileIds = imanageCreateDocumentDTO.OutputProfileIds,
                    SourceFilePath = imanageCreateDocumentDTO._SourceFilePath,
                    MaxRetries = imanageCreateDocumentDTO.MaxRetries,
                    MaxWaitTimeSeconds = imanageCreateDocumentDTO.MaxWaitTimeSeconds,
                };
                _logger.Debug(imanageCreateDocumentQueue);

                messageId = await _queueServiceImanage.CreateDocumentQueueAsync(
                        imanageCreateDocumentQueue,
                        token);
                _logger.Debug("response messageId", messageId);

                var messageIdBase64 = ControllerHelpers.Base64ConvertTo(messageId);
                return Created(
                        new Uri(Request.RequestUri + "/" + messageIdBase64),
                        new QueueMessage() { MessageIdBase64 = messageIdBase64 }
                        );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, messageId);

                return InternalServerError();
            }
        }

        [Route("documents/{messageidbase64}")]
        public async Task<IHttpActionResult> PutDocumentAsync(
            string messageidbase64,
            ImanageUpdateDocumentDTO imanageUpdateDocumentDTO,
            CancellationToken token)
        {
            if (string.IsNullOrEmpty(messageidbase64) || imanageUpdateDocumentDTO.Document == null) return BadRequest();

            var messageId = ControllerHelpers.Base64ConvertFrom(messageidbase64);
            _logger.Info(imanageUpdateDocumentDTO, messageId);

            try
            {
                ImanageEmailPropertiesQueue imanageEmailPropertiesQueue = null;
                if (imanageUpdateDocumentDTO.Document._EmailProperties != null)
                    imanageEmailPropertiesQueue = new ImanageEmailPropertiesQueue()
                    {
                        CcNames = imanageUpdateDocumentDTO.Document._EmailProperties.CcNames,
                        FromName = imanageUpdateDocumentDTO.Document._EmailProperties.FromName,
                        ReceivedDate = imanageUpdateDocumentDTO.Document._EmailProperties.ReceivedDate,
                        SentDate = imanageUpdateDocumentDTO.Document._EmailProperties.SentDate,
                        Subject = imanageUpdateDocumentDTO.Document._EmailProperties.Subject,
                        ToNames = imanageUpdateDocumentDTO.Document._EmailProperties.ToNames
                    };

                var imanageUpdateDocumentQueue = new ImanageUpdateDocumentQueue()
                {
                    AttachmentsOnly = imanageUpdateDocumentDTO._AttachmentsOnly,
                    Document = new ImanageDocumentQueue()
                    {
                        CommentBase64 = imanageUpdateDocumentDTO.Document._CommentBase64,
                        ContentBytesBase64 = imanageUpdateDocumentDTO.Document._ContentBytesBase64,
                        Database = imanageUpdateDocumentDTO.Document.Database,
                        DescriptionBase64 = imanageUpdateDocumentDTO.Document._DescriptionBase64,
                        EmailProperties = imanageEmailPropertiesQueue,
                        SecurityPasswordBase64 = imanageUpdateDocumentDTO.Document.SecurityPasswordBase64,
                        SecurityUsername = imanageUpdateDocumentDTO.Document.SecurityUsername,
                        Session = imanageUpdateDocumentDTO.Document.Session
                    },
                    FolderId = imanageUpdateDocumentDTO.FolderId
                };
                imanageUpdateDocumentQueue.MessageIdBase64 = messageidbase64;
                _logger.Debug(imanageUpdateDocumentQueue);

                messageId = await _queueServiceImanage.UpdateDocumentQueueAsync(
                    imanageUpdateDocumentQueue,
                    token);
                _logger.Debug("response messageId", messageId);

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

        private async Task<string> UploadFileAsync(IHttpPostedFileWrapper imanageFile)
        {
            _logger.Info(imanageFile);
            if (string.IsNullOrEmpty(_imanageFileCachePath)) 
                throw new Exception("Could not create imanage file cache");

            if (string.IsNullOrEmpty(_guid))
                _guid = Guid.NewGuid().ToString();

            return await Task.Run(() => {
                var filePath = _imanageFileCachePath + "\\" + _guid + "_" + imanageFile.FileName;
                _logger.Debug(filePath);
                imanageFile.SaveAs(filePath);
                _logger.Debug("File Saved: " + filePath);
                return filePath;
            });
        }
    }
}