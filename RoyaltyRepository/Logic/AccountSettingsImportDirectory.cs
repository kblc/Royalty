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
        /// Add AccountSettingsImportDirectory to database
        /// </summary>
        /// <param name="instance">AccountSettingsImportDirectory instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsImportDirectoryAdd(AccountSettingsImportDirectory instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountSettingsImportDirectoryAdd(new AccountSettingsImportDirectory[] { instance }, instance.AccountSettings, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountSettingsImportDirectory to database
        /// </summary>
        /// <param name="instance">AccountSettingsImportDirectory instance</param>
        /// <param name="settings">AccountSettings instance for shedule time</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsImportDirectoryAdd(AccountSettingsImportDirectory instance, AccountSettings settings, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountSettingsImportDirectoryAdd(new AccountSettingsImportDirectory[] { instance }, settings, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountSettingsImportDirectorys to database
        /// </summary>
        /// <param name="instances">AccountSettingsImportDirectory instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsImportDirectoryAdd(IEnumerable<AccountSettingsImportDirectory> instances, AccountSettings settings, bool saveAfterInsert = true, bool waitUntilSaving = true)
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
                    this.Context.AccountSettingsImportDirectories.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountSettingsImportDirectoryAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountSettingsImportDirectory from database
        /// </summary>
        /// <param name="instance">AccountSettingsImportDirectory instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsImportDirectoryRemove(AccountSettingsImportDirectory instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountSettingsImportDirectoryRemove(new AccountSettingsImportDirectory[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountSettingsImportDirectorys from database
        /// </summary>
        /// <param name="instances">AccountSettingsImportDirectory instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSettingsImportDirectoryRemove(IEnumerable<AccountSettingsImportDirectory> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountSettingsImportDirectories.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountSettingsImportDirectoryRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountSettingsImportDirectory instance without any link to database
        /// </summary>
        /// <returns>AccountSettingsImportDirectory instance</returns>
        public AccountSettingsImportDirectory AccountSettingsImportDirectoryNew(AccountSettings settings = null, object anonymousFiller = null)
        {
            try
            {
                var res = new AccountSettingsImportDirectory()
                {
                    Encoding = Encoding.Default,
                };

                if (anonymousFiller != null)
                    res.FillFromAnonymousType(anonymousFiller);

                if (settings != null)
                    settings.ImportFolders.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountSettingsImportDirectoryNew()"));
                throw;
            }
        }

        /// <summary>
        /// Get account settings shedule times
        /// </summary>
        /// <returns>AccountSettingsImportDirectory queriable collection</returns>
        public IQueryable<AccountSettingsImportDirectory> AccountSettingsImportDirectoryGet()
        {
            return this.Context.AccountSettingsImportDirectories;
        }
    }
}
