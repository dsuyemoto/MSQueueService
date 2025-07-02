namespace EWS
{
    public interface IExchAttachment
    {
        byte[] Content { get; set; }
        string FileName { get; set; }
    }
}
