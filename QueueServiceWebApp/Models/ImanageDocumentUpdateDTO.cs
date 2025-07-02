namespace QueueServiceWebApp.Models
{
    public class ImanageDocumentUpdateDTO : ImanageDocumentSecurity
    {
        public string _Author { get; set; }
        public string _Operator { get; set; }
        public string _Class { get; set; }
        public string _Type { get; set; }
        public string _DescriptionBase64 { get; set; }
        public string _CommentBase64 { get; set; }
        public ImanageEmailPropertiesDTO _EmailProperties { get; set; }
        public string _ContentBytesBase64 { get; set; }

        public ImanageDocumentUpdateDTO()
        {

        }
    }
}