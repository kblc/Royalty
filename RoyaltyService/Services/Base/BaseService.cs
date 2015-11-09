using RoyaltyService.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Base
{
    public abstract class BaseService
    {
        private static Dictionary<InstanceContext, CultureInfo> langDictionary = new Dictionary<InstanceContext, CultureInfo>();

        static BaseService()
        {
            var mapperInitializeMethods = Assembly.GetExecutingAssembly()
                .GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                .Where(mi => mi.GetCustomAttribute(typeof(MapperInitializeAttribute)) != null)
                .ToArray();

            foreach (var mi in mapperInitializeMethods)
                mi.Invoke(null, null);
        }

        public BaseService()
        {
            var removeAction = new Action<InstanceContext>((s) =>
            {
                lock (langDictionary)
                    if (langDictionary.ContainsKey(s))
                        langDictionary.Remove(s);
            });
            OperationContext.Current.InstanceContext.Faulted += (s, e) => removeAction((InstanceContext)s);
            OperationContext.Current.InstanceContext.Closed += (s, e) => removeAction((InstanceContext)s);
        }

        public CultureInfo CurrentCulture
        {
            get
            {
                lock (langDictionary)
                    return (langDictionary.ContainsKey(OperationContext.Current.InstanceContext))
                            ? langDictionary[OperationContext.Current.InstanceContext] ?? Thread.CurrentThread.CurrentCulture
                            : Thread.CurrentThread.CurrentCulture;
            }
            set
            {
                lock (langDictionary)
                    if (langDictionary.ContainsKey(OperationContext.Current.InstanceContext))
                        langDictionary[OperationContext.Current.InstanceContext] = value;
                    else
                        langDictionary.Add(OperationContext.Current.InstanceContext, value);
            }
        }

        private bool verboseLog = Config.Config.ServicesConfig?.VerboseLog ?? false;
        public bool VerboseLog
        {
            get { return verboseLog; }
            set { if (verboseLog == value) return; verboseLog = value; VerboseLogChanged(); }
        }

        protected virtual RoyaltyRepository.Repository GetNewRepository(Helpers.Log.SessionInfo logSession)
        {
            var rep = new RoyaltyRepository.Repository();
            rep.SqlLog += (s, e) => RaiseSqlLog(e);
            rep.Log += (s, e) => logSession.Add(e, "[REPOSITORY]");
            return rep;
        }

        protected long GetLongByString(string longText)
        {
            var res = TryGetLongByString(longText);
            if (res == null)
            {
                var ex = new Exception(Properties.Resources.BASESERVICES_BadIdentifierFormat);
                ex.Data.Add(nameof(longText), longText);
                throw ex;
            }
            return res.Value;
        }
        protected long? TryGetLongByString(string longText)
        {
            long res;
            if (!long.TryParse(longText, out res))
                return null;
            return res;
        }

        protected Guid GetGuidByString(string guidText)
        {
            var res = TryGetGuidByString(guidText);
            if (res == null)
            {
                var ex = new Exception(Properties.Resources.BASESERVICES_BadIdentifierFormat);
                ex.Data.Add(nameof(guidText), guidText);
                throw ex;
            }
            return res.Value;
        }
        protected Guid? TryGetGuidByString(string guidText)
        {
            Guid res;
            if (!Guid.TryParse(guidText, out res))
                if (!Guid.TryParseExact(guidText, "N", out res))
                    return null;
            return res;
        }

        protected virtual void VerboseLogChanged() { }

        protected void UpdateSessionCulture()
        {
            var cultureForSession = CurrentCulture;
            Thread.CurrentThread.CurrentCulture = cultureForSession;
            Thread.CurrentThread.CurrentUICulture = cultureForSession;
        }

        public void ChangeLanguage(string codename)
        {
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    CurrentCulture = new CultureInfo(codename);
                    UpdateSessionCulture();
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(codename), codename);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                }
        }

        #region Log events

        public event EventHandler<string> SqlLog;
        public event EventHandler<string> Log;

        protected void RaiseSqlLog(string logMessage)
        {
            SqlLog?.Invoke(this, logMessage);
            StaticSqlLog?.Invoke(this, logMessage);
        }
        protected void RaiseLog(string logMessage)
        {
            Log?.Invoke(this, logMessage);
            StaticLog?.Invoke(this, logMessage);
        }
        protected void RaiseLog(IEnumerable<string> logMessages)
        {
            foreach (var s in logMessages)
                RaiseLog(s);
        }

        public static event EventHandler<string> StaticSqlLog;
        public event EventHandler<string> StaticLog;

        #endregion
    }
}
