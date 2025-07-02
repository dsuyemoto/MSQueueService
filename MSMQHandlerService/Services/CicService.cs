using LoggerHelper;
using MSMQHandlerService.Models;
using PureConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static QueueService.QueueServiceBase;

namespace MSMQHandlerService.Services
{
    public class CicService : ICicService
    {
        static IQueueServiceCic _queueServiceCic;
        static IQueueServiceServiceNow _queueServiceServiceNow;
        static ICacheService _cacheService;
        static ILogger _logger;

        public CicService(ILogger logger)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            _queueServiceCic = new QueueServiceCic(QueueType.MSMQ, true, _logger);
            _queueServiceServiceNow = new QueueServiceServiceNow(QueueType.MSMQ, true, _logger);
            _cacheService = new CacheService(_logger);
        }

        public CicService(IQueueServiceCic queueServiceCic, IQueueServiceServiceNow queueServiceServiceNow, ICacheService cacheService, ILogger logger)
        {
            _queueServiceCic = queueServiceCic;
            _queueServiceServiceNow = queueServiceServiceNow;
            _cacheService = cacheService;
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;
        }

        public async Task<object> HandlerCallbackAsync(object interaction, CancellationToken token)
        {
            try
            {
                if (interaction is CicInteractionUpdateQueue)
                    return await UpdateInteractionAsync((CicInteractionUpdateQueue)interaction, token);
                if (interaction is CicInteractionGetQueue)
                    return await GetInteractionAsync((CicInteractionGetQueue)interaction, token);

                throw new Exception("Cic interaction callback type not configured");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        private static async Task<CicInteractionResultQueue> GetInteractionAsync(
            CicInteractionGetQueue cicInteractionGetQueue, 
            CancellationToken token)
        {
            _logger.Debug(cicInteractionGetQueue);

            var cicInteractionResult = new CicInteractionResultQueue()
            {
                InteractionId = cicInteractionGetQueue.InteractionId
            };

            try
            {
                var ininInteractionsResource = await GetIninServiceAsync(cicInteractionGetQueue.Creds, token);

                var responses = await ininInteractionsResource.GetInteractionAttributesAsync(
                    new ININGetInteractionAttributesRequestParameters()
                    {
                        InteractionId = cicInteractionGetQueue.InteractionId,
                        Select = string.Join(",", cicInteractionGetQueue.AttributeNames)
                    });

                cicInteractionResult.IsSuccessful = responses.IsSuccessful;
                if (!cicInteractionResult.IsSuccessful)
                    cicInteractionResult.ErrorResult = new ErrorResultQueue(responses.StatusCode.ToString());
                if (responses.InteractionAttributesDataContract != null)
                    cicInteractionResult.Attributes = Helpers.DictionaryToArray(responses.InteractionAttributesDataContract.Attributes);
            }
            catch (Exception ex)
            {
                cicInteractionResult.ErrorResult = new ErrorResultQueue(ex);
            }

            _logger.Debug(cicInteractionResult);

            return cicInteractionResult;
        }

        private async Task<CicInteractionResultQueue> UpdateInteractionAsync(
            CicInteractionUpdateQueue cicInteractionUpdateQueue,
            CancellationToken token)
        {
            _logger.Debug(cicInteractionUpdateQueue);

            var cicInteractionResult = new CicInteractionResultQueue();

            var attributes = new Dictionary<string, string>();

            try
            {
                var result = await _queueServiceCic.GetMessageAsync(
                    Helpers.Base64ConvertFrom(cicInteractionUpdateQueue.MessageIdBase64), 
                    token,
                    false);
                if (result == null) throw new Exception("Interaction not found");

                var origCicInteractionResult = (CicInteractionResultQueue)result;
                
                if (cicInteractionUpdateQueue.Attributes != null && cicInteractionUpdateQueue.Attributes.Length != 0)
                    attributes = Helpers.ArrayToDictionary(cicInteractionUpdateQueue.Attributes);

                await GetSourceAttributesAsync(cicInteractionUpdateQueue.SourceAttributes, attributes, token);

                if (attributes.Count == 0) throw new Exception("No attributes found to update");

                var ininInteractionsResource = await GetIninServiceAsync(cicInteractionUpdateQueue.Creds, token);

                token.ThrowIfCancellationRequested();
                var responses = await ininInteractionsResource.UpdateInteractionAttributesAsync(
                    new ININUpdateInteractionAttributesRequestParameters() { InteractionId = origCicInteractionResult.InteractionId },
                    new ININInteractionAttributesUpdateDataContract() { Attributes = attributes }
                    );

                cicInteractionResult.IsSuccessful = responses.IsSuccessful;
                cicInteractionResult.Attributes = Helpers.DictionaryToArray(attributes);
                cicInteractionResult.InteractionId = origCicInteractionResult.InteractionId;
            }
            catch (Exception ex)
            {
                cicInteractionResult.ErrorResult = new ErrorResultQueue(ex);
            }

            _logger.Debug(cicInteractionResult);

            return cicInteractionResult;
        }

        private async static Task GetSourceAttributesAsync(
            CicServiceNowSourceQueue cicServiceNowSourceQueue,
            Dictionary<string, string> attributes,
            CancellationToken token)
        {
            if (cicServiceNowSourceQueue == null || string.IsNullOrEmpty(cicServiceNowSourceQueue.MessageIdBase64)) return;
            _logger.Debug(cicServiceNowSourceQueue);

            var result = await _queueServiceServiceNow.GetMessageAsync(
                Helpers.Base64ConvertFrom(cicServiceNowSourceQueue.MessageIdBase64), 
                token);

            if (result == null) throw new Exception("No ticket found in queue");

            var serviceNowTicketResultQueue = (ServiceNowResultTicketQueue)result;
            _logger.Debug(serviceNowTicketResultQueue);
            if (serviceNowTicketResultQueue.ErrorResult != null) throw new Exception(serviceNowTicketResultQueue.ErrorResult.Message);

            var nameMappings = Helpers.ArrayToDictionary(cicServiceNowSourceQueue.CicToSnNameMappings);

            if (serviceNowTicketResultQueue.Tickets == null || serviceNowTicketResultQueue.Tickets.Count() == 0) throw new Exception("Ticket not found");

            Helpers.ArrayToDictionary(serviceNowTicketResultQueue.Tickets[0].Fields).ToList().ForEach(sn =>
            {
                nameMappings.ToList().ForEach(nm =>
                {
                    if (nm.Value == sn.Key)
                    {
                        if (attributes.ContainsKey(nm.Key))
                            attributes[nm.Key] = sn.Value;
                        else
                            attributes.Add(nm.Key, sn.Value);
                    }
                });
            });
        }

        private static async Task<IININInteractionsResource> GetIninServiceAsync(
            CicCredsQueue cicCredsQueue,
            CancellationToken token)
        {
            _logger.Debug(cicCredsQueue);

            return (IININInteractionsResource)await _cacheService.GetConnectionAsync(
                cicCredsQueue,
                () =>
                    {
                        var ininInteractionsResource = new ININInteractionsResource(new ININService());
                        ininInteractionsResource.Connect(
                            cicCredsQueue.Servername,
                            cicCredsQueue.Username,
                            cicCredsQueue.PasswordBase64);

                        return ininInteractionsResource;
                    },
                token);
        }
    }
}