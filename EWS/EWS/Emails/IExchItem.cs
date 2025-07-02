namespace EWS
{
    public interface IExchItem
    {
        byte[] Content { get; }
        bool IsAttachment { get; }
    }
}