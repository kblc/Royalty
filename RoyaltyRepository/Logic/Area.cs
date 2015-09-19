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
        /// Add Area to database
        /// </summary>
        /// <param name="instance">Area instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AreaAdd(Area instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AreaAdd(new Area[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add areas to database
        /// </summary>
        /// <param name="instances">Area instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AreaAdd(IEnumerable<Area> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Areas.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AreaAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove Area from database
        /// </summary>
        /// <param name="instance">Area instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AreaRemove(Area instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AreaRemove(new Area[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove areas from database
        /// </summary>
        /// <param name="instances">Area instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AreaRemove(IEnumerable<Area> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Areas.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AreaRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new Area without any link to database
        /// </summary>
        /// <param name="instanceName">Area name</param>
        /// <returns>Area instance</returns>
        public Area AreaNew(string instanceName = null, bool isDefault = false, City city = null)
        {
            try
            {
                var res = new Area() { Name = instanceName, City = city };
                if (city != null)
                {
                    //if (Context.Entry(city).State != EntityState.Detached)
                        city.Areas.Add(res);
                    //else
                    //    res.City = city;
                }
                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AreaNew(instanceName='{0}',city='{1}')", instanceName ?? "NULL", city == null ? "NULL" : city.ToString()));
                throw;
            }
        }
        /// <summary>
        /// Get areas
        /// </summary>
        /// <returns>Area queriable collection</returns>
        public IQueryable<Area> AreaGet()
        {
            return this.Context.Areas;
        }
        /// <summary>
        /// Get one Area by identifier
        /// </summary>
        /// <param name="instanceId">Area identifier</param>
        /// <returns>Area</returns>
        public Area AreaGet(long instanceId)
        {
            return AreaGet(new long[] { instanceId }).SingleOrDefault();
        }
        /// <summary>
        /// Get one area by name
        /// </summary>
        /// <param name="instanceName">Area name</param>
        /// <returns>Area</returns>
        public Area AreaGet(string instanceName, City city)
        {
            return AreaGet(new string[] { instanceName }, city).SingleOrDefault();
        }
        /// <summary>
        /// Get areas by names
        /// </summary>
        /// <param name="instanceNames">Area name array</param>
        /// <returns>Area array</returns>
        public IQueryable<Area> AreaGet(IEnumerable<string> instanceNames, City city)
        {
            var res = city.Areas
                .Join(instanceNames.Select(c => c.ToUpper()), s => s.Name.ToUpper(), i => i, (s, i) => s);
            if (city != null)
                res = res.Join(new long[] { city.CityID }, a => a.CityID, i => i, (a,i) => a);
            return res.AsQueryable();
        }
        /// <summary>
        /// Get areas by identifiers
        /// </summary>
        /// <param name="instanceIds">Area identifier array</param>
        /// <returns>Area queriable collection</returns>
        public IQueryable<Area> AreaGet(IEnumerable<long> instanceIds)
        {
            return AreaGet().Join(instanceIds, s => s.AreaID, i => i, (s, i) => s);
        }
    }
}
