using RoyaltyFileStorage.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Web;

namespace RoyaltyService.Config
{
    public static class Config
    {
        /// <summary>
        /// Check is services configured
        /// </summary>
        public static bool IsServicesConfigured { get { try { return ServicesConfig != null; } catch { return false; } } }

        /// <summary>
        /// Services config
        /// </summary>
        public static ServicesConfigSection ServicesConfig
        {
            get
            {
                return ConfigurationManager.GetSection(ServicesConfigSection.SectionName) as ServicesConfigSection;
            }
        }
    }
}