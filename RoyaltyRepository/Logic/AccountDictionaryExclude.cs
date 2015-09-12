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
        /// Add AccountDictionaryExclude to database
        /// </summary>
        /// <param name="instance">AccountDictionaryExclude instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryExcludeAdd(AccountDictionaryExclude instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDictionaryExcludeAdd(new AccountDictionaryExclude[] { instance }, instance.Dictionary, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDictionaryExclude to database
        /// </summary>
        /// <param name="instance">AccountDictionaryExclude instance</param>
        /// <param name="accountDictionary">AccountDictionary instance for instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryExcludeAdd(AccountDictionaryExclude instance, AccountDictionary accountDictionary, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDictionaryExcludeAdd(new AccountDictionaryExclude[] { instance }, accountDictionary, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDictionaryExcludes to database
        /// </summary>
        /// <param name="instances">AccountDictionaryExclude instance array</param>
        /// <param name="accountDictionary">AccountDictionary instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryExcludeAdd(IEnumerable<AccountDictionaryExclude> instances, AccountDictionary accountDictionary, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                if (accountDictionary == null)
                    throw new ArgumentNullException("accountDictionary");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    foreach (var i in instances)
                        if (i.Dictionary != accountDictionary)
                            i.Dictionary = accountDictionary;

                    this.Context.AccountDictionaryExcludes.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDictionaryExcludeAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountDictionaryExclude from database
        /// </summary>
        /// <param name="instance">AccountDictionaryExclude instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryExcludeRemove(AccountDictionaryExclude instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountDictionaryExcludeRemove(new AccountDictionaryExclude[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountDictionaryExcludes from database
        /// </summary>
        /// <param name="instances">AccountDictionaryExclude instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryExcludeRemove(IEnumerable<AccountDictionaryExclude> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountDictionaryExcludes.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDictionaryExcludeRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountDictionaryExclude instance without any link to database
        /// </summary>
        /// <returns>AccountDictionaryExclude instance</returns>
        public AccountDictionaryExclude AccountDictionaryExcludeNew(AccountDictionary accountDictionary = null, string exclude = null)
        {
            try
            {
                var res = new AccountDictionaryExclude();
                if (exclude != null)
                    res.Exclude = exclude;
                if (accountDictionary != null)
                    accountDictionary.Excludes.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountDictionaryExcludeNew()"));
                throw;
            }
        }
    }
}
