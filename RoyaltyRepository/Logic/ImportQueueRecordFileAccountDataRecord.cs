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
        /// Add ImportQueueRecordFileAccountDataRecord to database
        /// </summary>
        /// <param name="instance">ImportQueueRecordFileAccountDataRecord instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileAccountDataRecordAdd(ImportQueueRecordFileAccountDataRecord instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileAccountDataRecordAdd(new ImportQueueRecordFileAccountDataRecord[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add ImportQueueRecordFileAccountDataRecords to database
        /// </summary>
        /// <param name="instances">ImportQueueRecordFileAccountDataRecord instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileAccountDataRecordAdd(IEnumerable<ImportQueueRecordFileAccountDataRecord> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException(nameof(instances));
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    this.Context.ImportQueueRecordFileAccountDataRecords.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileAccountDataRecordAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }  
        /// <summary>
        /// Remove ImportQueueRecordFileAccountDataRecord from database
        /// </summary>
        /// <param name="instance">ImportQueueRecordFileAccountDataRecord instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileAccountDataRecordRemove(ImportQueueRecordFileAccountDataRecord instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileAccountDataRecordRemove(new ImportQueueRecordFileAccountDataRecord[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove ImportQueueRecordFileAccountDataRecords from database
        /// </summary>
        /// <param name="instances">ImportQueueRecordFileAccountDataRecord instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileAccountDataRecordRemove(IEnumerable<ImportQueueRecordFileAccountDataRecord> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    this.Context.ImportQueueRecordFileAccountDataRecords.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileAccountDataRecordRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new ImportQueueRecordFileAccountDataRecord instance without any link to database
        /// </summary>
        /// <returns>ImportQueueRecordFileAccountDataRecord instance</returns>
        public ImportQueueRecordFileAccountDataRecord ImportQueueRecordFileAccountDataRecordNew(ImportQueueRecordFileInfo importQueueRecordFile = null, AccountDataRecord accountDataRecord = null)
        {
            try
            {
                var dt = DateTime.UtcNow;
                var res = new ImportQueueRecordFileAccountDataRecord()
                { 
                    ImportQueueRecordFile = importQueueRecordFile,
                    AccountDataRecord = accountDataRecord,
                    LoadDate = dt,
                };

                if (importQueueRecordFile != null)
                    importQueueRecordFile.LoadedRecords.Add(res);

                if (accountDataRecord != null)
                    accountDataRecord.LoadedByQueueFiles.Add(res);

                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileAccountDataRecordNew()"));
                throw;
            }
        }
    }
}
