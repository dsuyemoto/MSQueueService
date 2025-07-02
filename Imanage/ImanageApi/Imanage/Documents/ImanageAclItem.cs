namespace Imanage
{
    public class ImanageAclItem
    {
        public string Name { get; set; }
        public ImanageAccessRight ImanAccessRight { get; set; }
        public ImanageAclItemType ImanAclType { get; set; }

        public enum ImanageAccessRight { NONE, READ, READWRITE, ALL }
        public enum ImanageAclItemType { USER, GROUP };

        public ImanageAclItem(string name,
            ImanageAccessRight imanAccessRight = ImanageAccessRight.ALL,
            ImanageAclItemType imanAclType = ImanageAclItemType.USER) 
        {
            Name = name;
            ImanAccessRight = imanAccessRight;
            ImanAclType = imanAclType;
        }
    }
}
