using iManage.WorkSite.Web.ServicesProxy.IWOVService;

namespace Imanage
{
    public class ImanageProfileError
    {
        public string AttributeId { get; set; }
        public string Message { get; set; }

        public ImanageProfileError()
        {

        }

        public ImanageProfileError(ProfileError profileError)
        {
            AttributeId = profileError.AttributeID.ToString();
            Message = profileError.Message;
        }
    }
}
