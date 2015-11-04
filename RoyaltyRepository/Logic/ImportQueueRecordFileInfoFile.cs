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
        public void ImportQueueRecordFileInfoFileAdd(ImportQueueRecordFileInfoFile instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileInfoFileAdd(new ImportQueueRecordFileInfoFile[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add ImportQueueRecordFiles to database
        /// </summary>
        /// <param name="instances">ImportQueueRecordFile instance array</param>
        /// <param name="importQueueRecord">ImportQueueRecord instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileInfoFileAdd(IEnumerable<ImportQueueRecordFileInfoFile> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    this.Context.ImportQueueRecordFileInfoFiles.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileInfoFileAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove ImportQueueRecordFile from database
        /// </summary>
        /// <param name="instance">ImportQueueRecordFile instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileInfoFileRemove(ImportQueueRecordFileInfoFile instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            ImportQueueRecordFileInfoFileRemove(new ImportQueueRecordFileInfoFile[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove ImportQueueRecordFiles from database
        /// </summary>
        /// <param name="instances">ImportQueueRecordFile instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ImportQueueRecordFileInfoFileRemove(IEnumerable<ImportQueueRecordFileInfoFile> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();
                var files = instances.Select(i => i.File);
                try
                {
                    var save = new Action(() =>
                    {
                        this.Context.ImportQueueRecordFileInfoFiles.RemoveRange(instances);
                        this.RemoveRange(files, saveAfterRemove);
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
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileInfoFileRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new ImportQueueRecordFile instance without any link to database
        /// </summary>
        /// <returns>ImportQueueRecordFile instance</returns>
        public ImportQueueRecordFileInfoFile ImportQueueRecordFileInfoFileNew(ImportQueueRecordFileInfoFileType fileType, ImportQueueRecordFileInfo importQueueRecordFileInfo = null, File file = null)
        {
            try
            {
                var dt = DateTime.UtcNow;
                var res = new ImportQueueRecordFileInfoFile()
                { 
                    Type = fileType,
                    ImportQueueRecordFileInfo = importQueueRecordFileInfo,
                    File = file
                };

                if (importQueueRecordFileInfo != null)
                    importQueueRecordFileInfo.Files.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.ImportQueueRecordFileInfoFileNew()"));
                throw;
            }
        }
    }
}
