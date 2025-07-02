namespace Imanage
{
    public interface IImanageDocumentOutput
    {
        ImanageError ImanageError { get; }
        ImanageDocumentObjectId DocumentObjectId { get; }
        byte[] Content { get; }
        string Database { get; }
        DocumentProfileItemsOutput DocumentProfileItems { get; }
        ImanageHistoryItem[] History { get; }
        ImanageSecurityObject SecurityObject { get; }
        string FileName { get; }
        ImanageDocumentNrl Nrl { get; }
    }
}