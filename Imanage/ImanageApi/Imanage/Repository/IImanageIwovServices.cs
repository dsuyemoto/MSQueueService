using iManage.WorkSite.Web.ServicesProxy.IWOVService;
using System;

namespace Imanage
{
    public interface IImanageIwovServices : IDisposable
    {
        CreateDocumentsOutput CreateDocuments(CreateDocumentsInput createDocumentsInput);
        GetDocumentsOutput GetDocuments(GetDocumentsInput getDocumentsInput);
        GetFolderContentsOutput GetFolderContents(GetFolderContentsInput getFolderContentsInput);
        GetFoldersOutput GetFolders(GetFoldersInput getFoldersInput);
        GetWorkspacesOutput GetWorkspaces(GetWorkspacesInput getWorkspacesInput);
        SearchWorkspacesOutput SearchWorkspaces(SearchWorkspacesInput searchWorkspacesInput);
        SetDocumentsPropertiesOutput SetDocumentsProperties(SetDocumentsPropertiesInput setDocumentsPropertiesInput);
    }
}