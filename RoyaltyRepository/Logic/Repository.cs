﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository.Models;
using EntityFramework.Utilities;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using Helpers;
using Helpers.Linq;

namespace RoyaltyRepository
{
    public partial class Repository : IDisposable
    {
        public RepositoryContext Context { get; private set; }

        public Repository()
        {
            Context = new RepositoryContext();
            InitLog();
        }
        public Repository(string connectionStringName)
        {
            Context = new RepositoryContext(connectionStringName);
            InitLog();
        }
        public Repository(string connectionString, string connectionProviderName)
        {
            Context = new RepositoryContext(connectionString, connectionProviderName);
            InitLog();
        }

        public IDisposable BeginTransaction(bool commitOnDispose = false)
        {
            return Context.BeginTransaction(commitOnDispose);
        }
        public void CommitTransaction()
        {
            Context.CommitTransaction();
        }
        public void RollbackTransaction()
        {
            Context.RollbackTransaction();
        }

        public void SaveChanges(bool waitUntilSaving = true)
        {
            if (Context == null)
                throw new ArgumentNullException("Context", "Initialize Context first for Repository");
            if (waitUntilSaving)
                Context.SaveChanges();
            else
                Context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Context != null)
            {
                Context.Dispose();
                Context = null;
            }
        }

        public void Dispose()
        {
            Dispose(false);
        }

        private void InitLog()
        {
            Context.Log = (s) => SqlLog?.Invoke(this, s);
        }

        protected void RaiseLogEvent(string logMessage)
        {
            Log?.Invoke(this, logMessage);
        }

        public event EventHandler<string> SqlLog;
        public event EventHandler<string> Log;

        public Array GetHistoryElements(Type sourceType, Type returnArrayElementType, string[] ids)
        {
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", false, str => str.ToList().ForEach(s => RaiseLogEvent(s))))
                try
                {
                    var returnArrayType = returnArrayElementType.MakeArrayType();

                    var methods = sourceType
                        .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        .Where(mi => mi.ReturnType == returnArrayType && mi.GetCustomAttribute(typeof(HistoryResolverAttribute)) != null) //validTypes.Contains(mi.ReturnType)
                        .ToArray();

                    foreach (var method in methods)
                        try
                        {
                            var invokeArray = (Array)method.Invoke(null, new object[] { this, ids });
                            var resArray = Array.CreateInstance(returnArrayElementType, invokeArray.Length);
                            Array.Copy(invokeArray, resArray, invokeArray.Length);
                            return resArray;
                        }
                        catch (Exception ex)
                        {
                            ex.Data.Add(nameof(sourceType), sourceType);
                            ex.Data.Add(nameof(returnArrayElementType), returnArrayElementType);
                            ex.Data.Add(nameof(ids), ids.Concat(i => i, ","));
                            logSession.Enabled = true;
                            logSession.Add(ex);
                        }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(sourceType), sourceType);
                    ex.Data.Add(nameof(returnArrayElementType), returnArrayElementType);
                    ex.Data.Add(nameof(ids), ids.Concat(i => i, ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                }
            return null;
        }

        protected void BulkInsert<T>(IEnumerable<T> instances)
            where T: class
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    var set = Context.GetType().GetProperties()
                        .Select(pi => pi.GetValue(Context) as IDbSet<T>)
                        .Where(i => i != null)
                        .Single();

                    EFBatchOperation.For(Context, set).InsertAll(instances);

                    foreach (var i in instances)
                        Context.Entry<T>(i).State = EntityState.Detached;
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
                Helpers.Log.Add(ex, string.Format("Repository.BulkInsert(instances=[{0}])", instances == null ? "NULL" : instances.Count().ToString()));
                throw;
            }
        }

        protected void BulkDelete<T>(Expression<Func<T, bool>> selector)
            where T : class
        {
            try
            {
                if (selector == null)
                    throw new ArgumentNullException("selector");

                var set = Context.GetType().GetProperties()
                    .Select(pi => pi.GetValue(Context) as IDbSet<T>)
                    .Where(i => i != null)
                    .Single();

                EFBatchOperation.For(this.Context, set).Where(selector).Delete();
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, $"Repository.BulkDelete(selector='{selector}')");
                throw;
            }
        }
    }
}
