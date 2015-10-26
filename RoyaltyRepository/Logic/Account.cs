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
        /// <summary>
        /// Add account to database
        /// </summary>
        /// <param name="instance">Account instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountAdd(Account instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            AccountAdd(new Account[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add accounts to database
        /// </summary>
        /// <param name="instances">Account instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountAdd(IEnumerable<Account> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Accounts.AddRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove account from database
        /// </summary>
        /// <param name="instance">Account instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountRemove(Account instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            AccountRemove(new Account[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove accounts from database
        /// </summary>
        /// <param name="instances">Account instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AccountRemove(IEnumerable<Account> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances
                    .Where(i => i != null)
                    //.Where(i => Context.Entry(i).State != EntityState.Deleted)
                    .ToArray();

                try
                {
                    this.Context.Accounts.RemoveRange(instances);
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
                Helpers.Log.Add(ex, string.Format("Repository.AccountRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new account without any link to database
        /// </summary>
        /// <param name="byDefault">Copy all settings and other from defaults</param>
        /// <returns>Account instance</returns>
        public Account AccountNew(bool byDefault = false, string accountName = null)
        {
            try
            { 
                var res = new Account()
                { 
                    AccountUID = Guid.NewGuid(),
                    Dictionary = new AccountDictionary(),
                    Settings = new AccountSettings(),
                    State = new AccountState()
                };

                if (byDefault)
                {
                    var defaultAccount = AccountGet(Account.defaultAccountName, true, new string[]
                    {
                        "AdditionalColumns",
                        "Data",
                        "Dictionary",
                        "Dictionary.Excludes",
                        "Dictionary.Records",
                        "Dictionary.Records.Conditions",
                        "Dictionary.Records.Street",
                        "Dictionary.Records.ChangeStreetTo",
                        "ExportTypes",
                        "ExportTypes.Mark",
                        "SeriesOfNumbers",
                        "State",
                        "Settings",
                        "Settings.SheduleTimes",
                        "Settings.Columns"
                    });
                    if (defaultAccount != null)
                        AccountCopy(defaultAccount, res);
                    else
                        throw new Exception(RoyaltyRepository.Properties.Resources._DEFAULT_ACCOUNT_NOT_FOUND);
                }
                
                if (accountName != null)
                    res.Name = accountName;

                return res;
            }
            catch(Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.AccountNew(byDefault={0})", byDefault));
                throw;
            }
        }
        
        /// <summary>
        /// Copy account information (withou IDs) from one Account to 'to' account
        /// </summary>
        /// <param name="from">Account for copy from</param>
        /// <param name="to">Account for copy to</param>
        public void AccountCopy(Account from, Account to,
            bool copySeriesOfNumbers = true,
            bool copyExportType = true,
            bool copyData = true,
            bool copyDataAdditionalColumns = true,
            bool copyDictionary = true, 
            bool copyDictionaryData = true,
            bool copyDictionaryExclude = true,
            bool copySettings = true,
            bool copySettingsShedule = true,
            bool copyState = true,
            bool copyPhoneMarks = true)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (to == null)
                throw new ArgumentNullException("to");

            #region SeriesOfNumbers
            if (copySeriesOfNumbers)
            {
                to.SeriesOfNumbers.Clear();
                foreach (var son in from.SeriesOfNumbers)
                    to.SeriesOfNumbers.Add(new AccountSeriesOfNumbersRecord() 
                    {
                        Delay = son.Delay,
                        DigitCount = son.DigitCount
                    });
            }
            #endregion
            #region ExportType
            if (copyExportType)
            {
                to.ExportTypes.Clear();
                foreach (var et in from.ExportTypes)
                    to.ExportTypes.Add(new AccountExportType()
                    {
                        FileName = et.FileName,
                        Mark = et.Mark
                    });
            }
            #endregion
            #region Phone marks
            if (copyPhoneMarks)
            { 
                to.PhoneMarks.Clear();
                foreach (var i in to.PhoneMarks)
                    to.PhoneMarks.Add(new AccountPhoneMark()
                    {
                        Account = to,
                        Mark = i.Mark,
                        Phone = i.Phone,
                    }
                );
            }
            #endregion
            #region Data
            if (copyData)
            {
                if (copyDataAdditionalColumns)
                {
                    to.AdditionalColumns.Clear();
                    foreach (var ac in from.AdditionalColumns)
                        to.AdditionalColumns.Add(new AccountDataRecordAdditionalColumn()
                        {
                            ColumnName = ac.ColumnName,
                            ColumnSystemName = ac.ColumnSystemName
                        });
                }

                #region Data

                to.Data.Clear();
                foreach (var d in from.Data)
                    to.Data.Add(new AccountDataRecord()
                    {
                        Account = to,
                        HouseNumber = d.HouseNumber,
                        Street = d.Street,
                        //Changed = d.Changed,
                        //Created = d.Created,
                        Exported = d.Exported,
                        Host = d.Host,
                        Phone = d.Phone,
                        DataAdditional = d.DataAdditional != null && copyDataAdditionalColumns
                        ? new AccountDataRecordAdditional()
                        {
                            Column00 = d.DataAdditional.Column00,
                            Column01 = d.DataAdditional.Column01,
                            Column02 = d.DataAdditional.Column02,
                            Column03 = d.DataAdditional.Column03,
                            Column04 = d.DataAdditional.Column04,
                            Column05 = d.DataAdditional.Column05,
                            Column06 = d.DataAdditional.Column06,
                            Column07 = d.DataAdditional.Column07,
                            Column08 = d.DataAdditional.Column08,
                            Column09 = d.DataAdditional.Column09,
                            Column10 = d.DataAdditional.Column10,
                            Column11 = d.DataAdditional.Column11,
                            Column12 = d.DataAdditional.Column12,
                            Column13 = d.DataAdditional.Column13,
                            Column14 = d.DataAdditional.Column14,
                            Column15 = d.DataAdditional.Column15,
                            Column16 = d.DataAdditional.Column16,
                            Column17 = d.DataAdditional.Column17,
                            Column18 = d.DataAdditional.Column18,
                            Column19 = d.DataAdditional.Column19,
                        }
                        : null,
                    }
                    );
                #endregion
            }
            #endregion
            #region Dictionary
            if (copyDictionary)
            { 
                to.Dictionary.AllowAddToDictionaryAutomatically = from.Dictionary.AllowAddToDictionaryAutomatically;
                to.Dictionary.AllowCalcAreasIfStreetExistsOnly = from.Dictionary.AllowCalcAreasIfStreetExistsOnly;
                to.Dictionary.SimilarityForTrust = from.Dictionary.SimilarityForTrust;
                to.Dictionary.ConditionsScoreForTrust = from.Dictionary.ConditionsScoreForTrust;

                if (copyDictionaryExclude)
                {
                    to.Dictionary.Excludes.Clear();
                    foreach (var e in from.Dictionary.Excludes) 
                        to.Dictionary.Excludes.Add(new AccountDictionaryExclude() { Exclude = e.Exclude });
                }
                if (copyDictionaryData)
                {
                    to.Dictionary.Records.Clear();
                    foreach (var r in from.Dictionary.Records)
                    {
                        var record = new AccountDictionaryRecord()
                        {
                            ChangeStreetTo = r.ChangeStreetTo,
                            Street = r.Street
                        };
                        foreach (var c in r.Conditions)
                            record.Conditions.Add(new AccountDictionaryRecordCondition() { From = c.From, To = c.To });
                        
                        to.Dictionary.Records.Add(record);
                    }
                }
            }
            #endregion
            #region State
            if (copyState)
            { 
                to.State.IsActive = from.State.IsActive;
            }
            #endregion
            #region Settings
            if (copySettings)
            { 
                to.Settings.DeleteFileAfterImport = from.Settings.DeleteFileAfterImport;
                to.Settings.ExecuteAfterAnalizeCommand = from.Settings.ExecuteAfterAnalizeCommand;
                to.Settings.FolderExportAnalize = from.Settings.FolderExportAnalize;
                to.Settings.FolderExportPhones = from.Settings.FolderExportPhones;
                to.Settings.FolderImportAnalize = from.Settings.FolderImportAnalize;
                to.Settings.FolderImportMain = from.Settings.FolderImportMain;
                to.Settings.IgnoreExportTime = from.Settings.IgnoreExportTime;
                to.Settings.RecursiveFolderSearch = from.Settings.RecursiveFolderSearch;
                to.Settings.TimeForTrust = from.Settings.TimeForTrust;
                to.Settings.WaitExecutionAfterAnalize = from.Settings.WaitExecutionAfterAnalize;

                foreach (var c in from.Settings.Columns)
                    to.Settings.Columns.Add( new AccountSettingsColumn() { ColumnName = c.ColumnName, ColumnType = c.ColumnType } );

                if (copySettingsShedule)
                    foreach (var t in from.Settings.SheduleTimes)
                        to.Settings.SheduleTimes.Add(new AccountSettingsSheduleTime() { Time = t.Time });
            }
            #endregion
        }
        /// <summary>
        /// Get accounts
        /// </summary>
        /// <param name="showHidden">Show hidden accounts</param>
        /// <returns>Accounts</returns>
        public IQueryable<Account> AccountGet(bool showHidden = false, IEnumerable<string> eagerLoad = null)
        {
            System.Data.Entity.Infrastructure.DbQuery<Account> res = Context.Accounts;
            if (eagerLoad != null)
            {
                foreach (var el in eagerLoad)
                    res = res.Include(el);
                res.Include((a) => a.Dictionary);
            }
            return res.Where(a => showHidden || !a.IsHidden);
        }
        /// <summary>
        /// Get one account by UID
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <param name="showHidden">Show hidden accounts</param>
        /// <returns>Account with identifier</returns>
        public Account AccountGet(Guid accountId, bool showHidden = false, IEnumerable<string> eagerLoad = null)
        {
            return AccountGet(new Guid[] { accountId }, showHidden, eagerLoad).FirstOrDefault();
        }
        /// <summary>
        /// Get one account by name
        /// </summary>
        /// <param name="accountName">Account name</param>
        /// <param name="showHidden">Show hidden accounts</param>
        /// <returns>Account with identifier</returns>
        public Account AccountGet(string accountName, bool showHidden = false, IEnumerable<string> eagerLoad = null)
        {
            return AccountGet(new string[] { accountName }, showHidden, eagerLoad)
                .SingleOrDefault();
        }
        /// <summary>
        /// Get accounts by names
        /// </summary>
        /// <param name="accountNames">Account name array</param>
        /// <param name="showHidden">Show hidden accounts</param>
        /// <returns>Account with identifier</returns>
        public IQueryable<Account> AccountGet(IEnumerable<string> accountNames, bool showHidden = false, IEnumerable<string> eagerLoad = null)
        {
            return AccountGet(showHidden, eagerLoad)
                .Join(accountNames.Select(n => n.ToUpper()), a => a.Name.ToUpper(), i => i, (a, i) => a);
        }
        /// <summary>
        /// Get accounts by UIDs
        /// </summary>
        /// <param name="instanceIds">Account identifier array</param>
        /// <param name="showHidden">Show hidden accounts</param>
        /// <returns>Account with identifier</returns>
        public IQueryable<Account> AccountGet(IEnumerable<Guid> instanceIds, bool showHidden = false, IEnumerable<string> eagerLoad = null)
        {
            return AccountGet(showHidden, eagerLoad)
                .Join(instanceIds, s => s.AccountUID, i => i, (s, i) => s);
        }
    }
}
