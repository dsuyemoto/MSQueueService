using EWS;
using LoggerHelper;
using MSMQHandlerService.Models;
using QueueService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public class EwsService : IEwsService
    {
        const char FOLDERPATHDIVIDER = '/';

        static ILogger _logger;
        static ICacheService _cacheService { get; set; }
        static IQueueService _queueServiceEws { get; set; }

        public static int RetryIntervalMilliseconds { get; set; }
        public static int RetryMaxWaitTimeSeconds { get; set; }
        public static string EwsTraceFolderPath { get; set; }
        public static bool EwsTraceEnabled { get; set; }

        public EwsService(IQueueService queueServiceEws, ICacheService cacheService, ILogger logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            RetryIntervalMilliseconds = 1000;
            RetryMaxWaitTimeSeconds = 600;
            EwsTraceFolderPath = "";
            EwsTraceEnabled = false;
            _queueServiceEws = queueServiceEws;
            _cacheService = cacheService;
        }

        public async Task<object> HandlerCallbackAsync(object ewsObject, CancellationToken token)
        {
            try
            {
                if (ewsObject.GetType() == typeof(EwsGetEmailsQueue))
                    return await GetEmailsAsync((EwsGetEmailsQueue)ewsObject, token);
                if (ewsObject.GetType() == typeof(EwsGetEmailQueue))
                    return await GetEmailAsync((EwsGetEmailQueue)ewsObject, token);
                if (ewsObject.GetType() == typeof(EwsGetFolderQueue))
                    return await GetFolderAsync((EwsGetFolderQueue)ewsObject, token);
                if (ewsObject.GetType() == typeof(EwsDeleteFolderQueue))
                    return await DeleteFolderAsync((EwsDeleteFolderQueue)ewsObject, token);

                throw new Exception("Ews email callback type not configured");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        public async Task<byte[]> DownloadEmailAsync(string uniqueId, EwsCredsQueue ewsCredsQueue, CancellationToken token)
        {
            _logger.Debug("uniqueid: " + uniqueId);
            _logger.Debug(ewsCredsQueue);

            byte[] content = null;

            try
            {
                token.ThrowIfCancellationRequested();

                var ewsWrapper = await GetEwsWrapperAsync(ewsCredsQueue, token);

                var email = await Task.Run(() => ewsWrapper.GetEmail(uniqueId), token);
                _logger.Debug(email);

                content = email.Content;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return content;
        }

        public async Task<List<IExchAttachment>> DownloadAttachmentsAsync(EwsEmailQueue ewsEmailQueue, EwsCredsQueue ewsCredsQueue, CancellationToken token)
        {
            _logger.Debug(ewsEmailQueue);
            _logger.Debug(ewsCredsQueue);

            var attachments = new List<IExchAttachment>();

            try
            {
                token.ThrowIfCancellationRequested();

                var ewsWrapper = await GetEwsWrapperAsync(ewsCredsQueue, token);

                var email = await Task.Run(() => ewsWrapper.GetEmail(ewsEmailQueue.UniqueId), token);
                _logger.Debug(email);

                return email.Attachments;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return attachments;
        }

        private static async Task<EwsResultEmailQueue> GetEmailAsync(
            EwsGetEmailQueue ewsEmailGetQueue, 
            CancellationToken token)
        {
            _logger.Debug(ewsEmailGetQueue);

            var ewsResult = new EwsResultEmailQueue();

            try
            {
                token.ThrowIfCancellationRequested();

                var ewsWrapper = await GetEwsWrapperAsync(ewsEmailGetQueue.Creds, token);
                    
                ExchEmail email = null;
                int waitTime = 0;
                while (true)
                {
                    email = await Task.Run(() => ewsWrapper.GetEmail(ewsEmailGetQueue.UniqueId), token);
                    if (email != null) break;

                    if (RetryTimeoutExpired(ewsEmailGetQueue.MaxWaitTimeSeconds, waitTime)) return null;

                    waitTime++;
                }
                _logger.Debug(email);

                if (email == null) throw new Exception("Email not found");

                ewsResult.Emails = new EwsEmailQueue[] {
                    new EwsEmailQueue()
                    {
                        FromName = email.FromName,
                        ToNames = email.ToNames,
                        CcNames = email.CcNames,
                        SentDate = email.Sent.ToString(),
                        ReceivedDate = email.Received.ToString(),
                        Subject = email.Subject,
                        UniqueId = email.UniqueId
                    } };
            }
            catch (Exception ex)
            {
                ewsResult.ErrorResult = new ErrorResultQueue(ex);
                _logger.Error(ex);
            }

            return ewsResult;
        }

        private static async Task<EwsResultEmailQueue> GetEmailsAsync(EwsGetEmailsQueue ewsEmailsGetQueue, CancellationToken token)
        {
            _logger.Debug(ewsEmailsGetQueue);

            var ewsResultEmailQueue = new EwsResultEmailQueue();

            try
            {
                token.ThrowIfCancellationRequested();

                EwsFolderQueue ewsFolder= null;
                
                if (!string.IsNullOrEmpty(ewsEmailsGetQueue.FolderMessageIdBase64))
                {
                    var result = await _queueServiceEws.GetMessageAsync(
                        Helpers.Base64ConvertFrom(ewsEmailsGetQueue.FolderMessageIdBase64), 
                        token,
                        false);

                    _logger.Debug(result);

                    if (result == null) throw new Exception("Folder not found");
                    var ewsFolderResult = (EwsResultFolderQueue)result;                    
                    ewsFolder = ewsFolderResult.Folder;
                }
                else if (ewsEmailsGetQueue.Folder != null)
                {
                    ewsFolder = ewsEmailsGetQueue.Folder;    
                }
                else
                {
                    throw new Exception("Folder ID not provided");
                }
                _logger.Debug(ewsFolder);

                var ewsWrapper = await GetEwsWrapperAsync(ewsEmailsGetQueue.Creds, token);

                var foundEmails = new List<ExchEmail>();
                int waitTime = 0;
                while (true)
                {
                    foundEmails = await Task.Run(() => (List<ExchEmail>)ewsWrapper.GetEmails(
                        new ExchFolder()
                        {
                            UniqueId = ewsFolder.UniqueId,
                            Name = ewsFolder.Name
                        }), token);
                    if (foundEmails.Count > 0) break;

                    if (RetryTimeoutExpired(ewsEmailsGetQueue.MaxWaitTimeSeconds, waitTime)) return null;

                    waitTime++;
                }
                
                foreach (var foundEmail in foundEmails)
                    _logger.Debug(foundEmail);

                if (foundEmails.Count > 0)
                {
                    var emails = new List<EwsEmailQueue>();

                    foreach (var foundEmail in foundEmails)
                    {
                        emails.Add(new EwsEmailQueue()
                        {
                            Subject = foundEmail.Subject,
                            FromName = foundEmail.FromName,
                            ToNames = foundEmail.ToNames,
                            CcNames = foundEmail.CcNames,
                            SentDate = foundEmail.Sent.ToString(),
                            ReceivedDate = foundEmail.Received.ToString(),
                            UniqueId = foundEmail.UniqueId,
                            EwsFolder = ewsFolder
                        });
                    }

                    ewsResultEmailQueue.Emails = emails.ToArray();
                }
            }
            catch (Exception ex)
            {
                ewsResultEmailQueue.ErrorResult = new ErrorResultQueue(ex);
                _logger.Error(ex);
            }

            return ewsResultEmailQueue;
        }

        private static async Task<EwsResultFolderQueue> GetFolderAsync(EwsGetFolderQueue ewsFolderGetQueue, CancellationToken token)
        {
            _logger.Debug(ewsFolderGetQueue);

            var ewsResultFolder = new EwsResultFolderQueue();
            ewsResultFolder.Folder = new EwsFolderQueue();

            try
            {
                token.ThrowIfCancellationRequested();

                var ewsWrapper = await GetEwsWrapperAsync(ewsFolderGetQueue.Creds, token);
                
                if (string.IsNullOrEmpty(ewsFolderGetQueue.FolderPath)) throw new Exception("Folderpath not provided");

                ExchFolder exchFolder = null;
                int waitTime = 0;
                while (true)
                {
                    exchFolder = await Task.Run(() => ewsWrapper.GetFolder(
                        ewsFolderGetQueue.FolderPath.Split(FOLDERPATHDIVIDER), 
                        ewsFolderGetQueue.MailboxEmailAddress,
                        10), token);
                    if (exchFolder != null) break;
                    _logger.Debug("Result is null");

                    if (RetryTimeoutExpired(ewsFolderGetQueue.MaxWaitTimeSeconds, waitTime)) return null;

                    waitTime++;
                }
                _logger.Debug(exchFolder);

                ewsResultFolder.Folder = new EwsFolderQueue(exchFolder);
                _logger.Debug(ewsResultFolder.Folder);
            }
            catch (Exception ex)
            {
                ewsResultFolder.ErrorResult = new ErrorResultQueue(ex);
                _logger.Error(ex);
            }

            return ewsResultFolder;
        }

        private static async Task<EwsResultFolderQueue> DeleteFolderAsync(EwsDeleteFolderQueue ewsFolderDeleteQueue, CancellationToken token)
        {
            _logger.Debug(ewsFolderDeleteQueue);

            var ewsResultFolder = new EwsResultFolderQueue();

            try
            {
                token.ThrowIfCancellationRequested();

                var folderToDelete = new ExchFolder()
                {
                    UniqueId = ewsFolderDeleteQueue.UniqueId,
                    Name = ewsFolderDeleteQueue.FolderName
                };

                if (!string.IsNullOrEmpty(ewsFolderDeleteQueue.MessageIdBase64))
                {
                    var result = await _queueServiceEws.GetMessageAsync(
                        Helpers.Base64ConvertFrom(ewsFolderDeleteQueue.MessageIdBase64),
                        token,
                        false
                        );
                    _logger.Debug(result);

                    if (result == null) throw new Exception("Folder not found");
                    var origEwsResultFolder = (EwsResultFolderQueue)result;

                    folderToDelete.Name = origEwsResultFolder.Folder.Name;
                    folderToDelete.UniqueId = origEwsResultFolder.Folder.UniqueId;
                }

                ewsResultFolder.Folder = new EwsFolderQueue(folderToDelete);

                var ewsWrapper = await GetEwsWrapperAsync(ewsFolderDeleteQueue.Creds, token);

               _logger.Debug($"Deleting folder {folderToDelete.Name}");
                await Task.Run(() => ewsWrapper.DeleteFolder(folderToDelete, false), token);
                _logger.Debug($"Folder {folderToDelete.Name} deleted.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                ewsResultFolder.ErrorResult = new ErrorResultQueue(ex);
            }

            return ewsResultFolder;
        }

        private static bool RetryTimeoutExpired(int maxWaitTimeSeconds, int waitTime)
        {
            var maxWaitTime = RetryMaxWaitTimeSeconds;
            if (maxWaitTimeSeconds > 0)
                maxWaitTime = maxWaitTimeSeconds;

            if (RetryIntervalMilliseconds > 0)
            {
                if (waitTime > (maxWaitTime * 1000) / RetryIntervalMilliseconds) return true;
                Thread.Sleep(RetryIntervalMilliseconds);

                _logger.Trace("WaitTime: " + waitTime);
            }

            return false;
        }

        private static async Task<IEwsWrapper> GetEwsWrapperAsync(EwsCredsQueue ewsCreds, CancellationToken token)
        {
            _logger.Debug(ewsCreds);

            return (IEwsWrapper)await _cacheService.GetConnectionAsync(
                ewsCreds, 
                () =>
                    {
                        return new EwsWrapper(
                            new EwsConnect(
                                ewsCreds.AutodiscoverEmailAddress,
                                    new NetworkCredential(
                                        ewsCreds.Username,
                                        Helpers.Base64ConvertFrom(ewsCreds.PasswordBase64)
                                        ),
                                    EwsTraceFolderPath,
                                    EwsTraceEnabled
                                    ), _logger);
                    },
                token);
        }
    }
}