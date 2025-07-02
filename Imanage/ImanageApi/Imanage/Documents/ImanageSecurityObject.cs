namespace Imanage
{
    public class ImanageSecurityObject
    {
        public enum SecurityType { VIEW, PRIVATE, PUBLIC }

        public ImanageAclItem[] ImanageAclItems { get; set; } = new ImanageAclItem[] { };
        public SecurityType Security { get; set; } = SecurityType.PRIVATE;
        public bool InheritSecurity { get; set; } = true;

        public ImanageSecurityObject(
            ImanageAclItem[] imanageAclItems,
            SecurityType securityType,
            bool inheritSecurity = true)
        {
            ImanageAclItems = imanageAclItems;
            Security = securityType;
            InheritSecurity = inheritSecurity;
        }
    }
}
