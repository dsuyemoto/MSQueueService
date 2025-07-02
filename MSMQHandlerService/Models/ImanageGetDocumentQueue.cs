namespace MSMQHandlerService.Models
{
    public class ImanageGetDocumentQueue : ImanageBase
    {
        public string Number { get; set; }
        public string Version { get; set; }
        public string[] OutputProfileItems { get; set; }
        public string SecurityUsername { get; set; }
        public string SecurityPasswordBase64 { get; set; }
        public string Session { get; set; }
        public string Database { get; set; }

        public ImanageGetDocumentQueue()
        {

        }
    }
}