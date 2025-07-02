using Imanage.Documents;
using Newtonsoft.Json;
using System;

namespace Imanage
{
    public class ImanageDocumentRest
    {
        public string Author { get; set; }
        public string Operator { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public EmailProfileItems EmailProfileItems { get; set; }     
        public string Number { get; set; }
        public string Version { get; set; }
        public byte[] Content { get; set; }

        public ImanageDocumentRest()
        {

        }

        public ImanageDocumentRest(DocumentProfileItems documentProfileItemsSet)
        {
            Author = documentProfileItemsSet.Author;
            Operator = documentProfileItemsSet.Operator;
            Comment = documentProfileItemsSet.Comment;
            Name = documentProfileItemsSet.Description;
            EmailProfileItems = documentProfileItemsSet.EmailProfileItems;
        }
    }
}
