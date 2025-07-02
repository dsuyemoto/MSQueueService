using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using System;
using System.Runtime.CompilerServices;

namespace LoggerHelper
{
    public class NlogLoggerWrapper : LoggerBase
    {
        NLog.Logger _nlog;
        string _appDomain;
        string _logLevel;
        bool _isEnabled = true;
        NlogTargetConfigurationBase[] _targetConfigurations;

        public NlogLoggerWrapper(NlogLogConfiguration nlogLogConfiguration)
        {
            if (nlogLogConfiguration == null) throw new Exception("NlogLogConfiguration is null");

            InternalLogger.LogFile = nlogLogConfiguration.InternalLoggingFile;
            InternalLogger.LogToConsole = nlogLogConfiguration.InternalLoggingToConsole;
            InternalLogger.LogLevel = GetLogLevel(nlogLogConfiguration.InternalLogLevel);
            _appDomain = nlogLogConfiguration.AppDomain;
            _logLevel = nlogLogConfiguration.LogLevel;
            _targetConfigurations = nlogLogConfiguration.TargetConfigurations;
        }

        public override void Fatal(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!_isEnabled || string.IsNullOrEmpty(message)) return;

            InitializeTargets();
            Execute((m) => _nlog.Fatal(m), FormatMessage(_appDomain, message, context, memberName, filePath, lineNumber));  
        }

        public override void Error(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            InitializeTargets();
            Execute((m) => _nlog.Error(m), FormatMessage(_appDomain, message, context, memberName, filePath, lineNumber));
        }

        public override void Error(
            Exception exception,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            //TODO: move to abstract class
            if (exception == null) return;

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
            InitializeTargets();
            Execute((m) => _nlog.Warn(m), FormatMessage(_appDomain, message, context, memberName, filePath, lineNumber));
        }

        public override void Info(
            string message,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            InitializeTargets();
            Execute((m) => _nlog.Info(m), FormatMessage(_appDomain, message, context, memberName, filePath, lineNumber));
        }

        public override void Debug(
            string message, 
            string context = "", 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            InitializeTargets();
            Execute((m) => _nlog.Debug(m), FormatMessage(_appDomain, message, context, memberName, filePath, lineNumber));
        }
        public override void Debug(
            object objectClass,
            string context = "",
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (objectClass == null) return;

            Debug(GetObject(objectClass), context, memberName, filePath, lineNumber);
        }

        public override void Trace(
            string message, 
            string context = "", 
            [CallerMemberName] string memberName = "", 
            [CallerFilePath] string filePath = "", 
            [CallerLineNumber] int lineNumber = 0)
        {
            InitializeTargets();
            Execute((m) => _nlog.Trace(m), FormatMessage(_appDomain, message, context, memberName, filePath, lineNumber));
        }

        public static LogLevel GetLogLevel(string loggerLevel)
        {
            switch (loggerLevel.ToLower())
            {
                case "trace":
                    return LogLevel.Trace;
                case "debug":
                    return LogLevel.Debug;
                case "info":
                    return LogLevel.Info;
                case "warn":
                    return LogLevel.Warn;
                case "error":
                    return LogLevel.Error;
                case "fatal":
                    return LogLevel.Fatal;
                case "none":
                    return LogLevel.Off;
                default:
                    InternalLogger.Error("Log level not defined: " + loggerLevel);
                    return LogLevel.Off;
            }
        }

        private void ConfigureTarget(LoggingConfiguration loggingConfiguration, Target target)
        {
            loggingConfiguration.AddTarget(target);
            loggingConfiguration.AddRule(
                GetLogLevel(_logLevel),
                LogLevel.Fatal,
                target);
        }

        private void InitializeTargets()
        {
            if (!_isEnabled || _targetConfigurations == null || _targetConfigurations.Length == 0) 
            { 
                _nlog = _nlog.Factory.CreateNullLogger();
                return;
            }

            var config = new LoggingConfiguration();

            foreach (var targetConfiguration in _targetConfigurations)
            {
                if (targetConfiguration is NlogEventTargetConfiguration eventTarget)
                    ConfigureTarget(config, eventTarget.CreateTarget($"target1", _appDomain));
                else if (targetConfiguration is NlogFileTargetConfiguration fileTarget)
                    ConfigureTarget(config, fileTarget.CreateTarget($"target2"));
                else if (targetConfiguration is NlogConsoleTargetConfiguration consoleTarget)
                    ConfigureTarget(config, consoleTarget.CreateTarget($"target3"));
            }

            LogManager.Configuration = config;
            _nlog = LogManager.GetCurrentClassLogger();
        }

    }
}
