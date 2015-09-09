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
        /// Add AccountExportType to database
        /// </summary>
        /// <param name="instance">AccountExportType instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountExportTypeAdd(AccountExportType instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountExportTypeAdd(new AccountExportType[] { instance }, instance.Account, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountExportType to database
        /// </summary>
        /// <param name="instance">AccountExportType instance</param>
        /// <param name="account">Account instance for shedule time</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountExportTypeAdd(AccountExportType instance, Account account, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountExportTypeAdd(new AccountExportType[] { instance }, account, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountExportType to database
        /// </summary>
        /// <param name="instances">AccountExportType instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountExportTypeAdd(IEnumerable<AccountExportType> instances, Account account, bool saveAfterInsert = true, bool waitUntilSaving = true)
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

                    this.Context.AccountExportTypes.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountExportTypeAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountExportType from database
        /// </summary>
        /// <param name="instance">AccountExportType instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountExportTypeRemove(AccountExportType instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountExportTypeRemove(new AccountExportType[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountExportType from database
        /// </summary>
        /// <param name="instances">AccountExportType instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountExportTypeRemove(IEnumerable<AccountExportType> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountExportTypes.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountExportTypeRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountExportType instance without any link to database
        /// </summary>
        /// <returns>AccountExportType instance</returns>
        public AccountExportType AccountExportTypeNew(Account account = null, string fileName = null, Mark mark = null)
        {
            try
            {
                var res = new AccountExportType();
                if (mark != null)
                    res.Mark = mark;
                if (fileName != null)
                    res.FileName = fileName;
                if (account != null)
                    account.ExportTypes.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountExportTypeNew()"));
                throw;
            }
        }
    }
}
