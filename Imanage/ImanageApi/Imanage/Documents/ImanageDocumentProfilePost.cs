using Newtonsoft.Json;

namespace Imanage.Documents
{
    public class ImanageDocumentProfilePost
    {
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("operator")]
        public string Operator { get; set; }
        [JsonProperty("class")]
        public string Class { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("extension")]
        public string Extension { get; set; }

        public ImanageDocumentProfilePost(DocumentProfileItems documentProfileItems)
        {
            Author = documentProfileItems.Author;
            Operator = documentProfileItems.Operator;
            Class = documentProfileItems.Class.ToString();
            Comment = documentProfileItems.Comment;
            Name = documentProfileItems.Description;
            Extension = documentProfileItems.Extension;
        }
    }
}
