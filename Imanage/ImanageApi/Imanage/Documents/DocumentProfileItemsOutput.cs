using Imanage.Documents;

namespace Imanage
{
    public class DocumentProfileItemsOutput : DocumentProfileItems
    {
        public DocumentProfileItemsOutput() { }

        public DocumentProfileItemsOutput(DocumentResponseSingleData data)
        {
            Author = data.Author;
            Comment = data.Comment;
            Description = data.Name;
            Operator = data.Operator;
            Extension = data.Extension;
            EmailProfileItems = new EmailProfileItems(
                data.To,
                data.From,
                data.Cc,
                data.Sent,
                data.Received,
                data.Name);
        }

        public DocumentProfileItemsOutput(DocumentProfileItems documentProfileItems)
        {
            Author = documentProfileItems.Author;
            Comment = documentProfileItems.Comment;
            Description = documentProfileItems.Description;
            Operator = documentProfileItems.Operator;
            EmailProfileItems = documentProfileItems.EmailProfileItems;
        }
    }
}
