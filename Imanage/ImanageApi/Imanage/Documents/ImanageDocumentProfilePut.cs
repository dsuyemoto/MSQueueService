using Newtonsoft.Json;

namespace Imanage.Documents
{
    internal class ImanageDocumentProfilePut
    {
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("operator")]
        public string Operator { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        public ImanageDocumentProfilePut(ImanageDocumentRest imanageDocumentRest)
        {
            Author = imanageDocumentRest.Author;
            Operator = imanageDocumentRest.Operator;
            Comment = imanageDocumentRest.Comment;
            Name = imanageDocumentRest.Name;
        }
    }
}
