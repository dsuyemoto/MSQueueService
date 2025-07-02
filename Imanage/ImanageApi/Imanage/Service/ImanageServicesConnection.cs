using iManage.WorkSite.Web.ServicesProxy;
using iManage.WorkSite.Web.ServicesProxy.IWOVService;
using System;
using System.Collections.Generic;
using System.Net;

namespace Imanage
{
    public class ImanageServicesConnection : IImanageServicesConnection
    {
        private string _serviceUrl;
        private string _authType;
        private NetworkCredential _networkCredential;

        enum AuthType
        {
            ntlm
        }

        public ImanageServicesConnection(string serviceUrl, NetworkCredential networkCredential)
        {
            _serviceUrl = serviceUrl;
            _authType = AuthType.ntlm.ToString();
            _networkCredential = networkCredential;
        }
        public ImanageServicesConnection(string serviceUrl, string authType, NetworkCredential networkCredential)
        {
            _serviceUrl = serviceUrl; //http://lvdimanweb1.lw.com/Worksite
            _authType = authType; //ntlm
            _networkCredential = networkCredential;
        }

        private IWOVServices GetConnection()
        {
            var spm = ServicePointManager.FindServicePoint(new Uri(_serviceUrl));
            return new Connection(_serviceUrl, _authType, _networkCredential).IWOVServices;
        }

        public CreateDocumentsOutput CreateDocuments(CreateDocumentsInput createDocumentsInput)
        {
            var createDocumentsOutput = new CreateDocumentsOutput();

            using (var conn = GetConnection())
            {
                
                createDocumentsOutput = conn.CreateDocuments(createDocumentsInput);
            }

            return createDocumentsOutput;
        }

        public GetDocumentsOutput GetDocuments(ImanageGetDocumentsInput imanageGetDocumentsInput)
        {
            var getDocumentsOutput = new GetDocumentsOutput();

            using (var conn = GetConnection())
            {
                getDocumentsOutput = conn.GetDocuments(imanageGetDocumentsInput.GetDocumentsInput());
            }

            return getDocumentsOutput;
        }

        public GetFolderContentsOutput GetFolderContents(ImanageGetFolderContentsInput imanageGetFolderContentsInput)
        {
            var getFolderContentsOutput = new GetFolderContentsOutput();

            using (var conn = GetConnection())
            {
                getFolderContentsOutput = conn.GetFolderContents(imanageGetFolderContentsInput.GetFolderContentsInput());
            }

            return getFolderContentsOutput;
        }

        public SetDocumentsPropertiesOutput SetDocumentProperties(ImanageSetDocumentsPropertiesInput imanageSetDocumentPropertiesInput)
        {
            var setDocumentsPropertiesOutput = new SetDocumentsPropertiesOutput();

            using (var conn = GetConnection())
            {
                setDocumentsPropertiesOutput = conn.SetDocumentsProperties(imanageSetDocumentPropertiesInput.SetDocumentsPropertiesInput());
            }

            return setDocumentsPropertiesOutput;
        }

        public ImanageGetFoldersOutput GetFolders(ImanageGetFoldersInput imanageGetFoldersInput)
        {
            using (var conn = GetConnection())
                return new ImanageGetFoldersOutput(conn.GetFolders(
                    imanageGetFoldersInput.GetFoldersInput()));
        }

        public GetWorkspacesOutput GetWorkspaces(GetWorkspacesInput getWorkspacesInput)
        {
            using (var conn = GetConnection())
                return conn.GetWorkspaces(getWorkspacesInput);
        }

        public ImanageSearchWorkspacesOutput SearchWorkspaces(ImanageSearchWorkspacesInput imanageSearchWorkspacesInput)
        {
            using (var conn = GetConnection())
                return new ImanageSearchWorkspacesOutput(
                    conn.SearchWorkspaces(imanageSearchWorkspacesInput.SearchWorkspacesInput()));
        }
    }
}
