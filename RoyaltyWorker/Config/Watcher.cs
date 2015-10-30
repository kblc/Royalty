﻿using System;
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

        [ConfigurationProperty("checkTimerInterval", IsRequired = false)]
        public TimeSpan CheckTimerInterval
        {
            get
            {
                var res = new TimeSpan(0,0,30);
                TimeSpan.TryParse(this["checkTimerInterval"] as string, out res);
                return res;
            }
        }

        [ConfigurationProperty("exceptionIfNoOneFileInQueue", IsRequired = false, DefaultValue = true)]
        public bool ExceptionIfNoOneFileInQueue
        {
            get
            {
                var res = true;
                bool.TryParse(this["exceptionIfNoOneFileInQueue"] as string, out res);
                return res;
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