using ININ.ICWS.Configuration;
using System;

namespace PureConnect
{
    public class ININConfigurationIdDataContract
    {
        ConfigurationIdDataContract _configurationIdDataContract;

        public string Id { get { return _configurationIdDataContract.Id; } }
        public string DisplayName { get { return _configurationIdDataContract.DisplayName; } }
        public Uri Uri { get { return _configurationIdDataContract.Uri; } }

        public ININConfigurationIdDataContract(ConfigurationIdDataContract configurationIdDataContract)
        {
            _configurationIdDataContract = configurationIdDataContract;
        }
    }
}