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
        /// Add Host to database
        /// </summary>
        /// <param name="instance">Host instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HostAdd(Host instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            HostAdd(new Host[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add Hosts to database
        /// </summary>
        /// <param name="instances">Host instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HostAdd(IEnumerable<Host> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Hosts.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.HostAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove Host from database
        /// </summary>
        /// <param name="instance">Host instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HostRemove(Host instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            HostRemove(new Host[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove Hosts from database
        /// </summary>
        /// <param name="instances">Host instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void HostRemove(IEnumerable<Host> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Hosts.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.HostRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new Host without any link to database
        /// </summary>
        /// <param name="HostNumber">Host number</param>
        /// <returns>Host instance</returns>
        public Host HostNew(string hostName = null)
        {
            try
            {
                var res = new Host() { };
                if (hostName != null)
                    res.Name = hostName;
                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.HostNew(hostName='{0}')", hostName ?? "NULL"));
                throw;
            }
        }
        /// <summary>
        /// Get Hosts
        /// </summary>
        /// <returns>Host queriable collection</returns>
        public IQueryable<Host> HostGet()
        {
            return this.Context.Hosts;
        }
        /// <summary>
        /// Get one Host by identifier
        /// </summary>
        /// <param name="HostId">Host identifier</param>
        /// <returns>Host</returns>
        public Host HostGet(long hostId)
        {
            return HostGet(new long[] { hostId }).FirstOrDefault();
        }
        /// <summary>
        /// Get one Host by Host name
        /// </summary>
        /// <param name="hostName">Host number</param>
        /// <returns>Host</returns>
        public Host HostGet(string hostName)
        {
            return HostGet(new string[] { hostName }).FirstOrDefault();
        }
        /// <summary>
        /// Get Hosts by Host names
        /// </summary>
        /// <param name="hostNames">Host name array</param>
        /// <returns>Host array</returns>
        public IQueryable<Host> HostGet(IEnumerable<string> hostNames)
        {
            return HostGet().Where(a => hostNames.Any(pn => string.Compare(pn, a.Name, true) == 0));
        }
        /// <summary>
        /// Get Hosts by identifiers
        /// </summary>
        /// <param name="HostId">Host identifier array</param>
        /// <returns>Host queriable collection</returns>
        public IQueryable<Host> HostGet(IEnumerable<long> hostId)
        {
            return HostGet().Where(a => hostId.Contains(a.HostID));
        }
    }
}
