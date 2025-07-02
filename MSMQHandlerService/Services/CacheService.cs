using LoggerHelper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public class CacheService : ICacheService
    {
        ILogger _logger;
        Dictionary<string, CacheEntry> _cache;

        public TimeSpan ConnectionTimeout { get; set; } = new TimeSpan(24, 0, 0);

        public CacheService(ILogger logger)
        {
            _logger = logger;
            _cache = new Dictionary<string, CacheEntry>();
        }

        private bool IsConnected(string connectionKey)
        {
            if (_cache.ContainsKey(connectionKey))
            {
                var cacheEntry = _cache[connectionKey];
                var cacheConnectionLength = DateTime.Now.Subtract(cacheEntry.StartTime);

                var expired = TimeSpan.Compare(cacheConnectionLength, ConnectionTimeout) > 0 ? true : false;
                if (!expired)
                {
                    _logger.Trace($"Service connection found: {connectionKey}");
                    return true;
                }
                _logger.Trace($"Service connection expired: {connectionKey}");

                _cache.Remove(connectionKey);
                _logger.Trace($"Service connection removed: {connectionKey}");
            }

            return false;
        }

        private void Add(string connectionKey, object service)
        {
            if (_cache.ContainsKey(connectionKey))
                _cache.Remove(connectionKey);

            _cache.Add(
                connectionKey, 
                new CacheEntry() 
                {
                    Service = service, 
                    StartTime = DateTime.Now 
                });
        }

        private CacheEntry Get(string connectionKey)
        {
            if (_cache.ContainsKey(connectionKey))
                return _cache[connectionKey];
            else
                return null;
        }

        public async Task<object> GetConnectionAsync(
            object credentials, 
            Func<object> initializer,
            CancellationToken token)
        {
            var connectionKey = GetConnectionKey(credentials);
            object service;

            if (IsConnected(connectionKey))
            {
                var cacheEntry = Get(connectionKey);
                service = cacheEntry.Service;
                _logger.Trace($"Service retrieved: {connectionKey}");
            }
            else
            {
                service = await Task.Run(() => initializer(), token);
                Add(connectionKey, service);
                _logger.Trace($"Service created: {connectionKey}");
            }

            if (service == null) throw new Exception($"No service could be created in cache: {connectionKey}");

            return service;
        }

        private static string GetConnectionKey(object credentials)
        {
            var properties = credentials.GetType().GetProperties();
            var propertyValues = new List<string>();

            foreach (var property in properties)
                propertyValues.Add(property.GetValue(credentials).ToString());

            return string.Join("", propertyValues);
        }
    }
}