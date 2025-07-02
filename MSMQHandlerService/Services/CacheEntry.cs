using System;

namespace MSMQHandlerService.Services
{
    public class CacheEntry
    {
        public DateTime StartTime { get; set; }
        public object Service { get; set; }
    }
}
