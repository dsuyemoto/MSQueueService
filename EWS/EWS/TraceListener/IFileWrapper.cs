namespace EWS
{
    public interface IFileWrapper
    {
        void AppendAllText(string path, string contents);
        bool Exists(string path);
        byte[] ReadAllBytes(string path);
        void WriteAllText(string path, string contents);
        bool DirectoryExists(string directoryPath);
        void CreateDirectory(string directoryPath);
    }
}