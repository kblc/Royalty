using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Config
{
    public class ServicesConfigSection : ConfigurationSection
    {
        public const string SectionName = "servicesConfig";

        [ConfigurationProperty("fileServiceUrlPrefix", IsRequired = true, DefaultValue = "")]
        public Uri FileServiceUrlPrefix
        {
            get
            {
                return this["fileServiceUrlPrefix"] as Uri;
            }
        }

        [ConfigurationProperty("fileServiceLogFileName", IsRequired = false, DefaultValue = "")]
        public string FileServiceLogFileName
        {
            get
            {
                return this["fileServiceLogFileName"] as string;
            }
        }

        [ConfigurationProperty("maxHistoryCount", IsRequired = false, DefaultValue = "100")]
        public long MaxHistoryCount
        {
            get
            {
                return (long)this["maxHistoryCount"];
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
