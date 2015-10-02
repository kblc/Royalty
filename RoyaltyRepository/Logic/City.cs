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
        /// Add City to database
        /// </summary>
        /// <param name="instance">City instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void CityAdd(City instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            CityAdd(new City[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add Cities to database
        /// </summary>
        /// <param name="instances">City instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void CityAdd(IEnumerable<City> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Cities.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.CityAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Add cities to database
        /// </summary>
        /// <param name="instances">City instance array</param>
        public void CityAddBulk(IEnumerable<City> instances)
        {
            this.BulkInsert(instances);
        }/// <summary>
         /// Remove City from database
         /// </summary>
         /// <param name="instance">City instance</param>
         /// <param name="saveAfterRemove">Save database after removing</param>
         /// <param name="waitUntilSaving">Wait until saving</param>
        public void CityRemove(City instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            CityRemove(new City[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove Citys from database
        /// </summary>
        /// <param name="instances">City instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void CityRemove(IEnumerable<City> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    AreaRemove(instances.SelectMany(c => c.Areas), saveAfterRemove: false);

                    this.Context.Cities.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.CityRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove cities from database
        /// </summary>
        /// <param name="instances">City instance array</param>
        public void CityRemoveBulk(IEnumerable<City> instances)
        {
            var ids = instances.Select(c => c.CityID);
            BulkDelete<City>(i => ids.Contains(i.CityID));
        }
        /// <summary>
        /// Remove cities from database
        /// </summary>
        /// <param name="instances">City name array</param>
        public void CityRemoveBulk(IEnumerable<string> instances)
        {
            var names = instances.Select(c => c.ToUpper());
            BulkDelete<City>(i => names.Contains(i.Name.ToUpper()));
        }

        /// <summary>
        /// Create/Get new City without any link to database
        /// </summary>
        /// <param name="instanceName">City name</param>
        /// <returns>City instance</returns>
        public City CityNew(string instanceName = null, string phoneNumerCode = null)
        {
            try
            {
                return new City() { Name = instanceName, PhoneNumberCode = phoneNumerCode }; 
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.CityNew(instanceName='{0}')", instanceName ?? "NULL"));
                throw;
            }
        }
        /// <summary>
        /// Get Cities
        /// </summary>
        /// <returns>City queriable collection</returns>
        public IQueryable<City> CityGet()
        {
            return this.Context.Cities;
        }
        /// <summary>
        /// Get one City by identifier
        /// </summary>
        /// <param name="instanceId">City identifier</param>
        /// <returns>City</returns>
        public City CityGet(long instanceId)
        {
            return CityGet(new long[] { instanceId }).FirstOrDefault();
        }
        /// <summary>
        /// Get one City by name
        /// </summary>
        /// <param name="instanceName">City name</param>
        /// <returns>City</returns>
        public City CityGet(string instanceName)
        {
            return CityGet(new string[] { instanceName }).FirstOrDefault();
        }
        /// <summary>
        /// Get Cities by City names
        /// </summary>
        /// <param name="instanceNames">City name array</param>
        /// <returns>City array</returns>
        public IQueryable<City> CityGet(IEnumerable<string> instanceNames)
        {
            var names = instanceNames.Select(n => n.ToUpper());
            return CityGet()
                .Where(c => names.Contains(c.Name.ToUpper()));
                //.Join(instanceNames.Select(c => c.ToUpper()), s => s.Name.ToUpper(), i => i, (s, i) => s);
        }
        /// <summary>
        /// Get Cities by identifiers
        /// </summary>
        /// <param name="instanceIds">City identifier array</param>
        /// <returns>City queriable collection</returns>
        public IQueryable<City> CityGet(IEnumerable<long> instanceIds)
        {
            return CityGet()
                .Where(c => instanceIds.Contains(c.CityID));
                //.Join(instanceIds, s => s.CityID, i => i, (s, i) => s);
        }
    }
}
