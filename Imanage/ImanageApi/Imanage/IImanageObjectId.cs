namespace Imanage
{
    public interface IImanageObjectId
    {
        string Database { get; set; }
        string Session { get; set; }

        string GetObjectId();
    }
}