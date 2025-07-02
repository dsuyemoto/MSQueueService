namespace Imanage
{
    public class ImanageError
    {
        public ImanageProfileError[] ImanageProfileErrors { get; set; }
        public string Message { get; set; }

        public ImanageError()
        {

        }

        public ImanageError(string message, ImanageProfileError[] imanageProfileErrors)
        {
            Message = message;
            ImanageProfileErrors = imanageProfileErrors;
        }
    }
}
