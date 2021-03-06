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
using System.Transactions;

namespace RoyaltyRepository
{
    public partial class Repository
    {
        /// <summary>
        /// Create/Get new account without any link to database
        /// </summary>
        /// <param name="byDefault">Copy all settings and other from defaults</param>
        /// <returns>Account instance</returns>
        public Account NewAccount(bool byDefault = false, string accountName = null)
        {
            try
            { 
                var res = new Account()
                { 
                    AccountUID = Guid.NewGuid(),
                    Dictionary = new AccountDictionary(),
                    Settings = new AccountSettings(),
                };

                if (byDefault)
                {
                    var defaultAccount = GetAccount(Account.defaultAccountName, true, new string[]
                    {
                        "AdditionalColumns",
                        "Data",
                        "Dictionary",
                        "Dictionary.Excludes",
                        "Dictionary.Records",
                        "Dictionary.Records.Conditions",
                        "Dictionary.Records.Street",
                        "Dictionary.Records.ChangeStreetTo",
                        "SeriesOfNumbers",
                        "State",
                        "Settings",
                        "Settings.SheduleTimes",
                        "Settings.Columns",
                        "Settings.ImportDirectories",
                        "Settings.ExportDirectories",
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
            bool copyData = true,
            bool copyDataAdditionalColumns = true,
            bool copyDictionary = true, 
            bool copyDictionaryData = true,
            bool copyDictionaryExclude = true,
            bool copySettings = true,
            bool copySettingsShedule = true,
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
            #region Settings
            if (copySettings)
            {
                to.Settings.IgnoreExportTime = from.Settings.IgnoreExportTime;
                to.Settings.TimeForTrust = from.Settings.TimeForTrust;

                foreach (var i in from.Settings.ImportDirectories)
                    to.Settings.ImportDirectories.Add(new AccountSettingsImportDirectory()
                    {
                        DeleteFileAfterImport = i.DeleteFileAfterImport,
                        Encoding = i.Encoding,
                        Filter = i.Filter,
                        ForAnalize = i.ForAnalize,
                        Path = i.Path,
                        RecursiveFolderSearch = i.RecursiveFolderSearch,
                    });

                foreach (var i in from.Settings.ExportDirectories)
                    to.Settings.ExportDirectories.Add(new AccountSettingsExportDirectory()
                    {
                        Encoding = i.Encoding,
                        DirectoryPath = i.DirectoryPath,
                        ExecuteAfterAnalizeCommand = i.ExecuteAfterAnalizeCommand,
                        TimeoutForExecute = i.TimeoutForExecute,
                        FileName = i.FileName,
                        Mark = i.Mark,
                    });

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
        public IQueryable<Account> GetAccount(bool showHidden = false, IEnumerable<string> eagerLoad = null)
        {
            return Get<Account>(a => showHidden || !a.IsHidden, eagerLoad);
        }
        
        /// <summary>
        /// Get one account by name
        /// </summary>
        /// <param name="accountName">Account name</param>
        /// <param name="showHidden">Show hidden accounts</param>
        /// <returns>Account with identifier</returns>
        public Account GetAccount(string accountName, bool showHidden = false, IEnumerable<string> eagerLoad = null)
        {
            return GetAccount(new string[] { accountName }, showHidden, eagerLoad)
                .SingleOrDefault();
        }
        
        /// <summary>
        /// Get accounts by names
        /// </summary>
        /// <param name="accountNames">Account name array</param>
        /// <param name="showHidden">Show hidden accounts</param>
        /// <returns>Account with identifier</returns>
        public IQueryable<Account> GetAccount(IEnumerable<string> accountNames, bool showHidden = false, IEnumerable<string> eagerLoad = null)
        {
            var names = accountNames.Select(n => n.ToUpper());
            return GetAccount(showHidden, eagerLoad).Where(a => names.Contains(a.Name.ToUpper()));
        }
        
        /// <summary>
        /// Get accounts by UIDs
        /// </summary>
        /// <param name="instanceIds">Account identifier array</param>
        /// <param name="showHidden">Show hidden accounts</param>
        /// <returns>Account with identifier</returns>
        public IQueryable<Account> AccountGet(IEnumerable<Guid> instanceIds, bool showHidden = false, IEnumerable<string> eagerLoad = null)
        {
            return GetAccount(showHidden, eagerLoad).Where(a => instanceIds.Contains(a.AccountUID));
        }
    }
}
