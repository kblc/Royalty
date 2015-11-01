using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyWorker.Config
{
    public class WatcherConfigSection : ConfigurationSection
    {
        public const string SectionName = "watcherConfig";

        internal const string DefaultTimerInterval = "00:00:30.000";
        internal const bool DefaultExceptionIfNoOneFileInQueue = true;
        internal const bool DefaultVerboseLog = false;

        [ConfigurationProperty("timerInterval", IsRequired = false, DefaultValue = DefaultTimerInterval)]
        public TimeSpan TimerInterval
        {
            get
            {
                try
                {
                    return (TimeSpan)this["timerInterval"];
                }
                catch { return TimeSpan.Parse(DefaultTimerInterval); }
            }
        }

        [ConfigurationProperty("exceptionIfNoOneFileInQueue", IsRequired = false, DefaultValue = DefaultExceptionIfNoOneFileInQueue)]
        public bool ExceptionIfNoOneFileInQueue
        {
            get
            {
                bool res;
                if (bool.TryParse(this["exceptionIfNoOneFileInQueue"] as string, out res))
                    return res;
                else
                    return DefaultExceptionIfNoOneFileInQueue;
            }
        }

        [ConfigurationProperty("verboseLog", IsRequired = false, DefaultValue = DefaultVerboseLog)]
        public bool VerboseLog
        {
            get
            {
                bool res;
                if (bool.TryParse(this["verboseLog"] as string, out res))
                    return res;
                else
                    return DefaultVerboseLog;
            }
        }
    }
}
