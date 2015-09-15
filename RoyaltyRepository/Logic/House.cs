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
        /// Add House to database
        /// </summary>
        /// <param name="instance">House instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HouseAdd(House instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            HouseAdd(new House[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add Houses to database
        /// </summary>
        /// <param name="instances">House instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HouseAdd(IEnumerable<House> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Houses.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.HouseAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove House from database
        /// </summary>
        /// <param name="instance">House instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HouseRemove(House instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            HouseRemove(new House[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove Houses from database
        /// </summary>
        /// <param name="instances">House instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HouseRemove(IEnumerable<House> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Houses.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.HouseRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new House without any link to database
        /// </summary>
        /// <param name="instanceNumber">House number</param>
        /// <param name="street">Area to House add</param>
        /// <returns>House instance</returns>
        public House HouseNew(string instanceNumber = null, Street street = null)
        {
            try
            {
                var res = new House() { };
                if (instanceNumber != null)
                    res.Number = instanceNumber;
                if (street != null)
                {
                    if (Context.Entry(street).State != EntityState.Detached)
                        street.Houses.Add(res);
                    else
                        res.Street = street;
                }
                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.HouseNew(instanceName='{0}',area='{1}')", instanceNumber ?? "NULL", street == null ? "NULL" : street.ToString()));
                throw;
            }
        }
        /// <summary>
        /// Get Houses
        /// </summary>
        /// <returns>House queriable collection</returns>
        public IQueryable<House> HouseGet()
        {
            return this.Context.Houses;
        }
        /// <summary>
        /// Get one House by identifier
        /// </summary>
        /// <param name="instanceId">House identifier</param>
        /// <returns>House</returns>
        public House HouseGet(long instanceId)
        {
            return HouseGet(new long[] { instanceId }).FirstOrDefault();
        }
        /// <summary>
        /// Get Houses by identifiers
        /// </summary>
        /// <param name="instanceIds">House identifier array</param>
        /// <returns>House queriable collection</returns>
        public IQueryable<House> HouseGet(IEnumerable<long> instanceIds)
        {
            return HouseGet().Join(instanceIds, s => s.HouseID, i => i, (s,i) => s);
        }
    }
}
