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
using RoyaltyRepository.Extensions;

namespace RoyaltyRepository
{
    public partial class Repository
    {
        /// <summary>
        /// Add AccountSettingsExportDirectory to database
        /// </summary>
        /// <param name="instance">AccountSettingsExportDirectory instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsExportDirectoryAdd(AccountSettingsExportDirectory instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountSettingsExportDirectoryAdd(new AccountSettingsExportDirectory[] { instance }, instance.AccountSettings, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountSettingsExportDirectory to database
        /// </summary>
        /// <param name="instance">AccountSettingsExportDirectory instance</param>
        /// <param name="settings">AccountSettings instance for shedule time</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsExportDirectoryAdd(AccountSettingsExportDirectory instance, AccountSettings settings, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountSettingsExportDirectoryAdd(new AccountSettingsExportDirectory[] { instance }, settings, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountSettingsExportDirectorys to database
        /// </summary>
        /// <param name="instances">AccountSettingsExportDirectory instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsExportDirectoryAdd(IEnumerable<AccountSettingsExportDirectory> instances, AccountSettings settings, bool saveAfterInsert = true, bool waitUntilSaving = true)
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
                    this.Context.AccountSettingsExportDirectories.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountSettingsExportDirectoryAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountSettingsExportDirectory from database
        /// </summary>
        /// <param name="instance">AccountSettingsExportDirectory instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsExportDirectoryRemove(AccountSettingsExportDirectory instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountSettingsExportDirectoryRemove(new AccountSettingsExportDirectory[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountSettingsExportDirectorys from database
        /// </summary>
        /// <param name="instances">AccountSettingsExportDirectory instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsExportDirectoryRemove(IEnumerable<AccountSettingsExportDirectory> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountSettingsExportDirectories.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountSettingsExportDirectoryRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountSettingsExportDirectory instance without any link to database
        /// </summary>
        /// <returns>AccountSettingsExportDirectory instance</returns>
        public AccountSettingsExportDirectory AccountSettingsExportDirectoryNew(AccountSettings settings = null, object anonymousFiller = null)
        {
            try
            {
                var res = new AccountSettingsExportDirectory()
                {
                    Encoding = Encoding.Default,
                };

                if (anonymousFiller != null)
                    res.FillFromAnonymousType(anonymousFiller);

                if (settings != null)
                    settings.ExportDirectories.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountSettingsExportDirectoryNew()"));
                throw;
            }
        }

        /// <summary>
        /// Get account settings shedule times
        /// </summary>
        /// <returns>AccountSettingsExportDirectory queriable collection</returns>
        public IQueryable<AccountSettingsExportDirectory> AccountSettingsExportDirectoryGet()
        {
            return this.Context.AccountSettingsExportDirectories;
        }
    }
}
