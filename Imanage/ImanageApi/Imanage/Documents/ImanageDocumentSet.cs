namespace Imanage.Documents
{
    public class ImanageDocumentSet : ImanageDocument, IImanageDocument
    {
        public DocumentProfileItemsSet DocumentProfileItems { get; set; }

        public ImanageDocumentSet(
            byte[] content,
            DocumentProfileItemsSet documentProfileItemsSet,
            ImanageSecurityObject imanageSecurityObject,
            ImanageDocumentObjectId objectId)
        {
            Content = content;
            DocumentProfileItems = documentProfileItemsSet;
            SecurityObject = imanageSecurityObject;
            DocumentObjectId = objectId;
            if (DocumentObjectId != null)
                Database = DocumentObjectId.Database;
        }
    }
}
