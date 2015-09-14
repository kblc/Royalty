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
        /// Add File to database
        /// </summary>
        /// <param name="instance">File instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void FileAdd(File instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            FileAdd(new File[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add Files to database
        /// </summary>
        /// <param name="instances">File instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void FileAdd(IEnumerable<File> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Files.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.FileAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove File from database
        /// </summary>
        /// <param name="instance">File instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void FileRemove(File instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            FileRemove(new File[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove Files from database
        /// </summary>
        /// <param name="instances">File instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void FileRemove(IEnumerable<File> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Files.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.FileRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new File without any link to database
        /// </summary>
        /// <returns>File instance</returns>
        public File FileNew()
        {
            try
            {
                var res = new File() { FileID = Guid.NewGuid(), Date = DateTime.UtcNow };
                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.FileNew()"));
                throw;
            }
        }
        /// <summary>
        /// Get Files
        /// </summary>
        /// <returns>File queriable collection</returns>
        public IQueryable<File> FileGet()
        {
            return this.Context.Files;
        }
        /// <summary>
        /// Get one File by identifier
        /// </summary>
        /// <param name="instanceId">File identifier</param>
        /// <returns>File</returns>
        public File FileGet(Guid instanceId)
        {
            return FileGet(new Guid[] { instanceId }).FirstOrDefault();
        }
        /// <summary>
        /// Get Files by identifiers
        /// </summary>
        /// <param name="instanceIds">File identifier array</param>
        /// <returns>File queriable collection</returns>
        public IQueryable<File> FileGet(IEnumerable<Guid> instanceIds)
        {
            return FileGet().Where(a => instanceIds.Contains(a.FileID));
        }
    }
}
