using iManage.WorkSite.Web.ServicesProxy.IWOVService;
namespace Imanage
{
    public interface IImanageServicesConnection
    {
        CreateDocumentsOutput CreateDocuments(CreateDocumentsInput CreateDocumentsInput);
        GetDocumentsOutput GetDocuments(ImanageGetDocumentsInput imanageGetDocumentsInput);
        GetFolderContentsOutput GetFolderContents(ImanageGetFolderContentsInput imanageGetFolderContentsInput);
        SetDocumentsPropertiesOutput SetDocumentProperties(ImanageSetDocumentsPropertiesInput imanageSetDocumentPropertiesInput);
        ImanageGetFoldersOutput GetFolders(ImanageGetFoldersInput imanageGetFoldersInput);
        GetWorkspacesOutput GetWorkspaces(GetWorkspacesInput getWorkspacesInput);
        ImanageSearchWorkspacesOutput SearchWorkspaces(ImanageSearchWorkspacesInput imanageSearchWorkspacesInput);
    }
}
