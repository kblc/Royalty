using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyWorker.Config
{
    public class WorkerConfigSection : ConfigurationSection
    {
        public const string SectionName = "workerConfig";

        internal const string DefaultEncoding = "utf-8";

        [ConfigurationProperty("checkTimerInterval", IsRequired = false)]
        public TimeSpan CheckTimerInterval
        {
            get
            {
                var res = new TimeSpan(0, 0, 10);
                TimeSpan.TryParse(this["checkTimerInterval"] as string, out res);
                return res;
            }
        }

        [ConfigurationProperty("logFileEncoding", IsRequired = false, DefaultValue = DefaultEncoding)]
        public Encoding LogFileEncoding
        {
            get
            {
                try
                {
                    var encName = this["logFileEncoding"] as string;
                    return Encoding.GetEncoding(encName);
                } catch { return Encoding.Default; }
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
