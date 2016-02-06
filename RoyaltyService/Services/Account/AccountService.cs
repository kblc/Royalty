using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyService.Model;
using System.ServiceModel;
using Helpers.Linq;
using Helpers;
using RoyaltyService.Services.Account.Result;

namespace RoyaltyService.Services.Account
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public partial class AccountService : Base.BaseService, IAccountService
    {
        #region Static initializer

        static AccountService()
        {
            Model.ColumnType.InitializeMap();
            Model.Mark.InitializeMap();

            Model.Account.InitializeMap();
            Model.AccountSettingsColumn.InitializeMap();
            Model.AccountSettingsImportDirectory.InitializeMap();
            Model.AccountSettingsExportDirectory.InitializeMap();
            Model.AccountSettingsSheduleTime.InitializeMap();

            Model.AccountSeriesOfNumbersRecord.InitializeMap();
        }

        #endregion

        public AccountExecutionResults GetAll()
        {
            return GetRange(new Guid[] { });
        }

        public AccountExecutionResult Get(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = GetRange(new Guid[] { identifier });
                    if (res.Exception != null)
                        throw res.Exception;

                    var resValue = res.Values.SingleOrDefault();
                    if (resValue == null)
                        throw new Exception(Properties.Resources.SERVICES_FILE_AccountNotFound);

                    return new AccountExecutionResult(resValue);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResult(ex);
                }
        }

        public AccountExecutionResults GetRange(Guid[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account with ids = '{identifiers.Concat(i => i.ToString(), ",")}' from database...");
                        var items = identifiers.Length == 0
                            ? rep.Get<RoyaltyRepository.Models.Account>(asNoTracking: true).ToArray()
                            : rep.Get<RoyaltyRepository.Models.Account>(a => identifiers.Contains(a.AccountUID), asNoTracking: true).ToArray();
                        logSession.Add($"Accounts found: {items.Length}");

                        var res = items.Select(i => AutoMapper.Mapper.Map<Model.Account>(i)).ToArray();
                        return new AccountExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResults(ex);
                }
        }

        public AccountExecutionResult Put(Model.Account item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var mappedDbItem = AutoMapper.Mapper.Map<RoyaltyRepository.Models.Account>(item);
                        var dbItem = rep.New<RoyaltyRepository.Models.Account>((a) => 
                        {
                            a.CopyObjectFrom(mappedDbItem, new string[] { nameof(a.AccountUID) });
                        });
                        rep.Add(dbItem);
                        var res = AutoMapper.Mapper.Map<Model.Account>(dbItem);
                        return new AccountExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResult(ex);
                }
        }

        public GuidExecutionResult Remove(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = RemoveRange(new Guid[] { identifier });
                    if (res.Exception != null)
                        throw res.Exception;

                    return new GuidExecutionResult(res.Values.FirstOrDefault());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new GuidExecutionResult(ex);
                }
        }

        public GuidExecutionResults RemoveRange(Guid[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account with id = '{identifiers}' from database...");
                        var itemsToRemove = rep.Get<RoyaltyRepository.Models.Account>(a => identifiers.Contains(a.AccountUID)).ToArray();
                        logSession.Add($"Accounts found for remove: {itemsToRemove.Length}");
                        rep.RemoveRange(itemsToRemove);
                        rep.SaveChanges();
                        return new GuidExecutionResults(itemsToRemove.Select(i => i.AccountUID).ToArray());
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(),","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new GuidExecutionResults(ex);
                }
        }

        public AccountExecutionResult Update(Model.Account item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var settName = nameof(RoyaltyRepository.Models.Account.Settings);
                        var dbItem = rep.Get<RoyaltyRepository.Models.Account>(
                            a => a.AccountUID == item.AccountUID,
                            new[] {
                                settName,
                                $"{settName}.{nameof(RoyaltyRepository.Models.AccountSettings.Columns)}",
                                $"{settName}.{nameof(RoyaltyRepository.Models.AccountSettings.ImportDirectories)}",
                                $"{settName}.{nameof(RoyaltyRepository.Models.AccountSettings.ExportDirectories)}",
                                $"{settName}.{nameof(RoyaltyRepository.Models.AccountSettings.SheduleTimes)}",
                            }
                            ).SingleOrDefault();
                        if (dbItem == null)
                            throw new Exception(Properties.Resources.SERVICES_FILE_AccountNotFound);

                        UpdateAccountDbItemFromModelItem(dbItem, item, rep);
                        rep.SaveChanges();

                        var res = AutoMapper.Mapper.Map<Model.Account>(dbItem);
                        return new AccountExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResult(ex);
                }
        }

        private TModel Put<TModel,TRepository>(TModel item, Action<TRepository, TRepository, RoyaltyRepository.Repository> initAction, Helpers.Log.SessionInfo logSession)
            where TRepository: class
        {
            using (var rep = GetNewRepository(logSession))
            {
                var mappedDbItem = AutoMapper.Mapper.Map<TRepository>(item);
                var dbItem = rep.New<TRepository>((a) => initAction?.Invoke(a, mappedDbItem, rep));
                rep.Add(dbItem);
                var res = AutoMapper.Mapper.Map<TModel>(dbItem);
                return res;
            }
        }

        private static void UpdateAccountDbItemFromModelItem(RoyaltyRepository.Models.Account dbItem, Model.Account mappedDbItem, RoyaltyRepository.Repository rep)
        {
            if (mappedDbItem == null)
                return;

            dbItem.CopyObjectFrom(mappedDbItem, new string[] {
                nameof(mappedDbItem.Settings),
            });

            UpdateAccountSettingsDbItemFromModelItem(dbItem.Settings, mappedDbItem.Settings, rep);
        }

        private static void UpdateAccountSettingsDbItemFromModelItem(RoyaltyRepository.Models.AccountSettings dbItem, Model.AccountSettings mappedDbItem, RoyaltyRepository.Repository rep)
        {
            if (mappedDbItem == null)
                return;

            dbItem.CopyObjectFrom(mappedDbItem, new string[] {
                nameof(mappedDbItem.Columns),
                nameof(mappedDbItem.ExportDirectories),
                nameof(mappedDbItem.ImportDirectories),
                nameof(mappedDbItem.SheduleTimes),
            });

            UpdateAccountSettingsColumnsDbItemFromModelItem(dbItem, mappedDbItem.Columns, rep);
            UpdateAccountSettingsExportDirectoryDbItemFromModelItem(dbItem, mappedDbItem.ExportDirectories, rep);
            UpdateAccountSettingsImportDirectoryDbItemFromModelItem(dbItem, mappedDbItem.ImportDirectories, rep);
            UpdateAccountSettingsSheduleTimeDbItemFromModelItem(dbItem, mappedDbItem.SheduleTimes, rep);
        }

        private static void UpdateAccountSettingsColumnsDbItemFromModelItem(RoyaltyRepository.Models.AccountSettings dbItem, IList<Model.AccountSettingsColumn> mappedDbItems, RoyaltyRepository.Repository rep)
        {
            if (mappedDbItems == null)
                return;

            var upd = dbItem.Columns.FullOuterJoin(mappedDbItems, 
                i => i.AccountSettingsColumnID, 
                i => i.AccountSettingsColumnID, 
                (Existed, Update) => new { Existed, Update });

            foreach(var item in upd)
            {
                if (item.Existed != null && item.Update != null)
                {
                    item.Existed.CopyObjectFrom(item.Update);
                } else
                {
                    if (item.Existed == null)
                    {
                        var newItem = rep.New<RoyaltyRepository.Models.AccountSettingsColumn>(item.Update);
                        dbItem.Columns.Add(newItem);
                        rep.Add(newItem, saveAfterInsert: false);
                    } else
                    {
                        var delItem = item.Existed;
                        dbItem.Columns.Remove(delItem);
                        rep.Remove(delItem, saveAfterRemove: false);
                    }
                }
            }
        }

        private static void UpdateAccountSettingsExportDirectoryDbItemFromModelItem(RoyaltyRepository.Models.AccountSettings dbItem, IList<Model.AccountSettingsExportDirectory> mappedDbItems, RoyaltyRepository.Repository rep)
        {
            if (mappedDbItems == null)
                return;

            var upd = dbItem.ExportDirectories.FullOuterJoin(mappedDbItems,
                i => i.AccountSettingsExportDirectoryID,
                i => i.AccountSettingsExportDirectoryID,
                (Existed, Update) => new { Existed, Update });

            foreach (var item in upd)
            {
                if (item.Existed != null && item.Update != null)
                {
                    item.Existed.CopyObjectFrom(item.Update);
                }
                else
                {
                    if (item.Existed == null)
                    {
                        var newItem = rep.New<RoyaltyRepository.Models.AccountSettingsExportDirectory>(item.Update);
                        dbItem.ExportDirectories.Add(newItem);
                        rep.Add(newItem, saveAfterInsert: false);
                    }
                    else
                    {
                        var delItem = item.Existed;
                        dbItem.ExportDirectories.Remove(delItem);
                        rep.Remove(delItem, saveAfterRemove: false);
                    }
                }
            }
        }

        private static void UpdateAccountSettingsImportDirectoryDbItemFromModelItem(RoyaltyRepository.Models.AccountSettings dbItem, IList<Model.AccountSettingsImportDirectory> mappedDbItems, RoyaltyRepository.Repository rep)
        {
            if (mappedDbItems == null)
                return;

            var upd = dbItem.ImportDirectories.FullOuterJoin(mappedDbItems,
                i => i.AccountSettingsImportDirectoryID,
                i => i.AccountSettingsImportDirectoryID,
                (Existed, Update) => new { Existed, Update });

            foreach (var item in upd)
            {
                if (item.Existed != null && item.Update != null)
                {
                    item.Existed.CopyObjectFrom(item.Update);
                }
                else
                {
                    if (item.Existed == null)
                    {
                        var newItem = rep.New<RoyaltyRepository.Models.AccountSettingsImportDirectory>(item.Update);
                        dbItem.ImportDirectories.Add(newItem);
                        rep.Add(newItem, saveAfterInsert: false);
                    }
                    else
                    {
                        var delItem = item.Existed;
                        dbItem.ImportDirectories.Remove(delItem);
                        rep.Remove(delItem, saveAfterRemove: false);
                    }
                }
            }
        }

        private static void UpdateAccountSettingsSheduleTimeDbItemFromModelItem(RoyaltyRepository.Models.AccountSettings dbItem, IList<Model.AccountSettingsSheduleTime> mappedDbItems, RoyaltyRepository.Repository rep)
        {
            if (mappedDbItems == null)
                return;

            var upd = dbItem.SheduleTimes.FullOuterJoin(mappedDbItems,
                i => i.AccountSettingsSheduleTimeID,
                i => i.AccountSettingsSheduleTimeID,
                (Existed, Update) => new { Existed, Update });

            foreach (var item in upd)
            {
                if (item.Existed != null && item.Update != null)
                {
                    item.Existed.CopyObjectFrom(item.Update);
                }
                else
                {
                    if (item.Existed == null)
                    {
                        var newItem = rep.New<RoyaltyRepository.Models.AccountSettingsSheduleTime>(item.Update);
                        dbItem.SheduleTimes.Add(newItem);
                        rep.Add(newItem, saveAfterInsert: false);
                    }
                    else
                    {
                        var delItem = item.Existed;
                        dbItem.SheduleTimes.Remove(delItem);
                        rep.Remove(delItem, saveAfterRemove: false);
                    }
                }
            }
        }
    }
}
