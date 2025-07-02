using iManage.WorkSite.Web.ServicesProxy.IWOVService;
using System;

namespace Imanage
{
    public class ImanageIwovServices : IImanageIwovServices
    {
        internal IWOVServices _iwovServices;

        public ImanageIwovServices(IWOVServices iwovServices)
        {
            _iwovServices = iwovServices;
        }

        public CreateDocumentsOutput CreateDocuments(CreateDocumentsInput createDocumentsInput)
        {
            return _iwovServices.CreateDocuments(createDocumentsInput);
        }

        public GetDocumentsOutput GetDocuments(GetDocumentsInput getDocumentsInput)
        {
            return _iwovServices.GetDocuments(getDocumentsInput);
        }

        public GetFolderContentsOutput GetFolderContents(GetFolderContentsInput getFolderContentsInput)
        {
            return _iwovServices.GetFolderContents(getFolderContentsInput);
        }

        public SetDocumentsPropertiesOutput SetDocumentsProperties(SetDocumentsPropertiesInput setDocumentsPropertiesInput)
        {
            return _iwovServices.SetDocumentsProperties(setDocumentsPropertiesInput);
        }

        public GetFoldersOutput GetFolders(GetFoldersInput getFoldersInput)
        {
            return _iwovServices.GetFolders(getFoldersInput);
        }

        public GetWorkspacesOutput GetWorkspaces(GetWorkspacesInput getWorkspacesInput)
        {
            return _iwovServices.GetWorkspaces(getWorkspacesInput);
        }

        public SearchWorkspacesOutput SearchWorkspaces(SearchWorkspacesInput searchWorkspacesInput)
        {
            return _iwovServices.SearchWorkspaces(searchWorkspacesInput);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _iwovServices.Dispose();
        }
    }
}
