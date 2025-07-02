using Imanage.Documents;

namespace Imanage
{
    public class DocumentProfileItemsSet : DocumentProfileItems
    {
        public DocumentProfileItemsSet() { }

        public DocumentProfileItemsSet(
            string author,
            string comment,
            string description,
            string imanoperator,
            EmailProfileItems emailProfileItems,
            bool declareAsRecord = false
            )
        {
            Author = author;
            Comment = comment;
            Description = description;
            Operator = imanoperator;
            EmailProfileItems = emailProfileItems;
            DeclareAsRecord = declareAsRecord;
        }

        public DocumentProfileItemsSet(DocumentProfileItems documentProfileItems)
        {
            Author = documentProfileItems.Author;
            Comment = documentProfileItems.Comment;
            Description = documentProfileItems.Description;
            Operator = documentProfileItems.Operator;
            EmailProfileItems = documentProfileItems.EmailProfileItems;
            DeclareAsRecord = documentProfileItems.DeclareAsRecord;
        }

        public ImanageDocumentProfilePatch GetRequestProfile()
        {
            return new ImanageDocumentProfilePatch(this);
        }
    }
}
