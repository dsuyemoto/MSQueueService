using System;
using System.Runtime.CompilerServices;

namespace LoggerHelper
{
    public class ConsoleLogger : LoggerBase
    {
        bool _isEnabled;

        public ConsoleLogger(ConsoleLogConfiguration consoleLogConfiguration)
        {
            _isEnabled = consoleLogConfiguration.IsEnabled;
        }

        public override void Fatal(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isEnabled) return;
            Console.WriteLine("[FATAL] " + message);
        }
        public override void Fatal(
            Exception exception,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (exception == null || !_isEnabled) return;
            var message = exception.Message + Environment.NewLine + exception.StackTrace;
            Error(message, context, memberName, filePath, lineNumber);
        }
        public override void Error(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isEnabled) return;
            Console.WriteLine("[ERROR] " + message);
        }
        public override void Error(
            Exception exception,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (exception == null || !_isEnabled) return;
            var message = exception.Message + Environment.NewLine + exception.StackTrace;
            Error(message, context, memberName, filePath, lineNumber);
        }
        public override void Warn(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isEnabled) return;
            Console.WriteLine("[Warn] " + message);
        }
        public override void Info(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isEnabled) return;
            Console.WriteLine("[INFO] " + message);
        }
        public override void Info(
            object objectClass,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (objectClass == null || !_isEnabled) return;
            Info(GetObject(objectClass), context, memberName, filePath, lineNumber);
        }
        public override void Debug(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isEnabled) return;
            Console.WriteLine($"[DEBUG] {message}");
        }
        public override void Debug(
            object objectClass,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (objectClass == null || !_isEnabled) return;
            Debug(GetObject(objectClass), context, memberName, filePath, lineNumber);
        }
        public override void Trace(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isEnabled) return;
            Console.WriteLine("[TRACE] " + message);
        }
    }
}
