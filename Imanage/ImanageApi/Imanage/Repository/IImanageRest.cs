namespace Imanage
{
    public interface IImanageRest
    {
        ImanageDocumentsOutput CreateDocuments(ImanageCreateDocumentsInput imanageCreateDocumentsInput);
        ImanageDocumentsOutput UpdateDocuments(ImanageSetDocumentsPropertiesInput imanageSetDocumentsPropertiesInput);
        ImanageDocumentsOutput GetDocuments(ImanageGetDocumentsInput imanageGetDocumentsInput);
    }
}