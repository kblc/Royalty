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
        /// Add History to database
        /// </summary>
        /// <param name="instance">History instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HistoryAdd(History instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            HistoryAdd(new History[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        
        /// <summary>
        /// Add History to database
        /// </summary>
        /// <param name="instances">History instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HistoryAdd(IEnumerable<History> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.History.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.HistoryAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        
        /// <summary>
        /// Remove History from database
        /// </summary>
        /// <param name="instance">History instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HistoryRemove(History instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            HistoryRemove(new History[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        
        /// <summary>
        /// Remove Historys from database
        /// </summary>
        /// <param name="instances">History instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HistoryRemove(IEnumerable<History> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    this.Context.History.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.HistoryRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }

        /// <summary>
        /// Create/Get new History without any link to database
        /// </summary>
        /// <param name="actionType">History action name</param>
        /// <param name="sourceType">History source name</param>
        /// <param name="sourceId">History source identifier</param>
        /// <returns>History instance</returns>
        public History HistoryNew(HistoryActionType actionType, HistorySourceType sourceType, string sourceId = null)
        {
            try
            {
                return new History() { ActionType = actionType, SourceType = sourceType, SourceID = sourceId };
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.HistoryNew(sourceId='{0}',actionType='{1}',sourceType='{2}')", sourceId ?? "NULL", actionType, sourceType));
                throw;
            }
        }

        /// <summary>
        /// Create/Get new History without any link to database
        /// </summary>
        /// <param name="source">History source</param>
        /// <param name="actionType">History action name</param>
        /// <returns>History instance</returns>
        public History HistoryNew(IHistoryRecordSource source, HistoryActionType actionType)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return HistoryNew(actionType, source.SourceType, source.SourceId.ToString());
        }
        
        /// <summary>
        /// Get History
        /// </summary>
        /// <returns>History queriable collection</returns>
        public IQueryable<History> HistoryGet()
        {
            return this.Context.History;
        }
        
        /// <summary>
        /// Get one History by identifier
        /// </summary>
        /// <param name="instanceId">History identifier</param>
        /// <returns>History</returns>
        public History HistoryGet(long instanceId)
        {
            return HistoryGet(new long[] { instanceId }).FirstOrDefault();
        }
        
        /// <summary>
        /// Get History by identifiers
        /// </summary>
        /// <param name="instanceIds">History identifier array</param>
        /// <returns>History queriable collection</returns>
        public IQueryable<History> HistoryGet(IEnumerable<long> instanceIds)
        {
            return HistoryGet()
                .Where(c => instanceIds.Contains(c.HistoryID));
                //.Join(instanceIds, s => s.HistoryID, i => i, (s, i) => s);
        }
    }
}
