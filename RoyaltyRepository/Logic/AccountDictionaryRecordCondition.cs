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
        /// Add AccountDictionaryRecordCondition to database
        /// </summary>
        /// <param name="instance">AccountDictionaryRecordCondition instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordConditionAdd(AccountDictionaryRecordCondition instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDictionaryRecordConditionAdd(new AccountDictionaryRecordCondition[] { instance }, instance.DictionaryRecord, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDictionaryRecordCondition to database
        /// </summary>
        /// <param name="instance">AccountDictionaryRecordCondition instance</param>
        /// <param name="accountDictionaryRecord">AccountDictionaryRecord instance for instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordConditionAdd(AccountDictionaryRecordCondition instance, AccountDictionaryRecord accountDictionaryRecord, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDictionaryRecordConditionAdd(new AccountDictionaryRecordCondition[] { instance }, accountDictionaryRecord, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDictionaryRecordConditions to database
        /// </summary>
        /// <param name="instances">AccountDictionaryRecordCondition instance array</param>
        /// <param name="accountDictionaryRecord">AccountDictionaryRecord instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordConditionAdd(IEnumerable<AccountDictionaryRecordCondition> instances, AccountDictionaryRecord accountDictionaryRecord, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                if (accountDictionaryRecord == null)
                    throw new ArgumentNullException("accountDictionary");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    foreach (var i in instances)
                        if (i.DictionaryRecord != accountDictionaryRecord)
                            i.DictionaryRecord = accountDictionaryRecord;

                    this.Context.AccountDictionaryRecordConditions.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDictionaryRecordConditionAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountDictionaryRecordCondition from database
        /// </summary>
        /// <param name="instance">AccountDictionaryRecordCondition instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordConditionRemove(AccountDictionaryRecordCondition instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountDictionaryRecordConditionRemove(new AccountDictionaryRecordCondition[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountDictionaryRecordConditions from database
        /// </summary>
        /// <param name="instances">AccountDictionaryRecordCondition instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDictionaryRecordConditionRemove(IEnumerable<AccountDictionaryRecordCondition> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountDictionaryRecordConditions.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDictionaryRecordConditionRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountDictionaryRecordCondition instance without any link to database
        /// </summary>
        /// <returns>AccountDictionaryRecordCondition instance</returns>
        public AccountDictionaryRecordCondition AccountDictionaryRecordConditionNew(AccountDictionaryRecord accountDictionaryRecord = null, long? from = null, long? to = null)
        {
            try
            {
                var res = new AccountDictionaryRecordCondition();
                if (from.HasValue)
                    res.From = from.Value;
                if (to.HasValue)
                    res.To = to.Value;
                if (accountDictionaryRecord != null)
                    accountDictionaryRecord.Conditions.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountDictionaryRecordConditionNew()"));
                throw;
            }
        }
    }
}
