using Newtonsoft.Json;

namespace Imanage
{
    public class DocumentResponseSingle
    {
        public DocumentResponseSingleData data { get; set; }
        public DocumentResponseSingleWarnings[] warnings { get; set; }
    }

    public class DocumentResponseSingleWarnings
    {
        public string field { get; set; }
        public string error { get; set; }

        public DocumentResponseSingleWarnings(string field, string error)
        {
            this.field = field;
            this.error = error;
        }
    }

    public class DocumentResponseSingleData
    {
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("operator")]
        public string Operator { get; set; }
        [JsonProperty("class")]
        public string Class { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("document_number")]
        public string Number { get; set; }
        [JsonProperty("version")]
        public string Version { get; set; }
        [JsonProperty("extension")]
        public string Extension { get; set; }
        [JsonProperty("custom13")]
        public string From { get; set; }
        [JsonProperty("custom14")]
        public string To { get; set; }
        [JsonProperty("custom15")]
        public string Cc { get; set; }
        [JsonProperty("custom21")]
        public string Sent { get; set; }
        [JsonProperty("custom22")]
        public string Received { get; set; }
    }
}
