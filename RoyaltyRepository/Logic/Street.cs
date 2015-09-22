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
using Helpers.Linq;
using RoyaltyRepository.Properties;

namespace RoyaltyRepository
{
    public partial class Repository
    {
        /// <summary>
        /// Add Street to database
        /// </summary>
        /// <param name="instance">Street instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void StreetAdd(Street instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            StreetAdd(new Street[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add Streets to database
        /// </summary>
        /// <param name="instances">Street instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void StreetAdd(IEnumerable<Street> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Streets.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.StreetAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove Street from database
        /// </summary>
        /// <param name="instance">Street instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void StreetRemove(Street instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            StreetRemove(new Street[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove Streets from database
        /// </summary>
        /// <param name="instances">Street instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void StreetRemove(IEnumerable<Street> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    var instancesWithData = instances
                        .Select(s => new { Name = s, Count = s.DataRecords.Count })
                        .Where(s => s.Count > 0);
                    if (instancesWithData.Any())
                        throw new Exception(instancesWithData.Concat(s => string.Format(Resources.Logic_Street_StreetRemove_StreetContainsDataRecords, s, s.Count), Environment.NewLine) + Environment.NewLine + " " + Resources.Logic_Street_StreetRemove_StreetContainsDataRecords_TerminateDeletion);

                    this.Context.Streets.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.StreetRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new Street without any link to database
        /// </summary>
        /// <param name="instanceName">Street name</param>
        /// <param name="area">Area to Street add</param>
        /// <returns>Street instance</returns>
        public Street StreetNew(string instanceName = null, Area area = null)
        {
            try
            {
                var res = new Street() { Area = area, Name = instanceName };
                if (area != null)
                {
//                    if (Context.Entry(area).State != EntityState.Detached)
                        area.Streets.Add(res);
                    //else
                    //    res.Area = area;
                }
                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.StreetNew(instanceName='{0}',area='{1}')", instanceName ?? "NULL", area == null ? "NULL" : area.ToString()));
                throw;
            }
        }
        /// <summary>
        /// Get Streets
        /// </summary>
        /// <returns>Street queriable collection</returns>
        public IQueryable<Street> StreetGet()
        {
            return this.Context.Streets;
        }
        /// <summary>
        /// Get one Street by identifier
        /// </summary>
        /// <param name="instanceId">Street identifier</param>
        /// <returns>Street</returns>
        public Street StreetGet(long instanceId)
        {
            return StreetGet(new long[] { instanceId }).SingleOrDefault();
        }
        /// <summary>
        /// Get Street by name
        /// </summary>
        /// <param name="instanceName">Street name</param>
        /// <returns>Street instance</returns>
        public Street StreetGet(string instanceName, Area area)
        {
            return StreetGet(new string[] { instanceName }, area).SingleOrDefault();
        }
        /// <summary>
        /// Get Streets by Street names
        /// </summary>
        /// <param name="instanceNames">Streets name array</param>
        /// <returns>Street array</returns>
        public IQueryable<Street> StreetGet(IEnumerable<string> instanceNames, Area area)
        {
            var inst = instanceNames.Select(n => n.ToUpper()).Distinct();
            var res = StreetGet()
                    .Where(s => inst.Contains(s.Name.ToUpper()));
            if (area != null)
            {
                //var res2 = res.Where(s => s.AreaID == area.AreaID);
                //var ent = Context.Entry(area);
                //if ((ent.State == EntityState.Added || ent.State == EntityState.Detached) || ((ent.State == EntityState.Modified || ent.State == EntityState.Unchanged) && Context.Entry(area).Collection(nameof(area.Streets)).IsLoaded))
                //{
                    var subRes = area.Streets
                        .Join(inst, s => s.Name.ToUpper(), i => i, (s, i) => s);
                    return subRes.AsQueryable();
                //}
                //else
                //    return res2;
            } else
                return res;
        }
        /// <summary>
        /// Get Streets by identifiers
        /// </summary>
        /// <param name="instanceIds">Street identifier array</param>
        /// <returns>Street queriable collection</returns>
        public IQueryable<Street> StreetGet(IEnumerable<long> instanceIds)
        {
            return StreetGet().Join(instanceIds, s => s.StreetID, i => i, (s,i) => s);
        }
    }
}
