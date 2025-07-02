using Imanage.Documents;
using static Imanage.DocumentProfileItems;

namespace Imanage
{
    public class ImanageSetDocumentsPropertiesInput : ImanageInput
    {
        public readonly ImanageFolderObjectId FolderObjectId;
        public readonly ImanageDocumentSet[] ImanageDocumentsSet;

        public ImanageSetDocumentsPropertiesInput(
            ImanageFolderObjectId imanageDestinationFolderObjectId,
            ImanageDocumentSet[] imanageDocumentsSet,
            ProfileAttributeId[] outputProfileAttributeIds,
            OutputMaskName[] outputMaskNames)
        {
            FolderObjectId = imanageDestinationFolderObjectId;
            ImanageDocumentsSet = imanageDocumentsSet;
            OutputProfileAttributeIds = outputProfileAttributeIds;
            OutputMaskNames = outputMaskNames;
        }
    }
}
