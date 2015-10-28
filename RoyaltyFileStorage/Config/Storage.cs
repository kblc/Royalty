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
        [ConfigurationProperty("location", IsRequired = true, DefaultValue = "")]
        public string Location
        {
            get
            {
                return this["location"] as string;
            }
        }

        [ConfigurationProperty("verboseLog", IsRequired = false, DefaultValue = false)]
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
