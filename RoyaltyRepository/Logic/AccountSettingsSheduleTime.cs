using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository.Models;
using Helpers;

namespace RoyaltyRepository
{
    public partial class Repository
    {
        /// <summary>
        /// Add AccountSettingsSheduleTime to database
        /// </summary>
        /// <param name="instance">AccountSettingsSheduleTime instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsSheduleTimeAdd(AccountSettingsSheduleTime instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountSettingsSheduleTimeAdd(new AccountSettingsSheduleTime[] { instance }, instance.AccountSettings, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountSettingsSheduleTime to database
        /// </summary>
        /// <param name="instance">AccountSettingsSheduleTime instance</param>
        /// <param name="settings">AccountSettings instance for shedule time</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsSheduleTimeAdd(AccountSettingsSheduleTime instance, AccountSettings settings, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountSettingsSheduleTimeAdd(new AccountSettingsSheduleTime[] { instance }, settings, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountSettingsSheduleTimes to database
        /// </summary>
        /// <param name="instances">AccountSettingsSheduleTime instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsSheduleTimeAdd(IEnumerable<AccountSettingsSheduleTime> instances, AccountSettings settings, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                if (settings == null)
                    throw new ArgumentNullException("settings");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    foreach (var i in instances)
                        if (i.AccountSettings != settings)
                            i.AccountSettings = settings;

                    this.Context.AccountSettingsSheduleTimes.AddRange(instances);
                    if (saveAfterInsert)
                        this.SaveChanges(waitUntilSaving);
                }
                catch (Exception ex)
                {
                    var e = new Exception(ex.Message, ex);
                    for (int i = 0; i < instances.Count();i++)
                        e.Data.Add(string.Format("instance_{0}", i), instances.ElementAt(i).ToString());
                    throw e;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountSettingsSheduleTimeAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountSettingsSheduleTime from database
        /// </summary>
        /// <param name="instance">AccountSettingsSheduleTime instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsSheduleTimeRemove(AccountSettingsSheduleTime instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountSettingsSheduleTimeRemove(new AccountSettingsSheduleTime[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountSettingsSheduleTimes from database
        /// </summary>
        /// <param name="instances">AccountSettingsSheduleTime instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsSheduleTimeRemove(IEnumerable<AccountSettingsSheduleTime> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountSettingsSheduleTimes.RemoveRange(instances);
                    if (saveAfterRemove)
                        this.SaveChanges(waitUntilSaving);
                }
                catch(Exception ex)
                {
                    var e = new Exception(ex.Message, ex);
                    for (int i = 0; i < instances.Count(); i++)
                        e.Data.Add(string.Format("instance_{0}", i), instances.ElementAt(i).ToString());
                    throw e;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountSettingsSheduleTimeRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountSettingsSheduleTime instance without any link to database
        /// </summary>
        /// <returns>AccountSettingsSheduleTime instance</returns>
        public AccountSettingsSheduleTime AccountSettingsSheduleTimeNew(AccountSettings settings = null, TimeSpan? time = null)
        {
            try
            {
                var res = new AccountSettingsSheduleTime();
                if (time.HasValue)
                    res.Time = time.Value;
                if (settings != null)
                    settings.SheduleTimes.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountSettingsSheduleTimeNew()"));
                throw;
            }
        }
    }
}
