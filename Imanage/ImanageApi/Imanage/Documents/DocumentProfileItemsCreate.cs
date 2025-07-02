using Imanage.Documents;

namespace Imanage
{
    public class DocumentProfileItemsCreate : DocumentProfileItems
    {
        public DocumentProfileItemsCreate(
            string author,
            string comment,
            string description,
            string imanoperator,
            string extension,
            EmailProfileItems emailProfileItems,
            bool declareAsRecord = false
            )
        {
            Author = author;
            Comment = comment;
            Description = description;
            Operator = imanoperator;
            Extension = extension;
            EmailProfileItems = emailProfileItems;
            DeclareAsRecord = declareAsRecord;
        }

        public ImanageDocumentProfilePost GetRequestProfile()
        {
            if (EmailProfileItems != null)
                return new ImanageDocumentProfilePostEmail(this);
            return new ImanageDocumentProfilePost(this);
        }

    }
}
