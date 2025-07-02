using iManage.WorkSite.Web.ServicesProxy.IWOVService;
using static Imanage.DocumentProfileItems;

namespace Imanage
{
    public class ImanageCreateDocumentsInput : ImanageInput
    {
        public readonly ImanageFolderObjectId ImanageFolderObjectId;
        public readonly ImanageDocumentCreate[] ImanageDocumentsCreate;
        public readonly bool UseFolderProfile;

        public ImanageCreateDocumentsInput(
            ImanageFolderObjectId imanageDestinationFolderObjectId,
            ImanageDocumentCreate[] imanageDocumentsCreate,
            ProfileAttributeId[] outputProfileAttributeIds,
            OutputMaskName[] outputMaskNames,
            bool useFolderProfile = true)
        {
            ImanageFolderObjectId = imanageDestinationFolderObjectId;
            ImanageDocumentsCreate = imanageDocumentsCreate;
            OutputProfileAttributeIds = outputProfileAttributeIds;
            OutputMaskNames = outputMaskNames;
            UseFolderProfile = useFolderProfile;
        }
    }
}
