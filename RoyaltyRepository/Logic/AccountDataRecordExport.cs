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
        /// Add AccountDataRecordExport to database
        /// </summary>
        /// <param name="instance">AccountDataRecordExport instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordExportAdd(AccountDataRecordExport instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDataRecordExportAdd(new AccountDataRecordExport[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDataRecordExports to database
        /// </summary>
        /// <param name="instances">AccountDataRecordExport instance array</param>
        /// <param name="account">Account instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordExportAdd(IEnumerable<AccountDataRecordExport> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");

                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    this.Context.AccountDataRecordExport.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordExportAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountDataRecordExport from database
        /// </summary>
        /// <param name="instance">AccountDataRecordExport instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordExportRemove(AccountDataRecordExport instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountDataRecordExportRemove(new AccountDataRecordExport[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountDataRecordExports from database
        /// </summary>
        /// <param name="instances">AccountDataRecordExport instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordExportRemove(IEnumerable<AccountDataRecordExport> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountDataRecordExport.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordExportRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountDataRecordExport instance without any link to database
        /// </summary>
        /// <returns>AccountDataRecordExport instance</returns>
        public AccountDataRecordExport AccountDataRecordExportNew(AccountDataRecord accountDataRecord = null, File file = null, Host host = null)
        {
            try
            {
                var dt = DateTime.UtcNow;
                var res = new AccountDataRecordExport()
                {
                    ExportDate = DateTime.UtcNow,
                    AccountDataRecord = accountDataRecord,
                    File = file,
                    Host = host
                };
                if (accountDataRecord != null)
                    accountDataRecord.ExportInfo.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordExportNew()"));
                throw;
            }
        }
    }
}
