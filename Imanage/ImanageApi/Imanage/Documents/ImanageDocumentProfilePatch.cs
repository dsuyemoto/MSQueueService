using Newtonsoft.Json;

namespace Imanage.Documents
{
    public class ImanageDocumentProfilePatch
    {
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("operator")]
        public string Operator { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        public ImanageDocumentProfilePatch(DocumentProfileItems documentProfileItems)
        {
            Author = documentProfileItems.Author;
            Operator = documentProfileItems.Operator;
            Comment = documentProfileItems.Comment;
            Name = documentProfileItems.Description;
        }
    }
}
