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
using System.Transactions;

namespace RoyaltyRepository
{
    public partial class Repository
    {
        public DbSet<T> GetDbSet<T>()
            where T : class
        {
            var res = Context.GetType().GetProperties()
                .Select(p => p.GetValue(Context) as DbSet<T>)
                .FirstOrDefault(p => p != null);

            if (res == null)
                throw new Exception($"Database set not found for entity type '{typeof(T).Name}'");

            return res;
        }

        /// <summary>
        /// Add item to database
        /// </summary>
        /// <param name="instance">Item instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void Add<T>(T instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
            where T : class
        {
            AddRange(new T[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        
        /// <summary>
        /// Add items to database
        /// </summary>
        /// <param name="instances">Instances array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AddRange<T>(IEnumerable<T> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
            where T : class
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    GetDbSet<T>().AddRange(instances);
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
                RaiseLogEvent(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T).Name}>(instances=[{(instances == null ? "NULL" : instances.Count().ToString())}],saveAfterInsert={saveAfterInsert},waitUntilSaving={waitUntilSaving})"));
                throw;
            }
        }
        
        /// <summary>
        /// Remove item from database
        /// </summary>
        /// <param name="instance">Item instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void Remove<T>(T instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
            where T : class
        {
            RemoveRange(new T[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        
        /// <summary>
        /// Remove items from database
        /// </summary>
        /// <param name="instances">Instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void RemoveRange<T>(IEnumerable<T> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
            where T : class
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    GetDbSet<T>().RemoveRange(instances);
                    if (saveAfterRemove)
                        this.SaveChanges(waitUntilSaving);
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
                RaiseLogEvent(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T).Name}>(instances=[{(instances == null ? "NULL" : instances.Count().ToString())}],saveAfterInsert={saveAfterRemove},waitUntilSaving={waitUntilSaving})"));
                throw;
            }
        }
        
        /// <summary>
        /// New item without any link to database
        /// </summary>
        /// <returns>Item instance</returns>
        public T New<T>(object anonymousFiller = null)
            where T : class
        {
            try
            {
                var defConstructor = typeof(T).GetConstructor(new Type[] { });
                var res = (T)defConstructor.Invoke(null);
                if (anonymousFiller != null)
                    res.CopyObjectFrom(anonymousFiller);
                return res;
            }
            catch(Exception ex)
            {
                RaiseLogEvent(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T).Name}>()"));
                throw;
            }
        }

        /// <summary>
        /// New item without any link to database
        /// </summary>
        /// <returns>Item instance</returns>
        public T New<T>(Action<T> filler)
            where T : class
        {
            try
            {
                var defConstructor = typeof(T).GetConstructor(new Type[] { });
                var res = (T)defConstructor.Invoke(null);
                if (filler != null)
                    filler(res);
                return res;
            }
            catch (Exception ex)
            {
                RaiseLogEvent(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T).Name}>()"));
                throw;
            }
        }

        /// <summary>
        /// Get items
        /// </summary>
        /// <param name="eagerLoad">Eager load some property names</param>
        /// <returns>Item array</returns>
        public IQueryable<T> Get<T>(IEnumerable<string> eagerLoad = null, bool asNoTracking = false)
            where T : class
        {
            IQueryable<T> res = GetDbSet<T>();
            if (eagerLoad != null)
                foreach (var el in eagerLoad)
                    res = res.Include(el);
            if (asNoTracking)
                res = res.AsNoTracking();
            return res;
        }

        /// <summary>
        /// Get items
        /// </summary>
        /// <param name="eagerLoad">Eager load some property names</param>
        /// <param name="eagerLoadFuncs">Eager load some properties</param>
        /// <param name="whereClause">Where clause</param>
        /// <returns>Item array</returns>
        public IQueryable<T> Get<T>(
            System.Linq.Expressions.Expression<Func<T, bool>> whereClause,
            IEnumerable<string> eagerLoad = null,
            bool asNoTracking = false)
            where T : class
        {
            return Get<T>(eagerLoad, asNoTracking).Where(whereClause);
        }
    }
}
