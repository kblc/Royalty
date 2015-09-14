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
        /// Add ImportQueueRecordFile to database
        /// </summary>
        /// <param name="instance">ImportQueueRecordFile instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileAdd(ImportQueueRecordFile instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileAdd(new ImportQueueRecordFile[] { instance }, instance.ImportQueueRecord, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add ImportQueueRecordFile to database
        /// </summary>
        /// <param name="instance">ImportQueueRecordFile instance</param>
        /// <param name="importQueueRecord">ImportQueueRecord instance for instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileAdd(ImportQueueRecordFile instance, ImportQueueRecord importQueueRecord, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileAdd(new ImportQueueRecordFile[] { instance }, importQueueRecord, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add ImportQueueRecordFiles to database
        /// </summary>
        /// <param name="instances">ImportQueueRecordFile instance array</param>
        /// <param name="importQueueRecord">ImportQueueRecord instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileAdd(IEnumerable<ImportQueueRecordFile> instances, ImportQueueRecord importQueueRecord, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                if (importQueueRecord == null)
                    throw new ArgumentNullException("importQueueRecord");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    foreach (var i in instances)
                        i.ImportQueueRecord = importQueueRecord;

                    this.Context.ImportQueueRecordFiles.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove ImportQueueRecordFile from database
        /// </summary>
        /// <param name="instance">ImportQueueRecordFile instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileRemove(ImportQueueRecordFile instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileRemove(new ImportQueueRecordFile[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove ImportQueueRecordFiles from database
        /// </summary>
        /// <param name="instances">ImportQueueRecordFile instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileRemove(IEnumerable<ImportQueueRecordFile> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.ImportQueueRecordFiles.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new ImportQueueRecordFile instance without any link to database
        /// </summary>
        /// <returns>ImportQueueRecordFile instance</returns>
        public ImportQueueRecordFile ImportQueueRecordFileNew(ImportQueueRecord importQueueRecord = null, object anonymousFiller = null)
        {
            try
            {
                var dt = DateTime.UtcNow;
                var res = new ImportQueueRecordFile() { ImportQueueRecordFileUID = Guid.NewGuid() };
                if (anonymousFiller != null)
                    res.FillFromAnonymousType(anonymousFiller);
                importQueueRecord = importQueueRecord ?? res.ImportQueueRecord;
                if (importQueueRecord != null)
                    importQueueRecord.Files.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileNew()"));
                throw;
            }
        }
    }
}
