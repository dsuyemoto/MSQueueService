namespace Imanage
{
    public abstract class ImanageDocument
    {
        public const string EXTENSION_NRL = ".nrl"; 

        public string Database { get; protected set; }
        public byte[] Content { get; protected set; }
        public ImanageHistoryItem[] History { get; protected set; }
        public ImanageSecurityObject SecurityObject { get; protected set; }
        public ImanageDocumentObjectId DocumentObjectId { get; protected set; }
    }
}
