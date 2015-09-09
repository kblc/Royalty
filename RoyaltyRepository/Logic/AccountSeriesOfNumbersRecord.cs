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
        /// Add AccountSeriesOfNumbersRecord to database
        /// </summary>
        /// <param name="instance">AccountSeriesOfNumbersRecord instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSeriesOfNumbersRecordAdd(AccountSeriesOfNumbersRecord instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountSeriesOfNumbersRecordAdd(new AccountSeriesOfNumbersRecord[] { instance }, instance.Account, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountSeriesOfNumbersRecord to database
        /// </summary>
        /// <param name="instance">AccountSeriesOfNumbersRecord instance</param>
        /// <param name="account">Account instance for shedule time</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSeriesOfNumbersRecordAdd(AccountSeriesOfNumbersRecord instance, Account account, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountSeriesOfNumbersRecordAdd(new AccountSeriesOfNumbersRecord[] { instance }, account, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountSeriesOfNumbersRecord to database
        /// </summary>
        /// <param name="instances">AccountSeriesOfNumbersRecord instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSeriesOfNumbersRecordAdd(IEnumerable<AccountSeriesOfNumbersRecord> instances, Account account, bool saveAfterInsert = true, bool waitUntilSaving = true)
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

                    this.Context.AccountSeriesOfNumbersRecords.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountSeriesOfNumbersRecordAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountSeriesOfNumbersRecord from database
        /// </summary>
        /// <param name="instance">AccountSeriesOfNumbersRecord instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSeriesOfNumbersRecordRemove(AccountSeriesOfNumbersRecord instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountSeriesOfNumbersRecordRemove(new AccountSeriesOfNumbersRecord[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountSeriesOfNumbersRecord from database
        /// </summary>
        /// <param name="instances">AccountSeriesOfNumbersRecord instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountSeriesOfNumbersRecordRemove(IEnumerable<AccountSeriesOfNumbersRecord> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountSeriesOfNumbersRecords.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountSeriesOfNumbersRecordRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountSeriesOfNumbersRecord instance without any link to database
        /// </summary>
        /// <returns>AccountSeriesOfNumbersRecord instance</returns>
        public AccountSeriesOfNumbersRecord AccountSeriesOfNumbersRecordNew(Account account = null, TimeSpan? dalay = null, long? digitCount = null)
        {
            try
            {
                var res = new AccountSeriesOfNumbersRecord();
                if (dalay.HasValue)
                    res.Delay = dalay.Value;
                if (digitCount.HasValue)
                    res.DigitCount = digitCount.Value;
                if (account != null)
                    account.SeriesOfNumbers.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountSeriesOfNumbersRecordNew()"));
                throw;
            }
        }
    }
}
