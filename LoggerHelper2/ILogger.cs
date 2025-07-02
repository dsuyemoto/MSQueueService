using System;
using System.Runtime.CompilerServices;

namespace LoggerHelper
{
    public interface ILogger
    {
        void Fatal(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Error(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Error(
            Exception exception,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Warn(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Info(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Debug(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Debug(
            object objectClass,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Trace(
            string message, 
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
    }
}