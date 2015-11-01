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

        internal const string DefaultEncoding = "utf-8";
        internal const string DefaultExportFileNameValue = "export.csv";
        internal const string DefaultImportLogFileNamValue = "fileimport.log";
        internal const string DefaultCultureForExportThreadValue = "ru-RU";
        internal const string DefaultTimerInterval = "00:00:30.000";
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

        [ConfigurationProperty("logFileEncodingName", IsRequired = false, DefaultValue = DefaultEncoding)]
        public string LogFileEncodingName
        {
            get { return this["logFileEncodingName"] as string; }
        }

        public Encoding LogFileEncoding
        {
            get
            {
                try
                {
                    return Encoding.GetEncoding(LogFileEncodingName);
                } catch { return Encoding.GetEncoding(DefaultEncoding); }
            }
        }

        [ConfigurationProperty("exportFileEncodingName", IsRequired = false, DefaultValue = DefaultEncoding)]
        public string ExportFileEncodingName
        {
            get { return this["exportFileEncodingName"] as string; }
        }

        public Encoding ExportFileEncoding
        {
            get
            {
                try
                {
                    return Encoding.GetEncoding(ExportFileEncodingName);
                }
                catch { return Encoding.GetEncoding(DefaultEncoding); }
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

        [ConfigurationProperty("defaultCultureForExportThread", IsRequired = false, DefaultValue = DefaultCultureForExportThreadValue)]
        public CultureInfo DefaultCultureForExportThread
        {
            get
            {
                try
                {
                    return this["defaultCultureForExportThread"] as CultureInfo;
                }
                catch
                {
                    return new CultureInfo(DefaultCultureForExportThreadValue);
                }
            }
        }

        [ConfigurationProperty("verboseLog", IsRequired = false, DefaultValue = false)]
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
