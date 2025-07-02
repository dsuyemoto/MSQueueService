namespace QueueServiceWebApp.Models
{
    public class ImanageDocumentCreateDTO : ImanageDocumentSecurity
    {
        public string Author { get; set; }
        public string Operator { get; set; }
        public string _DescriptionBase64 { get; set; }
        public string _CommentBase64 { get; set; }
        public ImanageEmailPropertiesDTO _EmailProperties { get; set; }
        public string _ContentBytesBase64 { get; set; }
        public string _Extension { get; set; }

        public ImanageDocumentCreateDTO()
        {

        }
    }
}