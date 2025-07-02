using System;
using System.Runtime.CompilerServices;

namespace LoggerHelper
{
    public class DummyLogger : LoggerBase
    {
        public override void Fatal(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            
        }
        public override void Error(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            
        }
        public override void Error(
            Exception exception,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            
        }
        public override void Warn(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            
        }
        public override void Info(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {

        }
        public override void Info(
            object objectClass, string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {

        }
        public override void Debug(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            
        }
        public override void Debug(
            object objectClass, string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "", 
            [CallerLineNumber] int lineNumber = 0)
        {
            
        }
        public override void Trace(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Console.WriteLine($"Trace: {message}");
        }
    }
}
