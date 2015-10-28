using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyFileStorage.Config
{
    public class StorageConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("location", IsRequired = true)]
        public string Location
        {
            get
            {
                return this["location"] as string;
            }
        }

        [ConfigurationProperty("defaultExtension", IsRequired = true)]
        public string DefaultExtention
        {
            get
            {
                return this["defaultExtension"] as string;
            }
        }

        [ConfigurationProperty("verboseLog")]
        public bool VerboseLog
        {
            get
            {
                var res = false;
                bool.TryParse(this["verboseLog"] as string, out res);
                return res;
            }
        }
    }
}
