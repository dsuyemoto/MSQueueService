using ININ.ICWS.Configuration.System;
using System;
using System.Collections.Generic;

namespace PureConnect
{
    public class ININSystemParameterDataContract
    {
        public ININConfigurationIdDataContract ConfigurationId { get; }
        public DateTime? CreatedDate { get; }
        public List<ININCustomAttributeDataContract> CustomAttributes { get; }
        public string Value { get; }

        public ININSystemParameterDataContract()
        {

        }

        public ININSystemParameterDataContract(SystemParameterDataContract systemParameterDataContract)
        {
            if (systemParameterDataContract == null) throw new Exception("SystemParameterDataContract is null");

            ConfigurationId = new ININConfigurationIdDataContract(systemParameterDataContract.ConfigurationId);

            CreatedDate = systemParameterDataContract.CreatedDate;

            if (systemParameterDataContract.CustomAttributes != null)
            {
                CustomAttributes = new List<ININCustomAttributeDataContract>();
                foreach (var attribute in systemParameterDataContract.CustomAttributes)
                    CustomAttributes.Add(new ININCustomAttributeDataContract(attribute));
            }

            Value = systemParameterDataContract.Value;
        }
    }
}
