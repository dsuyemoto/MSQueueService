using EWS;
using Imanage;
using Imanage.Documents;
using LoggerHelper;
using MSMQHandlerService.Models;
using QueueService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Imanage.DocumentProfileItems;
using static Imanage.ImanageInput;

namespace MSMQHandlerService.Services
{
    public class ImanageService
    {
        const string EMAILEXTENSION = "EML";

        static ILogger _logger;
        static IQueueService _queueServiceImanage;
        static IQueueService _queueServiceEws;
        static ICacheService _cacheService;
        static IFileService _fileService;
        static IEwsService _ewsService;
        static List<string> _declareAsRecordExtensions = new List<string>();
        static bool _enableDeclareAsRecord = false;

        public ImanageService(
            IQueueService queueServiceImanage,
            IQueueService queueServiceEws,
            ICacheService cacheService,
            IFileService fileService,
            IEwsService ewsService,
            ILogger logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;
            _queueServiceImanage = queueServiceImanage;
            _queueServiceEws = queueServiceEws;
            _cacheService = cacheService;
            _fileService = fileService;
            _ewsService = ewsService;
        }

        public async Task<object> HandlerCallbackAsync(object document, CancellationToken token)
        {
            try
            {
                if (document is ImanageCreateDocumentQueue)
                    return await CreateDocumentAsync((ImanageCreateDocumentQueue)document, token);
                if (document is ImanageUpdateDocumentQueue)
                    return await UpdateDocumentAsync((ImanageUpdateDocumentQueue)document, token);
                if (document is ImanageGetDocumentQueue)
                    return await GetDocumentAsync((ImanageGetDocumentQueue)document, token);

                throw new Exception("Imanage document callback type not configured");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public static void SetDeclareAsRecordExtensions(StringCollection declareAsRecordExtensions)
        {
            if (declareAsRecordExtensions != null)
            {
                foreach (var extension in declareAsRecordExtensions)
                    _declareAsRecordExtensions.Add(extension);
                _enableDeclareAsRecord = true;
            }
        }

        private static async Task<ImanageResultDocumentQueue> CreateDocumentAsync(
            ImanageCreateDocumentQueue imanageCreateDocumentQueue,
            CancellationToken token)
        {
            _logger.Debug(imanageCreateDocumentQueue);

            var imanageResultDocumentQueue = new ImanageResultDocumentQueue();
            imanageResultDocumentQueue.FolderId = imanageCreateDocumentQueue.FolderId;
            if (imanageCreateDocumentQueue.Document == null) throw new Exception("Document is null");
            var sourceDocuments = new List<ImanageDocumentQueue>();
            var resultDocuments = new List<ImanageDocumentQueue>();
            var errors = new List<string>();

            try
            {
                token.ThrowIfCancellationRequested();

                if (
                    imanageCreateDocumentQueue.SourceEmail != null && 
                    (!string.IsNullOrEmpty(imanageCreateDocumentQueue.SourceEmail.MessageIdBase64) 
                    || !string.IsNullOrEmpty(imanageCreateDocumentQueue.SourceEmail.FolderUniqueId))
                    )
                {
                    sourceDocuments = await DownloadEmailsAsync(
                        imanageCreateDocumentQueue.SourceEmail,
                        imanageCreateDocumentQueue.Document,
                        token);

                    var newOutputProfiles = new List<string>();
                    newOutputProfiles.Add("Description");
                    if (imanageCreateDocumentQueue.OutputProfileIds != null && 
                        imanageCreateDocumentQueue.OutputProfileIds.Length > 0)
                        foreach (var outputProfile in imanageCreateDocumentQueue.OutputProfileIds)
                            newOutputProfiles.Add(outputProfile);
                    imanageCreateDocumentQueue.OutputProfileIds = newOutputProfiles.ToArray();
                }
                else if (!string.IsNullOrEmpty(imanageCreateDocumentQueue.Document.ContentBytesBase64))
                {
                    imanageCreateDocumentQueue.Document.Extension = imanageCreateDocumentQueue.Document.Extension;
                    sourceDocuments.Add(imanageCreateDocumentQueue.Document);
                }
                else if (!string.IsNullOrEmpty(imanageCreateDocumentQueue.SourceFilePath))
                {
                    imanageCreateDocumentQueue.Document.ContentBytesBase64 = Convert.ToBase64String(
                        await GetSourceFileAsync(imanageCreateDocumentQueue.SourceFilePath, token)
                        );
                    imanageCreateDocumentQueue.Document.Extension = GetExtension(imanageCreateDocumentQueue.SourceFilePath);
                    sourceDocuments.Add(imanageCreateDocumentQueue.Document);

                    _logger.Debug(imanageCreateDocumentQueue.SourceFilePath + " retrieved");   
                }
                else
                {
                    throw new Exception("No source content");
                }

                var imanageRepository = await GetImanageRepositoryAsync(
                    new ImanageDocumentSecurity(
                        imanageCreateDocumentQueue.Document.SecurityUsername,
                        imanageCreateDocumentQueue.Document.SecurityPasswordBase64,
                        imanageCreateDocumentQueue.Document.Session,
                        imanageCreateDocumentQueue.Document.Database
                    ), 
                    token);

                foreach (var sourceDocument in sourceDocuments)
                {
                    if (sourceDocument.ContentBytesBase64 == null)
                    {
                        _logger.Error($"Source Document Content is null for [Description]:{sourceDocument.DescriptionBase64}");
                        continue;
                    }

                    var emailProfileItems = new EmailProfileItems();
                    if (sourceDocument.EmailProperties != null)
                        emailProfileItems = new EmailProfileItems(
                                sourceDocument.EmailProperties.ToNames,
                                sourceDocument.EmailProperties.FromName,
                                sourceDocument.EmailProperties.CcNames,
                                sourceDocument.EmailProperties.SentDate,
                                sourceDocument.EmailProperties.ReceivedDate,
                                sourceDocument.EmailProperties.Subject);

                    var imanageCreateDocumentsInput = new ImanageCreateDocumentsInput(
                            new ImanageFolderObjectId(
                                sourceDocument.Session,
                                sourceDocument.Database,
                                imanageCreateDocumentQueue.FolderId),
                            new ImanageDocumentCreate[] {
                                new ImanageDocumentCreate(
                                    Convert.FromBase64String(sourceDocument.ContentBytesBase64),
                                    sourceDocument.Database,
                                    new DocumentProfileItemsCreate(
                                        sourceDocument.Author,
                                        Helpers.Base64ConvertFrom(sourceDocument.CommentBase64),
                                        Helpers.Base64ConvertFrom(sourceDocument.DescriptionBase64),
                                        sourceDocument.Operator,
                                        sourceDocument.Extension,
                                        emailProfileItems),
                                    new ImanageSecurityObject(
                                        new ImanageAclItem[] {
                                                new ImanageAclItem(
                                                    sourceDocument.SecurityUsername,
                                                    ImanageAclItem.ImanageAccessRight.ALL,
                                                    ImanageAclItem.ImanageAclItemType.USER
                                                    ) },
                                        ImanageSecurityObject.SecurityType.PUBLIC)) },
                            ImanageHelpers.BuildProfileAttributeIds(imanageCreateDocumentQueue.OutputProfileIds),
                            new OutputMaskName[] { OutputMaskName.Profile, OutputMaskName.Security },
                            true);
                    _logger.Debug(imanageCreateDocumentsInput);

                    var imanageDocumentsOutput = await Task.Run(
                        () => imanageRepository.CreateDocuments(imanageCreateDocumentsInput),
                        token);
                    _logger.Debug(imanageDocumentsOutput);

                    if (imanageDocumentsOutput.Errors.Length > 0)
                        foreach (var error in imanageDocumentsOutput.Errors)
                            errors.Add(error);

                    if (imanageDocumentsOutput.Documents.Length > 0)
                    {
                        var imanageDocumentOutput = imanageDocumentsOutput.Documents[0];

                        resultDocuments.Add(new ImanageDocumentQueue(imanageDocumentOutput));

                        if (imanageDocumentOutput.DocumentObjectId != null)
                        {
                            var documentProfileItemsSet = new DocumentProfileItemsSet(imanageDocumentOutput.DocumentProfileItems);

                            if (_declareAsRecordExtensions.Contains(sourceDocument.Extension))
                                documentProfileItemsSet.DeclareAsRecord = _enableDeclareAsRecord;
                            _logger.Debug(documentProfileItemsSet);

                            await Task.Run(() => imanageRepository.UpdateDocuments(
                                new ImanageSetDocumentsPropertiesInput(
                                    new ImanageFolderObjectId(
                                        imanageCreateDocumentQueue.Document.Session,
                                        imanageCreateDocumentQueue.Document.Database,
                                        imanageCreateDocumentQueue.FolderId),
                                        new ImanageDocumentSet[] {
                                        new ImanageDocumentSet(
                                            null,
                                            documentProfileItemsSet,
                                            new ImanageSecurityObject(
                                                new ImanageAclItem[] {
                                                        new ImanageAclItem(
                                                            sourceDocument.SecurityUsername,
                                                            ImanageAclItem.ImanageAccessRight.ALL,
                                                            ImanageAclItem.ImanageAclItemType.USER
                                                            ) },
                                                ImanageSecurityObject.SecurityType.PUBLIC),
                                            imanageDocumentOutput.DocumentObjectId
                                        ) },
                                ImanageHelpers.BuildProfileAttributeIds(imanageCreateDocumentQueue.OutputProfileIds),
                                new OutputMaskName[] { OutputMaskName.Profile, OutputMaskName.Security })), token);
                        }

                        LogImanageErrors(imanageDocumentOutput);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return new ImanageResultDocumentQueue(
                resultDocuments.ToArray(),
                imanageCreateDocumentQueue.FolderId,
                new ErrorResultQueue(errors.ToArray()));
        }

        private static async Task<ImanageResultDocumentQueue> GetDocumentAsync(
            ImanageGetDocumentQueue imanageGetDocumentQueue,
            CancellationToken token)
        {
            _logger.Debug(imanageGetDocumentQueue);
            
            var imanageResultDocumentQueue = new ImanageResultDocumentQueue();

            try
            {
                token.ThrowIfCancellationRequested();

                var imanageRepositoryWrapper = await GetImanageRepositoryAsync(
                    new ImanageDocumentSecurity(
                        imanageGetDocumentQueue.SecurityUsername,
                        imanageGetDocumentQueue.SecurityPasswordBase64,
                        imanageGetDocumentQueue.Session,
                        imanageGetDocumentQueue.Database),
                    token);

                var imanageDocumentOutputs = imanageRepositoryWrapper.GetDocuments(
                    new ImanageGetDocumentsInput(
                        new ImanageDocumentObjectId[] { 
                            new ImanageDocumentObjectId(imanageGetDocumentQueue.Session,
                                imanageGetDocumentQueue.Database, 
                                imanageGetDocumentQueue.Number,
                                imanageGetDocumentQueue.Version) },
                        ImanageHelpers.BuildProfileAttributeIds(imanageGetDocumentQueue.OutputProfileItems),
                        new OutputMaskName[] { 
                            OutputMaskName.DocumentContent,
                            OutputMaskName.Profile,
                            OutputMaskName.Security }));

                var imanageDocumentQueues = new List<ImanageDocumentQueue>();
                foreach (var imanageDocumentOutput in imanageDocumentOutputs.Documents)
                {
                    _logger.Debug(imanageDocumentOutput);
                    if (imanageDocumentOutput.DocumentObjectId == null && imanageDocumentOutput.ImanageError != null)
                        LogImanageErrors(imanageDocumentOutput);
                    imanageDocumentQueues.Add(new ImanageDocumentQueue(imanageDocumentOutput));
                }

                return new ImanageResultDocumentQueue()
                {
                    Documents = imanageDocumentQueues.ToArray()
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return imanageResultDocumentQueue;
        }

        private static async Task<ImanageResultDocumentQueue> UpdateDocumentAsync(
            ImanageUpdateDocumentQueue imanageUpdateDocumentQueue, 
            CancellationToken token)
        {
            _logger.Debug(imanageUpdateDocumentQueue);
            var messageId = string.Empty;

            var imanageResultDocumentQueue = new ImanageResultDocumentQueue();
            var sourceDocuments = new List<ImanageDocumentQueue>();
            var imanageDocumentQueue = imanageUpdateDocumentQueue.Document;

            try
            {
                token.ThrowIfCancellationRequested();

                if (!string.IsNullOrEmpty(imanageUpdateDocumentQueue.MessageIdBase64))
                {
                    messageId = Helpers.Base64ConvertFrom(imanageUpdateDocumentQueue.MessageIdBase64);
                    _logger.Debug(imanageUpdateDocumentQueue, messageId);

                    var result = await _queueServiceImanage.GetMessageAsync(messageId, token, false);
                    if (result == null) throw new Exception("Could not find document");

                    var documents = ((ImanageResultDocumentQueue)result).Documents;
                    if (documents == null || documents.Length == 0)
                        throw new Exception("Could not find document");

                    var updateDocument = imanageUpdateDocumentQueue.Document.Copy();

                    foreach (var document in documents)
                    {
                        _logger.Debug(document, messageId);
                        updateDocument.Number = document.Number;
                        updateDocument.Version = document.Version;

                        sourceDocuments.Add(updateDocument);
                    }
                }
                else if (imanageUpdateDocumentQueue.SourceEmail != null && !string.IsNullOrEmpty(imanageUpdateDocumentQueue.SourceEmail.MessageIdBase64))
                {
                    var emailDocuments = await DownloadEmailsAsync(
                        imanageUpdateDocumentQueue.SourceEmail,
                        imanageUpdateDocumentQueue.Document,
                        token);

                    var updateDocument = imanageUpdateDocumentQueue.Document.Copy();

                    foreach (var emailDocument in emailDocuments)
                    {
                        _logger.Debug(emailDocument, messageId);
                        updateDocument.ContentBytesBase64 = emailDocument.ContentBytesBase64;
                        updateDocument.DescriptionBase64 = emailDocument.DescriptionBase64;
                        updateDocument.EmailProperties = emailDocument.EmailProperties;

                        emailDocuments.Add(updateDocument);
                    }
                }
                else if (!string.IsNullOrEmpty(imanageUpdateDocumentQueue.SourceFilePath))
                {
                    var updateDocument = imanageUpdateDocumentQueue.Document.Copy();

                    var contentBytes = await GetSourceFileAsync(imanageUpdateDocumentQueue.SourceFilePath, token);
                    updateDocument.ContentBytesBase64 = Convert.ToBase64String(contentBytes);

                    sourceDocuments.Add(updateDocument);

                    _logger.Debug(imanageUpdateDocumentQueue.SourceFilePath + " retrieved");
                }

                var imanageRepository = await GetImanageRepositoryAsync(
                    new ImanageDocumentSecurity()
                    {
                        Database = imanageUpdateDocumentQueue.Document.Database,
                        Session = imanageUpdateDocumentQueue.Document.Session,
                        SecurityUsername = imanageUpdateDocumentQueue.Document.SecurityUsername,
                        SecurityPasswordBase64 = imanageUpdateDocumentQueue.Document.SecurityPasswordBase64
                    }, 
                    token);

                var imanageDocumentsSet = new List<ImanageDocumentSet>();
                foreach (var sourceDocument in sourceDocuments)
                {
                    _logger.Debug(sourceDocument);

                    EmailProfileItems emailProfileItems = null;
                    if (sourceDocument.EmailProperties != null)
                        emailProfileItems = new EmailProfileItems(
                            sourceDocument.EmailProperties.ToNames,
                            sourceDocument.EmailProperties.FromName,
                            sourceDocument.EmailProperties.CcNames,
                            sourceDocument.EmailProperties.SentDate,
                            sourceDocument.EmailProperties.ReceivedDate,
                            sourceDocument.EmailProperties.Subject
                            );

                    var documentProfileItemsSet = new DocumentProfileItemsSet(
                        sourceDocument.Author,
                        Helpers.Base64ConvertFrom(sourceDocument.CommentBase64),
                        null,
                        sourceDocument.Operator,
                        emailProfileItems
                        );

                    byte[] content = null;
                    if (!string.IsNullOrEmpty(sourceDocument.ContentBytesBase64))
                        content = Convert.FromBase64String(sourceDocument.ContentBytesBase64);

                    imanageDocumentsSet.Add(
                        new ImanageDocumentSet(
                            content,
                            documentProfileItemsSet,
                            new ImanageSecurityObject(
                                new ImanageAclItem[] {
                                new ImanageAclItem(
                                    sourceDocument.SecurityUsername,
                                    ImanageAclItem.ImanageAccessRight.ALL,
                                    ImanageAclItem.ImanageAclItemType.USER)},
                                ImanageSecurityObject.SecurityType.PRIVATE),
                            new ImanageDocumentObjectId(
                                sourceDocument.Session,
                                sourceDocument.Database,
                                sourceDocument.Number,
                                sourceDocument.Version)
                            )
                        );
                }

                var imanageDocumentsOutput = await Task.Run(() => imanageRepository.UpdateDocuments(
                    new ImanageSetDocumentsPropertiesInput(
                        new ImanageFolderObjectId(
                            imanageDocumentQueue.Session,
                            imanageDocumentQueue.Database,
                            imanageUpdateDocumentQueue.FolderId
                            ),
                        imanageDocumentsSet.ToArray(),
                        ImanageHelpers.BuildProfileAttributeIds(imanageUpdateDocumentQueue.OutputProfileItems),
                        new OutputMaskName[] {
                        OutputMaskName.DocumentContent,
                        OutputMaskName.Profile,
                        OutputMaskName.Security
                        })), token);

                var imanageDocumentResults = new List<ImanageDocumentQueue>();

                foreach (var imanageDocumentOutput in imanageDocumentsOutput.Documents)
                {
                    _logger.Debug(imanageDocumentOutput);

                    if (imanageDocumentOutput.DocumentObjectId == null && imanageDocumentOutput.ImanageError != null)
                        LogImanageErrors(imanageDocumentOutput);

                    imanageDocumentResults.Add(new ImanageDocumentQueue(imanageDocumentOutput));
                }

                imanageResultDocumentQueue = new ImanageResultDocumentQueue(
                    imanageDocumentResults.ToArray(),
                    imanageUpdateDocumentQueue.FolderId,
                    null);
            }
            catch (Exception ex)
            {
                _logger.Debug(messageId);
                _logger.Error(ex);
                imanageResultDocumentQueue.ErrorResult = new ErrorResultQueue(ex);
            }

            return imanageResultDocumentQueue;
        }

        private static void LogImanageErrors(ImanageDocumentOutput imanageDocumentOutput)
        {
            if (imanageDocumentOutput == null || imanageDocumentOutput.ImanageError == null) return;

            if (!string.IsNullOrEmpty(imanageDocumentOutput.ImanageError.Message))
                _logger.Error("ImanageError: " + imanageDocumentOutput.ImanageError.Message);

            if (imanageDocumentOutput.ImanageError.ImanageProfileErrors != null)
                foreach (var imanageProfileError in imanageDocumentOutput.ImanageError.ImanageProfileErrors)
                    _logger.Error("ImanageProfileError: " + imanageProfileError.Message);
        }

        private static async Task<List<ImanageDocumentQueue>> DownloadEmailsAsync(
            ImanageSourceEmailQueue sourceEmail,
            ImanageDocumentQueue sourceDocument,
            CancellationToken token)
        {
            _logger.Debug(sourceEmail);
            _logger.Debug(sourceDocument);

            var sourceDocuments = new List<ImanageDocumentQueue>();
            object result = null;

            if (!string.IsNullOrEmpty(sourceEmail.MessageIdBase64))
            {
                result = await _queueServiceEws.GetMessageAsync(
                    Helpers.Base64ConvertFrom(sourceEmail.MessageIdBase64), 
                    token,
                    false);
            }
            else if(!string.IsNullOrEmpty(sourceEmail.FolderUniqueId))
            {
                result = await _ewsService.HandlerCallbackAsync(
                    new EwsGetEmailsQueue()
                    {
                        Folder = new EwsFolderQueue() {
                            UniqueId = sourceEmail.FolderUniqueId,
                            Name = sourceEmail.FolderName
                        },
                        Creds = sourceEmail.Creds
                    },
                    token);
            }

            if (result == null) return sourceDocuments;
            
            var emails = ((EwsResultEmailQueue)result).Emails;
            if (emails == null || emails.Length == 0) throw new Exception("No emails found");

            var ewsFolder = new EwsFolderQueue();

            foreach (var email in emails)
            {
                foreach (var content in sourceEmail.Content)
                {
                    if (content.ToLower() == "attach")
                    {
                        var attachments = await _ewsService.DownloadAttachmentsAsync(
                                email, 
                                sourceEmail.Creds,
                                token);

                        foreach (var attachment in attachments)
                        {
                            var attachmentDocument = sourceDocument.Copy();

                            if (attachment is ExchAttachmentEmail)
                            {
                                var exchAttachmentEmail = (ExchAttachmentEmail)attachment;

                                attachmentDocument.ContentBytesBase64 = Convert.ToBase64String(exchAttachmentEmail.Content);
                                attachmentDocument.EmailProperties = new ImanageEmailPropertiesQueue()
                                {
                                    FromName = exchAttachmentEmail.ExchEmail.FromName,
                                    ToNames = exchAttachmentEmail.ExchEmail.ToNames,
                                    CcNames = exchAttachmentEmail.ExchEmail.CcNames,
                                    ReceivedDate = exchAttachmentEmail.ExchEmail.Received.ToString(),
                                    SentDate = exchAttachmentEmail.ExchEmail.Sent.ToString()
                                };
                                attachmentDocument.DescriptionBase64 = Helpers.Base64ConvertTo(
                                    $"{exchAttachmentEmail.ExchEmail.Subject}.{EMAILEXTENSION.ToLower()}");
                                attachmentDocument.Extension = EMAILEXTENSION;
                            }
                            else if (attachment is ExchAttachmentFile)
                            {
                                var exchAttachmentFile = (ExchAttachmentFile)attachment;

                                attachmentDocument.ContentBytesBase64 = Convert.ToBase64String(exchAttachmentFile.Content);
                                attachmentDocument.DescriptionBase64 = Helpers.Base64ConvertTo(exchAttachmentFile.FileName);
                                attachmentDocument.Extension = GetExtension(exchAttachmentFile.FileName);
                            }

                            sourceDocuments.Add(attachmentDocument);
                        }
                    }
                    else if (content.ToLower() == "email")
                    {
                        var emailDocument = sourceDocument.Copy();

                        var sourceContent = await _ewsService.DownloadEmailAsync(
                            email.UniqueId,
                            sourceEmail.Creds,
                            token);
                        if (sourceContent == null) throw new Exception("Unable to download email");

                        emailDocument.ContentBytesBase64 = Helpers.Base64ConvertTo(sourceContent);
                        emailDocument.EmailProperties = new ImanageEmailPropertiesQueue()
                        {
                            FromName = email.FromName,
                            ToNames = email.ToNames,
                            CcNames = email.CcNames,
                            ReceivedDate = email.ReceivedDate,
                            SentDate = email.SentDate
                        };
                        if (string.IsNullOrEmpty(emailDocument.DescriptionBase64))
                            emailDocument.DescriptionBase64 = Helpers.Base64ConvertTo($"{email.Subject}.{EMAILEXTENSION.ToLower()}");
                        emailDocument.Extension = EMAILEXTENSION;

                        sourceDocuments.Add(emailDocument);
                    }
                }

                ewsFolder = email.EwsFolder;
                _logger.Debug(ewsFolder);
            }

            await DeleteSourceFolderAsync(ewsFolder, sourceEmail, token);

            return sourceDocuments;
        }

        private static async Task<object> DeleteSourceFolderAsync(
            EwsFolderQueue ewsFolder,
            ImanageSourceEmailQueue sourceEmail, 
            CancellationToken token)
        {
            if (!sourceEmail.DeleteSourceFolder) return null;

            var deleteResult = await _ewsService.HandlerCallbackAsync(
                    new EwsDeleteFolderQueue()
                    {
                        Creds = sourceEmail.Creds,
                        UniqueId = ewsFolder.UniqueId,
                        FolderName = sourceEmail.FolderName
                    }, token);

            if (deleteResult != null)
            {
                var ewsFolderResult = (EwsResultFolderQueue)deleteResult;

                if (ewsFolderResult.ErrorResult != null)
                    _logger.Error(ewsFolderResult.ErrorResult.Message);
                else
                    _logger.Info("Folder deleted: " + ewsFolder.Name);
            }

            return deleteResult;
        }

        private static void UseAttachments(
            List<ImanageDocumentQueue> imanageDocumentQueues, 
            List<IExchAttachment> exchAttachments)
        {
            foreach (var attachment in exchAttachments)
            {
                var imanageDocumentQueue = new ImanageDocumentQueue();

                if (attachment is ExchAttachmentEmail)
                {
                    var exchAttachmentEmail = (ExchAttachmentEmail)attachment;
                    _logger.Debug($"Adding attachment {exchAttachmentEmail.FileName} to import");

                    imanageDocumentQueue.ContentBytesBase64 = Convert.ToBase64String(exchAttachmentEmail.Content);
                    imanageDocumentQueue.EmailProperties = new ImanageEmailPropertiesQueue()
                    {
                        FromName = exchAttachmentEmail.ExchEmail.FromName,
                        ToNames = exchAttachmentEmail.ExchEmail.ToNames,
                        CcNames = exchAttachmentEmail.ExchEmail.CcNames,
                        ReceivedDate = exchAttachmentEmail.ExchEmail.Received.ToString(),
                        SentDate = exchAttachmentEmail.ExchEmail.Sent.ToString()
                    };
                    imanageDocumentQueue.DescriptionBase64 = Helpers.Base64ConvertTo(
                        exchAttachmentEmail.ExchEmail.Subject + ".eml"
                        );
                }
                else if (attachment is ExchAttachmentFile)
                {
                    var exchAttachmentFile = (ExchAttachmentFile)attachment;
                    _logger.Debug($"Adding attachment {exchAttachmentFile.FileName} to import");

                    imanageDocumentQueue.ContentBytesBase64 = Convert.ToBase64String(exchAttachmentFile.Content);
                    imanageDocumentQueue.DescriptionBase64 = Helpers.Base64ConvertTo(exchAttachmentFile.FileName);
                }

                imanageDocumentQueues.Add(imanageDocumentQueue);
            }
        }

        private static void UseEmails(
            List<ImanageDocumentQueue> imanageDocumentQueues,
            byte[] sourceContent,
            EwsEmailQueue email)
        {
            var imanageDocumentQueue = new ImanageDocumentQueue();
            imanageDocumentQueue.ContentBytesBase64 = Convert.ToBase64String(sourceContent);
            if (imanageDocumentQueue.EmailProperties == null)
                imanageDocumentQueue.EmailProperties = new ImanageEmailPropertiesQueue()
                {
                    FromName = email.FromName,
                    ToNames = email.ToNames,
                    CcNames = email.CcNames,
                    ReceivedDate = email.ReceivedDate,
                    SentDate = email.SentDate
                };
            if (string.IsNullOrEmpty(imanageDocumentQueue.DescriptionBase64))
                imanageDocumentQueue.DescriptionBase64 = Helpers.Base64ConvertTo(email.Subject + ".eml");
            _logger.Debug($"Adding email {email.Subject} to import");

            imanageDocumentQueues.Add(imanageDocumentQueue);
        }

        private static async Task<byte[]> GetSourceFileAsync(string sourceFilePath, CancellationToken token)
        {
            if (!_fileService.Exists(sourceFilePath))
                throw new Exception("No file found at " + sourceFilePath);

            var sourceContent = await _fileService.ReadAsync(sourceFilePath, token);
            if (sourceContent == null) throw new Exception("Unable to retrieve file");
            _logger.Debug(
                "Retrieved source file: " + 
                Helpers.Base64ConvertTo(Path.GetFileName(sourceFilePath))
                );

            try
            {
                _fileService.Delete(sourceFilePath);
                _logger.Debug(sourceFilePath + " deleted");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return sourceContent;
        }

        private static async Task<IImanageRepository> GetImanageRepositoryAsync(
            ImanageDocumentSecurity imanageDocumentSecurity,
            CancellationToken token
            )
        {
            _logger.Debug(imanageDocumentSecurity);

            var serviceUrl = Properties.Settings.Default["ImanageUrl" + 
                imanageDocumentSecurity.Database.Replace("-", "").ToUpper()].ToString();
            _logger.Debug($"serviceUrl = {serviceUrl}");

            return (IImanageRepository)await _cacheService.GetConnectionAsync(
                imanageDocumentSecurity, 
                () => 
                    {
                        return new ImanageRepository(
                            new ImanageCreds(
                                imanageDocumentSecurity.Database,
                                imanageDocumentSecurity.SecurityUsername,
                                Helpers.Base64ConvertFrom(imanageDocumentSecurity.SecurityPasswordBase64)
                                )
                        );
                    }, 
                token);
        }

        
    }
}