using LoggerHelper;
using Microsoft.Exchange.WebServices.Data;
using System;

namespace EWS
{
    public class TraceListener : ITraceListener
    {
        private static TraceListener _tracelistener = null;
        private static int _logVersion = 0;
        private static string _traceStartDateTime;
        private static string _traceFolderPath = Environment.CurrentDirectory;
        private static IFileWrapper _fileWrapper;
        private static int _maxLogSizeBytes;
        static ILogger _logger;

        public static TraceListener StartNewLog(string traceFolderPath, IFileWrapper fileWrapper, ILogger logger, int maxLogSizeBytes = 10000000)
        {
            if (logger == null) _logger = LoggerFactory.GetLogger(null);
            else _logger = logger;

            _maxLogSizeBytes = maxLogSizeBytes;

            if (_tracelistener == null)
                _tracelistener = new TraceListener();

            _traceStartDateTime = GetLogDateTime();
            if (!string.IsNullOrEmpty(traceFolderPath))
                _traceFolderPath = traceFolderPath;
            _fileWrapper = fileWrapper;

            return _tracelistener;
        }

        public void Trace(string traceType, string traceContent)
        {
            try
            {
                _fileWrapper.AppendAllText(GetTraceLogFileName(), GetLogDateTime() + " [" + traceType + "] " + traceContent);
                if (_fileWrapper.ReadAllBytes(GetTraceLogFileName()).Length > _maxLogSizeBytes)
                    _logVersion++;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
        }

        private static string GetTraceLogFileName()
        {
            return _traceFolderPath + @"\EwsTraceLog_" +
                    _traceStartDateTime + "_" +
                    _logVersion.ToString() + ".txt";
        }

        private static string GetLogDateTime()
        {
            var currentDateTime = DateTime.Now;
            return (
                currentDateTime.ToShortDateString().Replace(@"/", "") + "_" +
                currentDateTime.Hour +
                currentDateTime.Minute +
                currentDateTime.Second +
                currentDateTime.Millisecond);
        }
    }

    public class LogEntry
    {
        public DateTime TimeStamp { get; set; }
        public string TraceType { get; set; }
        public string Message { get; set; }

        public LogEntry(DateTime timestamp, string tracetype, string message)
        {
            TimeStamp = timestamp;
            TraceType = tracetype;
            Message = message;
        }
    }
}
