namespace Imanage
{
    public class ImanageDocumentCreate : ImanageDocument, IImanageDocument
    {
        public DocumentProfileItemsCreate DocumentProfileItems { get; set; }

        public ImanageDocumentCreate(
            byte[] content, 
            string database,
            DocumentProfileItemsCreate documentProfileItemsCreate,
            ImanageSecurityObject imanageSecurityObject)
        {
            Content = content;
            Database = database;
            DocumentProfileItems = documentProfileItemsCreate;
            SecurityObject = imanageSecurityObject;           
        }
    }
}
