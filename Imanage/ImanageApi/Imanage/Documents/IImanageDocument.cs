namespace Imanage
{
    public interface IImanageDocument
    {
        byte[] Content { get; }
        string Database { get; }
        ImanageHistoryItem[] History { get; }
        ImanageSecurityObject SecurityObject { get; }
    }
}