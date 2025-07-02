using System.IO;
using System.Threading;

namespace EWS
{
    public class FileWrapper : IFileWrapper
    {
        static ReaderWriterLock _readerWriterLock = new ReaderWriterLock();

        public static int LockTimeoutMillisec { get; set; }

        public FileWrapper(int lockTimeMillisec = 5000)
        {
            LockTimeoutMillisec = lockTimeMillisec;
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void AppendAllText(string path, string contents)
        {
            try
            {
                _readerWriterLock.AcquireWriterLock(LockTimeoutMillisec);
                File.AppendAllText(path, contents);
            }
            finally
            {
                _readerWriterLock.ReleaseWriterLock();
            }
        }

        public byte[] ReadAllBytes(string path)
        {
            try
            {
                _readerWriterLock.AcquireReaderLock(LockTimeoutMillisec);
                return File.ReadAllBytes(path);
            }
            finally
            {
                _readerWriterLock.ReleaseReaderLock();
            }
        }

        public void WriteAllText(string path, string contents)
        {
            try
            {
                _readerWriterLock.AcquireWriterLock(LockTimeoutMillisec);
                File.WriteAllText(path, contents);
            }
            finally
            {
                _readerWriterLock.ReleaseWriterLock();
            }
        }

        public bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public void CreateDirectory(string directoryPath)
        {
            try
            {
                _readerWriterLock.AcquireWriterLock(LockTimeoutMillisec);
                Directory.CreateDirectory(directoryPath);
            }
            finally
            {
                _readerWriterLock.ReleaseWriterLock();
            }
        }
    }
}
