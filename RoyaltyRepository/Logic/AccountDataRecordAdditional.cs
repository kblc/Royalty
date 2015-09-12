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
        /// Add AccountDataRecordAdditional to database
        /// </summary>
        /// <param name="instance">AccountDataRecordAdditional instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalAdd(AccountDataRecordAdditional instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDataRecordAdditionalAdd(new AccountDataRecordAdditional[] { instance }, instance.AccountDataRecord, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDataRecordAdditional to database
        /// </summary>
        /// <param name="instance">AccountDataRecordAdditional instance</param>
        /// <param name="accountDataRecord">AccountDataRecord instance for instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalAdd(AccountDataRecordAdditional instance, AccountDataRecord accountDataRecord, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDataRecordAdditionalAdd(new AccountDataRecordAdditional[] { instance }, accountDataRecord, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDataRecordAdditionals to database
        /// </summary>
        /// <param name="instances">AccountDataRecordAdditional instance array</param>
        /// <param name="accountDataRecord">AccountDataRecord instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalAdd(IEnumerable<AccountDataRecordAdditional> instances, AccountDataRecord accountDataRecord, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                if (accountDataRecord == null)
                    throw new ArgumentNullException("accountDataRecord");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    foreach (var i in instances)
                        if (i.AccountDataRecord != accountDataRecord)
                            i.AccountDataRecord = accountDataRecord;

                    this.Context.AccountDataRecordAdditionals.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordAdditionalAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountDataRecordAdditional from database
        /// </summary>
        /// <param name="instance">AccountDataRecordAdditional instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalRemove(AccountDataRecordAdditional instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountDataRecordAdditionalRemove(new AccountDataRecordAdditional[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountDataRecordAdditionals from database
        /// </summary>
        /// <param name="instances">AccountDataRecordAdditional instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalRemove(IEnumerable<AccountDataRecordAdditional> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountDataRecordAdditionals.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordAdditionalRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountDataRecordAdditional instance without any link to database
        /// </summary>
        /// <returns>AccountDataRecordAdditional instance</returns>
        public AccountDataRecordAdditional AccountDataRecordAdditionalNew(AccountDataRecord accountDataRecord = null)
        {
            try
            {
                var res = new AccountDataRecordAdditional();
                if (accountDataRecord != null)
                    accountDataRecord.DataAdditional = res;
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordAdditionalNew()"));
                throw;
            }
        }
    }
}
