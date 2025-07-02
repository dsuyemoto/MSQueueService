using LoggerHelper;
using MSMQHandlerService.Models;
using QueueService;
using ServiceNow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static QueueService.QueueServiceBase;
using static ServiceNow.ServiceNowBase;

namespace MSMQHandlerService.Services
{
    public class ServiceNowService : IServiceNowService
    {
        static IQueueServiceImanage _queueServiceImanage;
        static IQueueServiceServiceNow _queueServiceServiceNow;
        static IServiceNowRepository _serviceNowRepository;
        static IFileService _fileService;
        static ILogger _logger;

        const string EMAIL_HEADER = "From: {FROM}\r\nTo: {TO}\r\nCc: {CC}\r\nSubject: {SUBJECT}\r\n\r\n";
        const string FETCHDOCURL = "http://home.lw.com/global/fetchDoc.asp?docNum={DOCNUMBER}&server={SESSION}&FORCEDOCUMENTVERSIONNUMBER={DOCVERSION}";
        const string FETCHDOCURL_HTML = @"[code]<p><a title=""{FETCHDOCURL}"" href=""{FETCHDOCURL}"
                + @""" target=""_blank""><span style=""display: block; -ms-word-wrap: break-word;"">{LINKNAME} ["
                + "{DATABASE} : {DOCNUMBER},{DOCVERSION}]</span></a></p>[/code]";
        const string PLAINTEXT = "text/plain";

        public enum SnFields
        {
            sys_id,
            number
        }

        public static string PlaceholderName { get; set; } = "Loading attachment...please wait";

        public ServiceNowService(ILogger logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            _serviceNowRepository = new ServiceNowRepository();
            _fileService = new FileService(_logger);
            _queueServiceImanage = new QueueServiceImanage(QueueType.MSMQ, true, _logger);
            _queueServiceServiceNow = new QueueServiceServiceNow(QueueType.MSMQ, true, _logger);
        }

        public ServiceNowService(
            IServiceNowRepository serviceNowRepository, 
            IFileService fileService, 
            IQueueServiceImanage queueServiceImanage, 
            IQueueServiceServiceNow queueServiceServiceNow,
            ILogger logger)
        {
            _serviceNowRepository = serviceNowRepository;
            _fileService = fileService;
            _queueServiceImanage = queueServiceImanage;
            _queueServiceServiceNow = queueServiceServiceNow;
            _logger = logger;
        }

        public async Task<object> HandlerCallbackAsync(object snObject, CancellationToken token)
        {
            try
            {
                if (snObject is ServiceNowCreateTicketQueue)
                    return await CreateTicketAsync((ServiceNowCreateTicketQueue)snObject, token);
                if (snObject is ServiceNowGetTicketsQueue)
                    return await GetTicketsAsync((ServiceNowGetTicketsQueue)snObject, token);
                if (snObject is ServiceNowCreateAttachmentQueue)
                    return await CreateAttachmentAsync((ServiceNowCreateAttachmentQueue)snObject, token);
                if (snObject is ServiceNowUpdateTicketQueue)
                    return await UpdateTicketAsync((ServiceNowUpdateTicketQueue)snObject, token);
                if (snObject is ServiceNowGetUserQueue)
                    return await GetUsersAsync((ServiceNowGetUserQueue)snObject, token);
                if (snObject is ServiceNowQueryUserQueue)
                    return await QueryUsersAsync((ServiceNowQueryUserQueue)snObject, token);
                if (snObject is ServiceNowQueryTicketQueue)
                    return await QueryTicketsAsync((ServiceNowQueryTicketQueue)snObject, token);
                if (snObject is ServiceNowQueryGroupQueue)
                    return await QueryGroupsAsync((ServiceNowQueryGroupQueue)snObject, token);
                if (snObject is ServiceNowGetGroupQueue)
                    return await GetGroupsAsync((ServiceNowGetGroupQueue)snObject, token);

                throw new Exception("ServiceNow Callback Type not configured");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        private static async Task<ServiceNowResultTicketQueue> CreateTicketAsync(ServiceNowCreateTicketQueue serviceNowCreateTicket, CancellationToken token)
        {
            _logger.Debug(serviceNowCreateTicket);

            try
            {
                token.ThrowIfCancellationRequested();

                var result = await Task.Run(() => _serviceNowRepository.CreateTicket(
                        new SnTicketCreate(
                            serviceNowCreateTicket.TableName,
                            serviceNowCreateTicket.InstanceUrl,
                            serviceNowCreateTicket.Username,
                            Helpers.Base64ConvertFrom(serviceNowCreateTicket.PasswordBase64),
                            Helpers.ArrayToDictionary(serviceNowCreateTicket.Fields))
                        ), token);

                return new ServiceNowResultTicketQueue(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                return new ServiceNowResultTicketQueue() { ErrorResult = new ErrorResultQueue(ex) };
            }
        }

        private static async Task<ServiceNowResultTicketQueue> GetTicketsAsync(
            ServiceNowGetTicketsQueue serviceNowGetTicketQueue,
            CancellationToken token)
        {
            _logger.Debug(serviceNowGetTicketQueue);

            try
            {
                token.ThrowIfCancellationRequested();

                string[] resultNames = null;
                if (serviceNowGetTicketQueue._ResultNames != null)
                    resultNames = serviceNowGetTicketQueue._ResultNames.Split(',');

                var snResult = await Task.Run(() => _serviceNowRepository.GetTicket(
                        new SnTicketGet(
                            serviceNowGetTicketQueue.TableName,
                            serviceNowGetTicketQueue.InstanceUrl,
                            serviceNowGetTicketQueue.Username,
                            Helpers.Base64ConvertFrom(serviceNowGetTicketQueue.PasswordBase64),
                            Helpers.ArrayToDictionary(serviceNowGetTicketQueue.Fields),
                            resultNames,
                            serviceNowGetTicketQueue._SysId)), token);

                return new ServiceNowResultTicketQueue(snResult);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                return new ServiceNowResultTicketQueue() { ErrorResult = new ErrorResultQueue(ex) };
            }
        }

        private static async Task<ServiceNowResultTicketQueue> QueryTicketsAsync(
            ServiceNowQueryTicketQueue serviceNowQueryTicketQueue, 
            CancellationToken token)
        {
            return await GetTicketsAsync(new ServiceNowGetTicketsQueue()
            {
                Fields = serviceNowQueryTicketQueue.Fields,
                InstanceUrl = serviceNowQueryTicketQueue.InstanceUrl,
                _MaxRetries = serviceNowQueryTicketQueue._MaxRetries,
                PasswordBase64 = serviceNowQueryTicketQueue.PasswordBase64,
                _ResultNames = serviceNowQueryTicketQueue._ResultNames,
                TableName = serviceNowQueryTicketQueue.TableName,
                Username = serviceNowQueryTicketQueue.Username
            }, token) ;
        }

        private static async Task<ServiceNowResultUserQueue> GetUsersAsync(
            ServiceNowGetUserQueue serviceNowGetUserQueue,
            CancellationToken token)
        {
            _logger.Debug(serviceNowGetUserQueue);

            try
            {
                token.ThrowIfCancellationRequested();

                string[] resultNames = null;
                if (serviceNowGetUserQueue._ResultNames != null)
                    resultNames = serviceNowGetUserQueue._ResultNames.Split(',');
                var fields = Helpers.ArrayToDictionary(serviceNowGetUserQueue.Fields);

                var result = await Task.Run(() => _serviceNowRepository.GetUser(
                        new SnUserGet(
                            serviceNowGetUserQueue.InstanceUrl,
                            serviceNowGetUserQueue.Username,
                            Helpers.Base64ConvertFrom(serviceNowGetUserQueue.PasswordBase64),
                            fields,
                            resultNames,
                            serviceNowGetUserQueue._SysId)), token);

                return new ServiceNowResultUserQueue(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                return new ServiceNowResultUserQueue() { ErrorResult = new ErrorResultQueue(ex) };
            }
        }

        private static async Task<ServiceNowResultUserQueue> QueryUsersAsync(
            ServiceNowQueryUserQueue serviceNowQueryUserQueue, 
            CancellationToken token)
        {
            return await GetUsersAsync(
                new ServiceNowGetUserQueue()
                {
                    Fields = serviceNowQueryUserQueue.Fields,
                    InstanceUrl = serviceNowQueryUserQueue.InstanceUrl,
                    Username = serviceNowQueryUserQueue.Username,
                    PasswordBase64 = serviceNowQueryUserQueue.PasswordBase64,
                    _ResultNames = serviceNowQueryUserQueue._ResultNames,
                    _MaxRetries = serviceNowQueryUserQueue._MaxRetries
                }, token);
        }

        private static async Task<ServiceNowResultGroupQueue> GetGroupsAsync(
            ServiceNowGetGroupQueue serviceNowGetGroupQueue,
            CancellationToken token)
        {
            _logger.Debug(serviceNowGetGroupQueue);

            try
            {
                token.ThrowIfCancellationRequested();

                string[] resultNames = null;
                if (serviceNowGetGroupQueue._ResultNames != null)
                    resultNames = serviceNowGetGroupQueue._ResultNames.Split(',');
                var fields = Helpers.ArrayToDictionary(serviceNowGetGroupQueue.Fields);

                var result = await Task.Run(() => _serviceNowRepository.GetGroup(
                        new SnGroupGet(
                            serviceNowGetGroupQueue.InstanceUrl,
                            serviceNowGetGroupQueue.Username,
                            Helpers.Base64ConvertFrom(serviceNowGetGroupQueue.PasswordBase64),
                            fields,
                            resultNames,
                            serviceNowGetGroupQueue._SysId)), token);

                return new ServiceNowResultGroupQueue(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                return new ServiceNowResultGroupQueue() { ErrorResult = new ErrorResultQueue(ex) };
            }
        }

        private static async Task<ServiceNowResultGroupQueue> QueryGroupsAsync(
            ServiceNowQueryGroupQueue serviceNowQueryGroupQueue,
            CancellationToken token)
        {
            return await GetGroupsAsync(
                new ServiceNowGetGroupQueue()
                {
                    Fields = serviceNowQueryGroupQueue.Fields,
                    InstanceUrl = serviceNowQueryGroupQueue.InstanceUrl,
                    Username = serviceNowQueryGroupQueue.Username,
                    PasswordBase64 = serviceNowQueryGroupQueue.PasswordBase64,
                    _ResultNames = serviceNowQueryGroupQueue._ResultNames,
                    _MaxRetries = serviceNowQueryGroupQueue._MaxRetries
                }, token);
        }

        private static async Task<ServiceNowResultTicketQueue> UpdateTicketAsync(
            ServiceNowUpdateTicketQueue serviceNowTicketUpdateQueue,
            CancellationToken token)
        {
            _logger.Debug(serviceNowTicketUpdateQueue);

            try
            {
                token.ThrowIfCancellationRequested();

                if (string.IsNullOrEmpty(serviceNowTicketUpdateQueue.MessageIdBase64)) throw new Exception("ServiceNow MessageIdBase64 not provided");

                var sysId = await GetSysIdAsync(serviceNowTicketUpdateQueue.MessageIdBase64, token);
                if (string.IsNullOrEmpty(sysId)) throw new Exception("Ticket number not found");

                var fields = Helpers.ArrayToDictionary(serviceNowTicketUpdateQueue.Fields);

                if (serviceNowTicketUpdateQueue.InsertImanageLink != null)
                {
                    var result = await _queueServiceImanage.GetMessageAsync(
                        Helpers.Base64ConvertFrom(serviceNowTicketUpdateQueue.InsertImanageLink.ImanageMessageIdBase64),
                        token,
                        false
                        );
                    if (result == null) throw new Exception("Imanage Document not found");

                    var imanageResultDocumentQueue = (ImanageResultDocumentQueue)result;
                    if (imanageResultDocumentQueue.Documents.Length == 0) throw new Exception("Imanage Document not found");
                    var imanageDocumentResultQueue = imanageResultDocumentQueue.Documents[0];

                    fields.Add(
                        serviceNowTicketUpdateQueue.InsertImanageLink.CommunicationsFieldName,
                        CreateLinkBody(
                            imanageDocumentResultQueue.Number,
                            imanageDocumentResultQueue.Version,
                            imanageDocumentResultQueue.Database,
                            imanageDocumentResultQueue.Session,
                            serviceNowTicketUpdateQueue.InsertImanageLink.EmailProperties
                        ));
                }

                var snResult = await Task.Run(() => _serviceNowRepository.UpdateTicket(
                    new SnTicketUpdate(
                        serviceNowTicketUpdateQueue.TableName,
                        serviceNowTicketUpdateQueue.InstanceUrl,
                        serviceNowTicketUpdateQueue.Username,
                        Helpers.Base64ConvertFrom(serviceNowTicketUpdateQueue.PasswordBase64),
                        sysId,
                        fields)), token);

                return new ServiceNowResultTicketQueue(snResult);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                return new ServiceNowResultTicketQueue() { ErrorResult = new ErrorResultQueue(ex) };
            }
        }

        private static async Task<ServiceNowResultAttachmentQueue> CreateAttachmentAsync(
            ServiceNowCreateAttachmentQueue serviceNowCreateAttachmentQueue, 
            CancellationToken token)
        {
            _logger.Debug(serviceNowCreateAttachmentQueue);

            var serviceNowResultAttachmentQueue = new ServiceNowResultAttachmentQueue();
            serviceNowResultAttachmentQueue.InstanceUrl = serviceNowCreateAttachmentQueue.InstanceUrl;
            serviceNowResultAttachmentQueue.TableName = serviceNowCreateAttachmentQueue.TableName;
            var ticketSysId = serviceNowCreateAttachmentQueue.TicketSysId;
            var fileName = string.Empty;
            
            var placeHolderService = new PlaceHolderService(_serviceNowRepository);

            try
            {
                token.ThrowIfCancellationRequested();
     
                if (!string.IsNullOrEmpty(serviceNowCreateAttachmentQueue.TicketMessageIdBase64))
                    ticketSysId = await GetSysIdAsync(serviceNowCreateAttachmentQueue.TicketMessageIdBase64, token);
                if (string.IsNullOrEmpty(ticketSysId)) throw new Exception("Ticket number not found");
                
                if (!string.IsNullOrEmpty(serviceNowCreateAttachmentQueue.FileName))
                    fileName = string.Join("", serviceNowCreateAttachmentQueue.FileName.Split(Path.GetInvalidFileNameChars()));

                if (!string.IsNullOrEmpty(serviceNowCreateAttachmentQueue.SourceContent.ImanageMessageIdBase64))
                {
                    var result = await _queueServiceImanage.GetMessageAsync(
                        Helpers.Base64ConvertFrom(
                            serviceNowCreateAttachmentQueue.SourceContent.ImanageMessageIdBase64
                            ),
                        token,
                        false);

                    if (result == null) throw new Exception("Imanage document not found");

                    var imanageResultDocumentQueue = (ImanageResultDocumentQueue)result;
                    _logger.Debug(imanageResultDocumentQueue);
                    if (imanageResultDocumentQueue.Documents.Length == 0) throw new Exception("Imanage document not found");

                    var document = imanageResultDocumentQueue.Documents[0];
                    serviceNowCreateAttachmentQueue.SourceContent.BytesBase64 = document.ImanageNrl.ContentBytesBase64;
                    fileName = document.ImanageNrl.FileName;
                    serviceNowCreateAttachmentQueue.MimeType = PLAINTEXT;
                }
                else if (!string.IsNullOrEmpty(serviceNowCreateAttachmentQueue.SourceContent.SourceFilePath))
                {
                    var content = await _fileService.ReadAsync(serviceNowCreateAttachmentQueue.SourceContent.SourceFilePath, token);
                    if (content == null) throw new Exception("Could not retrieve file");
                    serviceNowCreateAttachmentQueue.SourceContent.BytesBase64 = Convert.ToBase64String(content);
                }

                if (serviceNowCreateAttachmentQueue.SourceContent.BytesBase64 == null) 
                    throw new Exception("No source content found");

                await placeHolderService.Create(
                    serviceNowCreateAttachmentQueue.TableName,
                    serviceNowCreateAttachmentQueue.InstanceUrl,
                    serviceNowCreateAttachmentQueue.Username,
                    serviceNowCreateAttachmentQueue.PasswordBase64,
                    serviceNowCreateAttachmentQueue.MimeType,
                    serviceNowCreateAttachmentQueue.TicketSysId,
                    $"FileName : {fileName}\r\n" +
                    $"Content-Type : {serviceNowCreateAttachmentQueue.MimeType}\r\n" +
                    $"Start Time: {DateTime.Now}", 
                    PlaceholderName,
                    token);

                var snResult = await Task.Run(() => _serviceNowRepository.CreateAttachment(
                    new SnAttachmentCreate(
                        serviceNowCreateAttachmentQueue.TableName,
                        serviceNowCreateAttachmentQueue.InstanceUrl,
                        serviceNowCreateAttachmentQueue.Username,
                        Helpers.Base64ConvertFrom(serviceNowCreateAttachmentQueue.PasswordBase64),
                        fileName,
                        serviceNowCreateAttachmentQueue.MimeType,
                        ticketSysId,
                        Convert.FromBase64String(serviceNowCreateAttachmentQueue.SourceContent.BytesBase64))
                    ), token);
                _logger.Debug(snResult);

                serviceNowResultAttachmentQueue = new ServiceNowResultAttachmentQueue(snResult);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);

                serviceNowResultAttachmentQueue.ErrorResult = new ErrorResultQueue(ex);

                await placeHolderService.Delete(token);

                await placeHolderService.Create(
                    serviceNowCreateAttachmentQueue.TableName,
                    serviceNowCreateAttachmentQueue.InstanceUrl,
                    serviceNowCreateAttachmentQueue.Username,
                    serviceNowCreateAttachmentQueue.PasswordBase64,
                    serviceNowCreateAttachmentQueue.MimeType,
                    serviceNowCreateAttachmentQueue.TicketSysId,
                    $"FileName : {fileName}\r\n" +
                    $"Content-Type : {serviceNowCreateAttachmentQueue.MimeType}\r\n" +
                    $"Error: {ex.Message}",
                    "Error adding attachment",
                    token);
            }
            finally
            {
                await placeHolderService.Delete(token);
            }

            return serviceNowResultAttachmentQueue;
        }

        private static string CreateLinkBody(
            string docnumber,
            string docversion,
            string database,
            string session,
            ImanageEmailPropertiesQueue imanageEmailProperties
            )
        {
            var linkBody = GetHeaderHtml(imanageEmailProperties) +
                    GetFetchDocUrlHtml(docnumber, docversion, database, session, imanageEmailProperties.Subject);
            _logger.Debug("linkbody: " + linkBody);

            return linkBody;
        }

        private static string GetHeaderHtml(ImanageEmailPropertiesQueue imanageEmailProperties)
        {
            _logger.Debug(imanageEmailProperties);

            var header = string.Empty;

            if (imanageEmailProperties != null)
            {
                header = EMAIL_HEADER.Replace("{FROM}", imanageEmailProperties.FromName)
                                .Replace("{TO}", imanageEmailProperties.ToNames)
                                .Replace("{SUBJECT}", imanageEmailProperties.Subject);

                if (string.IsNullOrEmpty(imanageEmailProperties.CcNames))
                    header = header.Replace("Cc: {CC}\r\n", "");
                else
                    header = header.Replace("{CC}", imanageEmailProperties.CcNames);
            }
            _logger.Debug("header: " + header);

            return header;
        }

        private static string GetFetchDocUrlHtml(string docNumber, string docVersion, string database, string session, string linkName)
        {
            var fetchDocUrlHtml = FETCHDOCURL_HTML.Replace("{FETCHDOCURL}", FETCHDOCURL)
                                .Replace("{DOCNUMBER}", docNumber)
                                .Replace("{DOCVERSION}", docVersion)
                                .Replace("{DATABASE}", database)
                                .Replace("{SESSION}", session)
                                .Replace("{LINKNAME}", linkName);
            _logger.Debug("fetchDocUrlHtml: " + fetchDocUrlHtml);

            return fetchDocUrlHtml;
        }

        private static async Task<string> GetSysIdAsync(string messageIdBase64, CancellationToken token)
        {
            var messageId = Helpers.Base64ConvertFrom(messageIdBase64);
            _logger.Debug("messageId: " + messageId);

            var snResult = await _queueServiceServiceNow.GetMessageAsync(
                messageId,
                token,
                false
                );
            if (snResult == null) throw new Exception("Ticket number not found");

            var serviceNowTicketResultQueue = (ServiceNowResultTicketQueue)snResult;
            _logger.Debug(serviceNowTicketResultQueue);

            if (serviceNowTicketResultQueue == null ||
                    serviceNowTicketResultQueue.Tickets == null ||
                    serviceNowTicketResultQueue.Tickets.Length == 0) return null;

            var snFields = Helpers.ArrayToDictionary(serviceNowTicketResultQueue.Tickets[0].Fields);

            if (snFields.ContainsKey(SnField.sys_id.ToString())) return (string)snFields[SnField.sys_id.ToString()];

            return null;
        }

        public static string[][] FilterResults(Dictionary<string,string> snFields, string[] resultNames)
        {
            var snFilteredFields = snFields;

            if (resultNames != null)
            {
                snFilteredFields = new Dictionary<string, string>();
                foreach (var snfield in snFields)
                    if (resultNames.Contains(snfield.Key))
                        snFilteredFields.Add(snfield.Key, snfield.Value);
            }

            return Helpers.DictionaryToArray(snFilteredFields);
        }
    }
}