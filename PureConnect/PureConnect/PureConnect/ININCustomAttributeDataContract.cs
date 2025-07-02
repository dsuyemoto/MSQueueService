using ININ.ICWS.Configuration;

namespace PureConnect
{
    public class ININCustomAttributeDataContract
    {
        CustomAttributeDataContract _customAttributeDataContract;

        public string Name { get { return _customAttributeDataContract.Name; } }
        public string Value { get { return _customAttributeDataContract.Value; } }

        public ININCustomAttributeDataContract(CustomAttributeDataContract customAttributeDataContract)
        {
            _customAttributeDataContract = customAttributeDataContract;
        }
    }
}