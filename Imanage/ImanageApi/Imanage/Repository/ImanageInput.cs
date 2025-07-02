using System.Collections.Generic;
using static Imanage.DocumentProfileItems;

namespace Imanage
{
    public abstract class ImanageInput
    {
        public enum OutputMaskName
        {
            Profile,
            Security,
            DocumentContent,
            History,
            CustomProperties,
            AdditionalProperties,
            SubFolders,
            FolderContent
        }

        public ProfileAttributeId[] OutputProfileAttributeIds { get; protected set; }
        public OutputMaskName[] OutputMaskNames { get; protected set; }

        public static string[] GetObjectIds(ImanageDocumentObjectId[] imanageDocumentObjectIds)
        {
            var objectIds = new List<string>();
            foreach (var imanageDocumentObjectId in imanageDocumentObjectIds)
                objectIds.Add(imanageDocumentObjectId.GetObjectId());

            return objectIds.ToArray();
        }
    }
}
