using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyWorker.Config
{
    public class WorkerConfigSection : ConfigurationSection
    {
        public const string SectionName = "workerConfig";

        internal const string DefaultEncoding = "cp1251";
        internal const string DefaultExportFileNameValue = "export.csv";
        internal const string DefaultImportLogFileNamValue = "fileimport.log";
        internal const string DefaultNotTrustedPhonesSuffixValue = "phones";
        internal const string DefaultCultureForExportThreadValue = "ru-RU";

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

        [ConfigurationProperty("defaultExportFileName", IsRequired = false, DefaultValue = DefaultExportFileNameValue)]
        public string DefaultExportFileName
        {
            get { return this["defaultExportFileName"] as string; }
        }

        [ConfigurationProperty("defaultImportLogFileName", IsRequired = false, DefaultValue = DefaultImportLogFileNamValue)]
        public string DefaultImportLogFileName
        {
            get { return this["defaultImportLogFileName"] as string; }
        }

        [ConfigurationProperty("defaultNotTrustedPhonesSuffix", IsRequired = false, DefaultValue = DefaultNotTrustedPhonesSuffixValue)]
        public string DefaultNotTrustedPhonesSuffix
        {
            get { return this["defaultNotTrustedPhonesSuffix"] as string; }
        }

        [ConfigurationProperty("defaultCultureForExportThread", IsRequired = false, DefaultValue = DefaultCultureForExportThreadValue)]
        public CultureInfo DefaultCultureForExportThread
        {
            get
            {
                try
                {
                    var cultureName = this["defaultCultureForExportThread"] as string;
                    return new System.Globalization.CultureInfo(cultureName);
                }
                catch
                {
                    return System.Threading.Thread.CurrentThread.CurrentCulture;
                }
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
