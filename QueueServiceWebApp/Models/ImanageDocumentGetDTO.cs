namespace QueueServiceWebApp.Models
{
    public class ImanageDocumentGetDTO : ImanageDocumentSecurity
    {
        public string Number { get; set; }
        public string Version { get; set; }

        public ImanageDocumentGetDTO()
        {

        }
    }
}