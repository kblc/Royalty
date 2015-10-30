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
        public void ImportQueueRecordFileInfoAdd(ImportQueueRecordFileInfo instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileInfoAdd(new ImportQueueRecordFileInfo[] { instance }, instance.ImportQueueRecord, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add ImportQueueRecordFile to database
        /// </summary>
        /// <param name="instance">ImportQueueRecordFile instance</param>
        /// <param name="importQueueRecord">ImportQueueRecord instance for instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileInfoAdd(ImportQueueRecordFileInfo instance, ImportQueueRecord importQueueRecord, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileInfoAdd(new ImportQueueRecordFileInfo[] { instance }, importQueueRecord, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add ImportQueueRecordFiles to database
        /// </summary>
        /// <param name="instances">ImportQueueRecordFile instance array</param>
        /// <param name="importQueueRecord">ImportQueueRecord instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileInfoAdd(IEnumerable<ImportQueueRecordFileInfo> instances, ImportQueueRecord importQueueRecord, bool saveAfterInsert = true, bool waitUntilSaving = true)
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

                    this.Context.ImportQueueRecordFileInfoes.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileInfoAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove ImportQueueRecordFile from database
        /// </summary>
        /// <param name="instance">ImportQueueRecordFile instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileInfoRemove(ImportQueueRecordFileInfo instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileInfoRemove(new ImportQueueRecordFileInfo[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove ImportQueueRecordFiles from database
        /// </summary>
        /// <param name="instances">ImportQueueRecordFile instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileInfoRemove(IEnumerable<ImportQueueRecordFileInfo> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();
                var files = instances.SelectMany(i => i.Files).ToArray();
                try
                {
                    var save = new Action(() =>
                    {
                        this.Context.ImportQueueRecordFileInfoes.RemoveRange(instances);
                        this.ImportQueueRecordFileInfoFileRemove(files, saveAfterRemove);
                    });

                    if (saveAfterRemove)
                        using (BeginTransaction(commitOnDispose: true))
                        {
                            save();
                            this.SaveChanges(waitUntilSaving);
                        }
                    else
                        save();
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileInfoRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new ImportQueueRecordFile instance without any link to database
        /// </summary>
        /// <returns>ImportQueueRecordFile instance</returns>
        public ImportQueueRecordFileInfo ImportQueueRecordFileInfoNew(ImportQueueRecord importQueueRecord = null, object anonymousFiller = null)
        {
            try
            {
                var dt = DateTime.UtcNow;
                var res = new ImportQueueRecordFileInfo()
                { 
                    ImportQueueRecordFileUID = Guid.NewGuid(),
                    ImportQueueRecordState = ImportQueueRecordStateGetDefault(),
                };
                if (anonymousFiller != null)
                    res.FillFromAnonymousType(anonymousFiller);
                importQueueRecord = importQueueRecord ?? res.ImportQueueRecord;
                if (importQueueRecord != null)
                    importQueueRecord.FileInfoes.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileInfoNew()"));
                throw;
            }
        }
    }

    public static partial class RepositoryExtensions
    {
        public static IEnumerable<File> GetFileByType(this ImportQueueRecordFileInfo info, ImportQueueRecordFileInfoFileType fileType)
        {
#pragma warning disable 618
            return info.Files
                .Where(f => string.Compare(f.TypeSystemName, RoyaltyRepository.Models.ImportQueueRecordFileInfoFileType.Import.ToString(), true) == 0)
                .Select(f => f.File);
#pragma warning restore 618
        }
    }
}
