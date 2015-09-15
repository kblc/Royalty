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
        /// Add phone to database
        /// </summary>
        /// <param name="instance">Phone instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void PhoneAdd(Phone instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            PhoneAdd(new Phone[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add phones to database
        /// </summary>
        /// <param name="instances">Phone instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void PhoneAdd(IEnumerable<Phone> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Phones.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.PhoneAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove phone from database
        /// </summary>
        /// <param name="instance">Phone instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void PhoneRemove(Phone instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            PhoneRemove(new Phone[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove phones from database
        /// </summary>
        /// <param name="instances">Phone instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void PhoneRemove(IEnumerable<Phone> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Phones.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.PhoneRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new phone without any link to database
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <returns>Phone instance</returns>
        public Phone PhoneNew(string phoneNumber = null)
        {
            try
            {
                var res = new Phone() { };
                if (phoneNumber != null)
                    res.PhoneNumber = phoneNumber;
                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.PhoneNew(phoneNumber='{0}')", phoneNumber ?? "NULL"));
                throw;
            }
        }
        /// <summary>
        /// Get phones
        /// </summary>
        /// <returns>Phone queriable collection</returns>
        public IQueryable<Phone> PhoneGet()
        {
            return this.Context.Phones;
        }
        /// <summary>
        /// Get one phone by identifier
        /// </summary>
        /// <param name="phoneId">Phone identifier</param>
        /// <returns>Phone</returns>
        public Phone PhoneGet(long phoneId)
        {
            return PhoneGet(new long[] { phoneId }).FirstOrDefault();
        }
        /// <summary>
        /// Get one phone by phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <returns>Phone</returns>
        public Phone PhoneGet(string phoneNumber)
        {
            return PhoneGet(new string[] { phoneNumber }).FirstOrDefault();
        }
        /// <summary>
        /// Get phones by phone numbers
        /// </summary>
        /// <param name="phoneNumbers">Phone number array</param>
        /// <returns>Phone array</returns>
        public IQueryable<Phone> PhoneGet(IEnumerable<string> phoneNumbers)
        {
            return PhoneGet().Where(a => phoneNumbers.Any(pn => string.Compare(pn, a.PhoneNumber, true) == 0));
        }
        /// <summary>
        /// Get phones by identifiers
        /// </summary>
        /// <param name="instanceIds">Phone identifier array</param>
        /// <returns>Phone queriable collection</returns>
        public IQueryable<Phone> PhoneGet(IEnumerable<long> instanceIds)
        {
            return PhoneGet().Join(instanceIds, s => s.PhoneID, i => i, (s, i) => s);
        }
    }
}
