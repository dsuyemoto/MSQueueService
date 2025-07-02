using iManage.WorkSite.Web.ServicesProxy.IWOVService;
using static Imanage.DocumentProfileItems;

namespace Imanage
{
    public class ImanageGetDocumentsInput : ImanageInput
    {
        public readonly ImanageDocumentObjectId[] ImanageDocumentObjectIds;

        public ImanageGetDocumentsInput(
            ImanageDocumentObjectId[] imanageDocumentObjectIds,
            ProfileAttributeId[] outputProfileAttributeIds,
            OutputMaskName[] outputMaskNames)
        {
            ImanageDocumentObjectIds = imanageDocumentObjectIds;
            OutputProfileAttributeIds = outputProfileAttributeIds;
            OutputMaskNames = outputMaskNames;
        }
    }
}
