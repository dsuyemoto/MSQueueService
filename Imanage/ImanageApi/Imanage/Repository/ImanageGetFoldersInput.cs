namespace Imanage
{
    public class ImanageGetFoldersInput : ImanageInput
    {
        public string[] ObjectIDs { get; set; }

        public ImanageGetFoldersInput(string[] objectIds, OutputMaskName[] outputMaskNames)
        {
            ObjectIDs = objectIds;
            OutputMaskNames = outputMaskNames;
        }
    }
}
