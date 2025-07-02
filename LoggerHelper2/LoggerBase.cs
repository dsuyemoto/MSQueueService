using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace LoggerHelper
{
    public abstract class LoggerBase : ILogger
    {
        protected readonly object lockobj = new object();

        public abstract void Fatal(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        public abstract void Error(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        public abstract void Error(
            Exception exception,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        public abstract void Warn(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        public abstract void Info(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        public abstract void Debug(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        public abstract void Debug(
            object objectClass,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        public abstract void Trace(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);

        protected virtual void Execute(Action<string> logAction, string message)
        {
            lock (lockobj)
                logAction(message);
        }

        protected virtual string FormatMessage(string appDomain, string message, string context, string memberName, string filePath, int lineNumber)
        {
            if (!string.IsNullOrEmpty(context))
                context = $"[{context}]";
            return $"{appDomain}: [{memberName}, {Path.GetFileName(filePath)}, {lineNumber}]{context} {message}";
        }

        protected static string GetObject(object obj)
        {
            var message = "Logger failed getting object";
            bool failed = true;

            try
            {
                if (obj.GetType().Name == "Byte[]")
                    message = obj.GetType().Name;
                else
                    message = obj.GetType().Name + " " + JsonConvert.SerializeObject(obj, new JsonSerializerSettings());
                failed = false;
            }
            catch (Exception ex)
            {
                message = message + ":" + ex.Message;
            }

            if (failed)
            {
                var properties = new Dictionary<string, string>();

                foreach (var property in obj.GetType().GetProperties())
                {
                    var propertyName = property.PropertyType.Name.ToLower();
                    switch (propertyName)
                    {
                        case "string":
                            properties.Add(property.Name, property.GetValue(obj).ToString());
                            break;
                        case "string[]":
                            properties.Add(property.Name, string.Join(",", (string[])property.GetValue(obj)));
                            break;
                        case "int":
                            properties.Add(property.Name, ((int)property.GetValue(obj)).ToString());
                            break;
                        case "Byte[]":
                            properties.Add(property.Name, property.Name);
                            break;
                        default:
                            properties.Add(property.Name, property.PropertyType.Name);
                            break;
                    }
                }

                message = string.Join(",", properties);
            }

            return message;
        }
    }
}