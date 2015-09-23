﻿using System;
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
        /// Add AccountDataRecordAdditionalColumn to database
        /// </summary>
        /// <param name="instance">AccountDataRecordAdditionalColumn instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalColumnAdd(AccountDataRecordAdditionalColumn instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDataRecordAdditionalColumnAdd(new AccountDataRecordAdditionalColumn[] { instance }, instance.Account, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDataRecordAdditionalColumn to database
        /// </summary>
        /// <param name="instance">AccountDataRecordAdditionalColumn instance</param>
        /// <param name="account">Account instance for instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalColumnAdd(AccountDataRecordAdditionalColumn instance, Account account, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountDataRecordAdditionalColumnAdd(new AccountDataRecordAdditionalColumn[] { instance }, account, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add AccountDataRecordAdditionalColumns to database
        /// </summary>
        /// <param name="instances">AccountDataRecordAdditionalColumn instance array</param>
        /// <param name="account">Account instance for instances</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalColumnAdd(IEnumerable<AccountDataRecordAdditionalColumn> instances, Account account, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                if (account == null)
                    throw new ArgumentNullException("account");
                instances = instances.Where(i => i != null).ToArray();
                try
                {
                    foreach (var i in instances)
                        if (i.Account != account)
                            i.Account = account;

                    this.Context.AccountDataRecordAdditionalColumns.AddRange(instances);
                    if (saveAfterInsert)
                        this.SaveChanges(waitUntilSaving);
                }
                catch (Exception ex)
                {
                    var e = new Exception(ex.Message, ex);
                    for (int i = 0; i < instances.Count();i++)
                        e.Data.Add(string.Format("instance_{0}", i), instances.ElementAt(i).ToString());
                    throw e;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordAdditionalColumnAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove AccountDataRecordAdditionalColumn from database
        /// </summary>
        /// <param name="instance">AccountDataRecordAdditionalColumn instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalColumnRemove(AccountDataRecordAdditionalColumn instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountDataRecordAdditionalColumnRemove(new AccountDataRecordAdditionalColumn[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove AccountDataRecordAdditionalColumns from database
        /// </summary>
        /// <param name="instances">AccountDataRecordAdditionalColumn instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountDataRecordAdditionalColumnRemove(IEnumerable<AccountDataRecordAdditionalColumn> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.AccountDataRecordAdditionalColumns.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordAdditionalColumnRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new AccountDataRecordAdditionalColumn instance without any link to database
        /// </summary>
        /// <returns>AccountDataRecordAdditionalColumn instance</returns>
        public AccountDataRecordAdditionalColumn AccountDataRecordAdditionalColumnNew(Account account = null, string columnName = null, string columnSystemName = null)
        {
            try
            {
                var res = new AccountDataRecordAdditionalColumn()
                {
                    ColumnName = columnName,
                    ColumnSystemName = columnSystemName,
                    Account = account,
                };
                if (account != null)
                    account.AdditionalColumns.Add(res);
                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountDataRecordAdditionalColumnNew()"));
                throw;
            }
        }
    }
}
