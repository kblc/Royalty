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
        /// Add ImportQueueRecord to database
        /// </summary>
        /// <param name="instance">ImportQueueRecord instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordAdd(ImportQueueRecord instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordAdd(new ImportQueueRecord[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add ImportQueueRecords to database
        /// </summary>
        /// <param name="instances">ImportQueueRecord instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordAdd(IEnumerable<ImportQueueRecord> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.ImportQueueRecords.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove ImportQueueRecord from database
        /// </summary>
        /// <param name="instance">ImportQueueRecord instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordRemove(ImportQueueRecord instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordRemove(new ImportQueueRecord[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove ImportQueueRecords from database
        /// </summary>
        /// <param name="instances">ImportQueueRecord instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordRemove(IEnumerable<ImportQueueRecord> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.ImportQueueRecords.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new ImportQueueRecord without any link to database
        /// </summary>
        /// <param name="ImportQueueRecordNumber">ImportQueueRecord number</param>
        /// <returns>ImportQueueRecord instance</returns>
        public ImportQueueRecord ImportQueueRecordNew(Account account = null)
        {
            try
            {
                var res = new ImportQueueRecord() 
                { 
                    ImportQueueRecordUID = Guid.NewGuid(), 
                    Account = account,
                    CreatedDate = DateTime.UtcNow,
                };
                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordNew(account='{0}')", account == null ? "NULL" : account.ToString()));
                throw;
            }
        }
        /// <summary>
        /// Get ImportQueueRecords
        /// </summary>
        /// <returns>ImportQueueRecord queriable collection</returns>
        public IQueryable<ImportQueueRecord> ImportQueueRecordGet()
        {
            return this.Context.ImportQueueRecords;
        }
        /// <summary>
        /// Get one ImportQueueRecord by identifier
        /// </summary>
        /// <param name="importQueueRecordId">ImportQueueRecord identifier</param>
        /// <returns>ImportQueueRecord</returns>
        public ImportQueueRecord ImportQueueRecordGet(Guid importQueueRecordId)
        {
            return ImportQueueRecordGet(new Guid[] { importQueueRecordId }).FirstOrDefault();
        }
        /// <summary>
        /// Get ImportQueueRecords by identifiers
        /// </summary>
        /// <param name="instanceIds">ImportQueueRecord identifier array</param>
        /// <returns>ImportQueueRecord queriable collection</returns>
        public IQueryable<ImportQueueRecord> ImportQueueRecordGet(IEnumerable<Guid> instanceIds)
        {
            return ImportQueueRecordGet().Join(instanceIds, s => s.ImportQueueRecordUID, i => i, (s, i) => s);
        }
    }
}
