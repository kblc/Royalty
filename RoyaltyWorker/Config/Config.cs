using RoyaltyFileStorage.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Web;

namespace RoyaltyWorker.Config
{
    //public interface IConfig
    //{
    //    IQueryable<MimeType> MimeTypes { get; }
    //}

    public static class Config// : IConfig
    {
        /// <summary>
        /// Check is watcher configured
        /// </summary>
        public static bool IsWatcherConfigured { get { return WatcherConfig != null; } }

        /// <summary>
        /// Watcher config
        /// </summary>
        public static WatcherConfigSection WatcherConfig
        {
            get
            {
                return ConfigurationManager.GetSection(WatcherConfigSection.SectionName) as WatcherConfigSection;
            }
        }

        public static bool IsWorkerConfigured { get { return WorkerConfig != null; } }

        /// <summary>
        /// Worker config
        /// </summary>
        public static WorkerConfigSection WorkerConfig
        {
            get
            {
                return ConfigurationManager.GetSection(WorkerConfigSection.SectionName) as WorkerConfigSection;
            }
        }
    }
}