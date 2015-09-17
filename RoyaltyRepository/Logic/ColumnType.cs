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
        /// Add ColumnType to database
        /// </summary>
        /// <param name="instance">ColumnType instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ColumnTypeAdd(ColumnType instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            ColumnTypeAdd(new ColumnType[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add ColumnTypes to database
        /// </summary>
        /// <param name="instances">ColumnType instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ColumnTypeAdd(IEnumerable<ColumnType> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.ColumnTypes.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ColumnTypeAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove ColumnType from database
        /// </summary>
        /// <param name="instance">ColumnType instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ColumnTypeRemove(ColumnType instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            ColumnTypeRemove(new ColumnType[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove ColumnTypes from database
        /// </summary>
        /// <param name="instances">ColumnType instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void ColumnTypeRemove(IEnumerable<ColumnType> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    this.Context.ColumnTypes.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.ColumnTypeRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new ColumnType without any link to database
        /// </summary>
        /// <param name="instanceName">ColumnType name</param>
        /// <returns>ColumnType instance</returns>
        public ColumnType ColumnTypeNew(string instanceName = null)
        {
            try
            {
                var res = new ColumnType() { };
                if (instanceName != null)
                    res.SystemName = instanceName;
                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.ColumnTypeNew(instanceName='{0}')", instanceName ?? "NULL"));
                throw;
            }
        }
        /// <summary>
        /// Get column types
        /// </summary>
        /// <returns>ColumnType queriable collection</returns>
        public IQueryable<ColumnType> ColumnTypeGet()
        {
            return this.Context.ColumnTypes;
        }
        /// <summary>
        /// Get one ColumnType by identifier
        /// </summary>
        /// <param name="instanceId">ColumnType identifier</param>
        /// <returns>ColumnType</returns>
        public ColumnType ColumnTypeGet(long instanceId)
        {
            return ColumnTypeGet(new long[] { instanceId }).FirstOrDefault();
        }
        /// <summary>
        /// Get one ColumnType by name
        /// </summary>
        /// <param name="instanceName">ColumnType name</param>
        /// <returns>ColumnType</returns>
        public ColumnType ColumnTypeGet(string instanceName)
        {
            return ColumnTypeGet(new string[] { instanceName }).FirstOrDefault();
        }
        /// <summary>
        /// Get ColumnTypes by ColumnType names
        /// </summary>
        /// <param name="instanceNames">ColumnType name array</param>
        /// <returns>ColumnType array</returns>
        public IQueryable<ColumnType> ColumnTypeGet(IEnumerable<string> instanceNames)
        {
            return ColumnTypeGet()
                .Join(instanceNames.Select(c => c.ToUpper()), s => s.Name.ToUpper(), i => i, (s, i) => s);
        }
        /// <summary>
        /// Get ColumnTypes by identifiers
        /// </summary>
        /// <param name="instanceIds">ColumnType identifier array</param>
        /// <returns>ColumnType queriable collection</returns>
        public IQueryable<ColumnType> ColumnTypeGet(IEnumerable<long> instanceIds)
        {
            return ColumnTypeGet().Join(instanceIds, s => s.ColumnTypeID, i => i, (s, i) => s);
        }
    }
}
