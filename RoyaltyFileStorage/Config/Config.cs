using RoyaltyFileStorage.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Web;

namespace RoyaltyFileStorage.Config
{
    //public interface IConfig
    //{
    //    IQueryable<MimeType> MimeTypes { get; }
    //}

    public static class Config// : IConfig
    {
        /// <summary>
        /// Mimetypes from config
        /// </summary>
        public static IQueryable<MimeType> MimeTypes
        {
            get
            {
                var configInfo = ConfigurationManager.GetSection("mimeConfig") as MimeTypesConfigSection;
                return (configInfo == null 
                    ? Enumerable.Empty<MimeType>()
                    : configInfo.MimeTypes.OfType<MimeType>()
                    ).AsQueryable();
            }
        }

        /// <summary>
        /// Check is storage configured
        /// </summary>
        public static bool IsStorageConfigured { get { return StorageConfig != null; } }

        /// <summary>
        /// Storage config
        /// </summary>
        public static StorageConfigSection StorageConfig
        {
            get
            {
                return ConfigurationManager.GetSection(StorageConfigSection.SectionName) as StorageConfigSection;
            }
        }
    }
}