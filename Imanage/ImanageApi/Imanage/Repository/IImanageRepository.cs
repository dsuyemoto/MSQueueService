namespace Imanage
{
    public interface IImanageRepository
    {
        ImanageDocumentsOutput CreateDocuments(ImanageCreateDocumentsInput imanageCreateDocumentsInput);
        ImanageDocumentsOutput GetDocuments(ImanageGetDocumentsInput imanageGetDocumentsInput);
        ImanageDocumentsOutput UpdateDocuments(ImanageSetDocumentsPropertiesInput imanageSetDocumentsPropertiesInput);
        ImanageFoldersOutput GetFolders(ImanageGetFoldersInput imanageGetFoldersInput);
        ImanageDocumentsOutput GetFolderContents(ImanageGetFolderContentsInput imanageGetFolderContentsInput);
        ImanageWorkspacesOutput GetWorkspaces(ImanageGetWorkspacesInput imanageGetWorkspacesInput);
        ImanageWorkspacesOutput SearchWorkspaces(ImanageSearchWorkspacesInput imanageSearchWorkspacesInput);
    }
}