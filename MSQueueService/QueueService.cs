using LoggerHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QueueService
{
    public class QueueService
    {
        ConcurrentQueue<QueueRequest> _queue = new ConcurrentQueue<QueueRequest>();
        List<QueueResponse> _responses = new List<QueueResponse>();       
        ConcurrentDictionary<string, QueueItemStatus> _requestStatus = new ConcurrentDictionary<string, QueueItemStatus>();
        QueueRequest[] _queueRequests;
        ILogger _logger;

        public enum QueueItemStatus
        {
            Queued,
            Error,
            Timeout,
            Completed,
            None            
        }

        public QueueService()
        {
            _logger = LoggerFactory.GetLogger(new ConsoleLogConfiguration("DEBUG", "MSQueueService", true));
        }

        public Task StartHandler(Func<object, object> callbackFunction, CancellationToken token)
        {
            return Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    QueueRequest queueRequest = null;

                    try
                    {
                        if (!_queue.IsEmpty)
                        {
                            _queueRequests = _queue.ToArray();

                            if (_queue.TryDequeue(out queueRequest))
                            {
                                if ((DateTime.Now - queueRequest.StartTime) > queueRequest.Timeout)
                                    throw new TimeoutException();

                                var result = callbackFunction(queueRequest.Input);

                                if (result != null)
                                {
                                    _responses.Add(new QueueResponse() { Output = result });
                                    _requestStatus.AddOrUpdate(queueRequest.Id, QueueItemStatus.Completed, (i, s) => QueueItemStatus.Completed);
                                }
                                else
                                {
                                    _queue.Enqueue(queueRequest);
                                    _logger.Warn("ID:" + queueRequest.Id + " Callback Result Null");
                                }
                            }
                        }
                    }
                    catch (TimeoutException)
                    {
                        _responses.Add(new QueueResponse() { Id = queueRequest.Id, Errors = queueRequest.Errors });
                        _requestStatus.AddOrUpdate(queueRequest.Id, QueueItemStatus.Timeout, (i, s) => QueueItemStatus.Timeout);
                    }
                    catch (InvalidOperationException)
                    {

                    }
                    catch (Exception ex)
                    {
                        var error = ex.Message + "\\r\\n" + ex.StackTrace;

                        if (queueRequest != null)
                        {
                            if (!queueRequest.Errors.Contains(error))
                            {
                                queueRequest.Errors.Add(error);
                                _requestStatus.AddOrUpdate(queueRequest.Id, QueueItemStatus.Error, (i, s) => QueueItemStatus.Error);
                            }

                            _queue.Enqueue(queueRequest);
                        }
                    }
                }
            });
        }

        public string AddRequest(QueueRequest queueRequest)
        {
            var id = Guid.NewGuid().ToString();
            queueRequest.Id = id;
            _queue.Enqueue(queueRequest);

            _requestStatus.AddOrUpdate(id, QueueItemStatus.Queued, (i, s) => QueueItemStatus.Queued);
            _logger.Debug("ID:" + id + " Request " + QueueItemStatus.Queued.ToString());

            return id;
        }

        public async Task<QueueResponse> GetResponseAsync(string id, CancellationToken token)
        {
            return await Task.Run(() =>
            {
                QueueResponse response = null;

                while (response == null && !token.IsCancellationRequested) response = GetResponse(id);

                return response;
            }, token);
        }

        public QueueResponse GetResponse(string id)
        {
            var responses = _responses.ToArray();
            foreach (var response in responses)
                if (response.Id == id)
                {
                    _responses.Remove(response);
                    QueueItemStatus currentStatus;
                    _requestStatus.TryRemove(id, out currentStatus);
                    _logger.Debug("ID:" + id + " Response Removed");
                    return response;
                }

            return null;
        }

        public async Task<QueueRequest> GetRequestAsync(string id, CancellationToken token)
        {
            return await Task.Run(() =>
            {
                QueueRequest request = null;

                while (request == null && !token.IsCancellationRequested) request = GetRequest(id);

                return request;
            }, token);
        }

        public QueueRequest GetRequest(string id)
        {
            foreach (var queueRequest in _queueRequests)
                if (queueRequest.Id == id)
                    return queueRequest;

            return null;
        }

        public QueueItemStatus GetQueueItemStatus(string id)
        {
            QueueItemStatus status = QueueItemStatus.None;
            _requestStatus.TryGetValue(id, out status);

            return status;
        }
    }
}