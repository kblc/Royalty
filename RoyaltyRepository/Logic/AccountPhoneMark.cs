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
using EntityFramework.Utilities;

namespace RoyaltyRepository
{
    public partial class Repository
    {
        /// <summary>
        /// Add AccountPhoneMark to database
        /// </summary>
        /// <param name="instance">AccountPhoneMark instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountPhoneMarkAdd(AccountPhoneMark instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountPhoneMarkAdd(new AccountPhoneMark[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        
        /// <summary>
        /// Add AccountPhoneMarks to database
        /// </summary>
        /// <param name="instances">AccountPhoneMark instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountPhoneMarkAdd(IEnumerable<AccountPhoneMark> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountPhoneMarks.AddRange(instances);
                    if (saveAfterInsert)
                        this.SaveChanges(waitUntilSaving);
                }
                catch (Exception ex)
                {
                    var e = new Exception(ex.Message, ex);
                    for (int i = 0; i < instances.Count(); i++)
                        e.Data.Add(string.Format("instance_{0}", i), instances.ElementAt(i).ToString());
                    throw e;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountPhoneMarkAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        
        /// <summary>
        /// Remove AccountPhoneMark from database
        /// </summary>
        /// <param name="instance">AccountPhoneMark instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountPhoneMarkRemove(AccountPhoneMark instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountPhoneMarkRemove(new AccountPhoneMark[] { instance }, saveAfterRemove, waitUntilSaving);
        }

        /// <summary>
        /// Remove AccountPhoneMarks from database
        /// </summary>
        /// <param name="instances">AccountPhoneMark instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountPhoneMarkRemove(IEnumerable<AccountPhoneMark> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountPhoneMarks.RemoveRange(instances);
                    if (saveAfterRemove)
                        this.SaveChanges(waitUntilSaving);
                }
                catch (Exception ex)
                {
                    var e = new Exception(ex.Message, ex);
                    for (int i = 0; i < instances.Count(); i++)
                        e.Data.Add(string.Format("instance_{0}", i), instances.ElementAt(i).ToString());
                    throw e;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountPhoneMarkRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }

        /// <summary>
        /// Get AccountPhoneMarks
        /// </summary>
        /// <returns>AccountPhoneMark queriable collection</returns>
        public IQueryable<AccountPhoneMark> AccountPhoneMarkGet()
        {
            return this.Context.AccountPhoneMarks;
        }

        /// <summary>
        /// Create/Get new AccountPhoneMark without any link to database
        /// </summary>
        /// <param name="phone">Phone</param>
        /// <param name="account">Account</param>
        /// <param name="mark">Mark</param>
        /// <returns>AccountPhoneMark instance</returns>
        public AccountPhoneMark AccountPhoneMarkNew(Phone phone = null, Account account = null, Mark mark = null)
        {
            try
            {
                mark = mark ?? MarkGet(MarkTypes.Unknown);
                var res = new AccountPhoneMark() { Account = account, Phone = phone, Mark = mark };

                if (account != null)
                    account.PhoneMarks.Add(res);

                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountPhoneMarkNew(sourceId='{0}',actionType='{1}',sourceType='{2}')", phone.ToString() ?? "NULL", account.ToString() ?? "NULL", mark.ToString() ?? "NULL"));
                throw;
            }
        }
    }
}
