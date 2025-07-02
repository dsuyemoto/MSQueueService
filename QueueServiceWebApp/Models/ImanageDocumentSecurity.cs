namespace QueueServiceWebApp.Models
{
    public class ImanageDocumentSecurity
    {
        public string SecurityUsername { get; set; }
        public string SecurityPasswordBase64 { get; set; }
        public string Session { get; set; }
        public string Database { get; set; }
    }
}