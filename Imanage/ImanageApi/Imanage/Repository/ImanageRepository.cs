using System.Net;

namespace Imanage
{
    public class ImanageRepository : IImanageRepository
    {
        IImanageRest _imanageRest;

        enum AuthType
        {
            ntlm
        }

        public ImanageRepository()
        {

        }

        public ImanageRepository(ImanageCreds imanageCreds)
        {
            Initialize(
                new NetworkCredential(imanageCreds.Username, imanageCreds.Password),
                imanageCreds.Database);
        }

        public ImanageRepository(IImanageRest imanageRest)
        {
            _imanageRest = imanageRest;
        }

        private void Initialize(NetworkCredential networkCredential, string database)
        {
            _imanageRest = new ImanageRest(networkCredential, database);
        }

        public ImanageDocumentsOutput CreateDocuments(ImanageCreateDocumentsInput imanageCreateDocumentsInput)
        {
            return _imanageRest.CreateDocuments(imanageCreateDocumentsInput);
        }

        public ImanageDocumentsOutput GetDocuments(ImanageGetDocumentsInput imanageGetDocumentsInput)
        {
            return _imanageRest.GetDocuments(imanageGetDocumentsInput);
        }

        public ImanageDocumentsOutput UpdateDocuments(ImanageSetDocumentsPropertiesInput imanageSetDocumentsPropertiesInput)
        {
            return _imanageRest.UpdateDocuments(imanageSetDocumentsPropertiesInput);
        }

        public ImanageFoldersOutput GetFolders(ImanageGetFoldersInput imanageGetFoldersInput)
        {
            throw new System.NotImplementedException();
        }

        public ImanageDocumentsOutput GetFolderContents(ImanageGetFolderContentsInput imanageGetFolderContentsInput)
        {
            throw new System.NotImplementedException();
        }

        public ImanageWorkspacesOutput GetWorkspaces(ImanageGetWorkspacesInput imanageGetWorkspacesInput)
        {
            throw new System.NotImplementedException();
        }

        public ImanageWorkspacesOutput SearchWorkspaces(ImanageSearchWorkspacesInput imanageSearchWorkspacesInput)
        {
            throw new System.NotImplementedException();
        }
    }
}
