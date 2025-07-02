using System;
using System.Collections.Generic;

namespace QueueService
{
    public class QueueRequest
    {
        public string Id { get; set; }
        public object Input { get; }
        public DateTime StartTime { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public TimeSpan Timeout { get; set; } = new TimeSpan(24, 0, 0);

        public QueueRequest(object queueInput)
        {
            Input = queueInput;
            StartTime = DateTime.Now;
        }
    }
}