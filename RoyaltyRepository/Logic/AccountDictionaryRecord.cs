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
        /// Add AccountDictionaryRecord to database
        /// </summary>
        /// <param name="instance">AccountDictionaryRecord instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordAdd(AccountDictionaryRecord instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDictionaryRecordAdd(new AccountDictionaryRecord[] { instance }, instance.Dictionary, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDictionaryRecord to database
        /// </summary>
        /// <param name="instance">AccountDictionaryRecord instance</param>
        /// <param name="accountDictionary">AccountDictionary instance for instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordAdd(AccountDictionaryRecord instance, AccountDictionary accountDictionary, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDictionaryRecordAdd(new AccountDictionaryRecord[] { instance }, accountDictionary, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDictionaryRecords to database
        /// </summary>
        /// <param name="instances">AccountDictionaryRecord instance array</param>
        /// <param name="accountDictionary">AccountDictionary instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordAdd(IEnumerable<AccountDictionaryRecord> instances, AccountDictionary accountDictionary, bool saveAfterInsert = true, bool waitUntilSaving = true)
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

                    this.Context.AccountDictionaryRecords.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDictionaryRecordAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountDictionaryRecord from database
        /// </summary>
        /// <param name="instance">AccountDictionaryRecord instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordRemove(AccountDictionaryRecord instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountDictionaryRecordRemove(new AccountDictionaryRecord[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountDictionaryRecords from database
        /// </summary>
        /// <param name="instances">AccountDictionaryRecord instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordRemove(IEnumerable<AccountDictionaryRecord> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountDictionaryRecords.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDictionaryRecordRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountDictionaryRecord instance without any link to database
        /// </summary>
        /// <returns>AccountDictionaryRecord instance</returns>
        public AccountDictionaryRecord AccountDictionaryRecordNew(AccountDictionary accountDictionary = null)
        {
            try
            {
                var res = new AccountDictionaryRecord();
                if (accountDictionary != null)
                    accountDictionary.Records.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountDictionaryRecordNew()"));
                throw;
            }
        }
    }
}
