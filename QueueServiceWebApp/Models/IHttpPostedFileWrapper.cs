namespace QueueServiceWebApp.Models
{
    public interface IHttpPostedFileWrapper
    {
        string FileName { get; }

        void SaveAs(string fileName);
    }
}