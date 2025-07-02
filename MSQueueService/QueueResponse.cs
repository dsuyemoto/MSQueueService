using System.Collections.Generic;

namespace QueueService
{
    public class QueueResponse
    {
        public string Id { get; set; }
        public object Output { get; set; }
        public List<string> Errors { get; set; }
    }
}