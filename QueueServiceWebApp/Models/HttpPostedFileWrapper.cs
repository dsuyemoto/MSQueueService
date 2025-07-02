using System.IO;
using System.Web;

namespace QueueServiceWebApp.Models
{
    public class HttpPostedFileWrapper : IHttpPostedFileWrapper
    {
        HttpPostedFile _httpPostedFile;

        public string FileName => _httpPostedFile.FileName;

        public HttpPostedFileWrapper()
        {

        }

        public HttpPostedFileWrapper(HttpPostedFile httpPostedFile)
        {
            _httpPostedFile = httpPostedFile;
        }

        public void SaveAs(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            _httpPostedFile.SaveAs(fileName);
        }
    }
}