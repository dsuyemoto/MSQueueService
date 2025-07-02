using LoggerHelper;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MSMQHandlerService.Services
{
    public class FileService : IFileService
    {
        ILogger _logger;

        public FileService(ILogger logger)
        {
            if (logger == null)
                _logger = LoggerFactory.GetLogger(null);
            else
                _logger = logger; 
        }

        public async Task<byte[]> ReadAsync(string filePath, CancellationToken token)
        {
            byte[] content = null;
            using (var stream = File.OpenRead(filePath))
            {
                content = new byte[stream.Length];
                var task = await stream.ReadAsync(content, 0, (int)stream.Length, token);
            }

            return content;
        }

        public async Task SaveAsync(byte[] content, string filePath, CancellationToken token)
        {
            using (var stream = File.OpenWrite(filePath))
            {
                await stream.WriteAsync(content, 0, content.Length, token);
            }
        }

        public bool Exists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                _logger.Warn("File path is null or empty.");
                return false;
            }
            try
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
                {
                    _logger.Warn("Directory does not exist: " + fileInfo.DirectoryName);
                    return false;
                }

                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error checking file existence: " + filePath);
                return false;
            }
        }

        public void Delete(string filePath)
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException ex)
            {
                _logger.Error(ex, "Error deleting file: " + filePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Error(ex, "Unauthorized access to file: " + filePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error deleting file: " + filePath);
            }
        }
    }
}