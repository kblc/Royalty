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
        /// Add AccountDataRecord to database
        /// </summary>
        /// <param name="instance">AccountDataRecord instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdd(AccountDataRecord instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDataRecordAdd(new AccountDataRecord[] { instance }, instance.Account, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDataRecord to database
        /// </summary>
        /// <param name="instance">AccountDataRecord instance</param>
        /// <param name="account">Account instance for instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdd(AccountDataRecord instance, Account account, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDataRecordAdd(new AccountDataRecord[] { instance }, account, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDataRecords to database
        /// </summary>
        /// <param name="instances">AccountDataRecord instance array</param>
        /// <param name="account">Account instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdd(IEnumerable<AccountDataRecord> instances, Account account, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                if (account == null)
                    throw new ArgumentNullException("account");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    foreach (var i in instances)
                        if (i.Account != account)
                            i.Account = account;

                    this.Context.AccountDataRecords.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountDataRecord from database
        /// </summary>
        /// <param name="instance">AccountDataRecord instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordRemove(AccountDataRecord instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountDataRecordRemove(new AccountDataRecord[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountDataRecords from database
        /// </summary>
        /// <param name="instances">AccountDataRecord instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordRemove(IEnumerable<AccountDataRecord> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountDataRecords.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountDataRecord instance without any link to database
        /// </summary>
        /// <returns>AccountDataRecord instance</returns>
        public AccountDataRecord AccountDataRecordNew(Account account = null, object anonymousFiller = null)
        {
            try
            {
                var dt = DateTime.UtcNow;
                var res = new AccountDataRecord()
                {
                    AccountDataRecordID = Guid.NewGuid(),
                    Created = dt,
                    Changed = dt,
                };
                if (anonymousFiller != null)
                    res.FillFromAnonymousType(anonymousFiller);
                account = account ?? res.Account;
                if (account != null)
                    account.Data.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordNew()"));
                throw;
            }
        }
    }
}
