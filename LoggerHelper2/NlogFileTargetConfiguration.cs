using NLog;
using NLog.Targets;
using System;

namespace LoggerHelper
{
    public class NlogFileTargetConfiguration : NlogTargetConfigurationBase
    {
        string _folderPath;
        int _logFileSizeMB;
        string _fileName;

        public NlogFileTargetConfiguration(
            string folderPath,
            int logFileSizeMB,
            string fileName) : base()
        {
            _folderPath = folderPath;
            _logFileSizeMB = logFileSizeMB;
            _fileName = fileName;
        }

        public override Target CreateTarget(string targetId)
        {
            if (string.IsNullOrEmpty(_folderPath)) throw new Exception("FolderPath is null or empty");

            _folderPath = _folderPath.Replace('\\', '/').TrimEnd('/');

            var fileName = string.Empty;
            if (!string.IsNullOrEmpty(_fileName))
                fileName = _fileName + "_";
            var fileTarget = new FileTarget(targetId)
            {
                FileName = _folderPath + "/" + fileName + "${shortdate}.log",
                Layout = "${longdate} ${level} ${message}  ${exception}",
                CreateDirs = true,
                ConcurrentWrites = false
            };

            if (_logFileSizeMB > 0)
                fileTarget.ArchiveAboveSize = _logFileSizeMB * 1024 * 1024;

            return fileTarget;
        }
    }
}
