using static Imanage.DocumentProfileItems;

namespace Imanage
{
    public class ImanageGetFolderContentsInput : ImanageInput
    {
        public readonly ImanageFolderObjectId ImanageFolderObjectId;

        public ImanageGetFolderContentsInput(
            ImanageFolderObjectId imanageFolderObjectId, 
            ProfileAttributeId[] outputProfileAttributeIds,
            OutputMaskName[] outputMaskNames
            )
        {
            ImanageFolderObjectId = imanageFolderObjectId;
            OutputProfileAttributeIds = outputProfileAttributeIds;
            OutputMaskNames = outputMaskNames;
        }
    }
}
