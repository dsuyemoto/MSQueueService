using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QueueServiceWebApp.Models
{
    public class LogView
    {
        public DateTime Time { get; set; }
        public string LogLevel { get; set; }
        public string Line { get; set; }
        public string Message { get; set; }
    }
}