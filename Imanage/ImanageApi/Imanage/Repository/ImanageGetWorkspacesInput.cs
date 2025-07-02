using static Imanage.DocumentProfileItems;

namespace Imanage
{
    public class ImanageGetWorkspacesInput : ImanageInput
    {
        public readonly ImanageWorkspaceObjectId[] ImanageWorkspaceObjectIds;

        public ImanageGetWorkspacesInput(
            ImanageWorkspaceObjectId[] imanageWorkspaceObjectIds,
            ProfileAttributeId[] outputProfileAttributeIds,
            OutputMaskName[] outputMaskNames)
        {
            ImanageWorkspaceObjectIds = imanageWorkspaceObjectIds;
            OutputProfileAttributeIds = outputProfileAttributeIds;
            OutputMaskNames = outputMaskNames;
        }
    }
}
