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
    public partial class AccountService : Base.BaseService, IAccountService
    {
        public ImportQueueRecordExecutionResults GetImportQueueRecords(Guid accountId, DateTime? from, DateTime? to, uint pageIndex, uint itemsPerPage)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    if (itemsPerPage == 0)
                        throw new ArgumentException("itemsPerPage");

                    using (var rep = GetNewRepository(logSession))
                    {
                        var totalItemsWithFilter = 
                                rep.Get<RoyaltyRepository.Models.ImportQueueRecord>(a => a.AccountUID == accountId, 
                                    new[] {
                                        nameof(RoyaltyRepository.Models.ImportQueueRecord.FileInfoes),
                                        nameof(RoyaltyRepository.Models.ImportQueueRecord.FileInfoes) + "." + nameof(RoyaltyRepository.Models.ImportQueueRecordFileInfo.LoadedRecords),
                                        nameof(RoyaltyRepository.Models.ImportQueueRecord.FileInfoes) + "." + nameof(RoyaltyRepository.Models.ImportQueueRecordFileInfo.Files),
                                    }, asNoTracking: true)
                                .OrderByDescending(i => i.CreatedDate)
                                .AsQueryable();

                        if (from.HasValue)
                            totalItemsWithFilter = totalItemsWithFilter
                                .Where(pm => pm.CreatedDate >= from.Value);

                        if (to.HasValue)
                            totalItemsWithFilter = totalItemsWithFilter
                                .Where(pm => pm.CreatedDate <= to.Value);

                        var itemsCount = totalItemsWithFilter.Count();
                        var pageCount = (itemsCount / itemsPerPage) * itemsPerPage < itemsCount
                            ? (itemsCount / itemsPerPage) + 1
                            : (itemsCount / itemsPerPage);
                        pageIndex = Math.Min(pageIndex, (uint)pageCount - 1);
                        if (itemsCount == 0) pageCount = 0;
                        var items = totalItemsWithFilter
                            .Skip((int)pageIndex * (int)itemsPerPage)
                            .Take((int)itemsPerPage)
                            .ToArray();
                        var res = items.Select(i => AutoMapper.Mapper.Map<Model.ImportQueueRecord>(i)).ToArray();
                        return new ImportQueueRecordExecutionResults(res, pageIndex, (uint)pageCount);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(accountId), accountId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new ImportQueueRecordExecutionResults(ex);
                }
        }

        public ImportQueueRecordExecutionResult PutImportQueueRecord(Model.ImportQueueRecord item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var account = rep.Get<RoyaltyRepository.Models.Account>(p => p.AccountUID == item.AccountUID).SingleOrDefault();
                        if (account == null)
                            throw new Exception(Properties.Resources.SERVICES_ACCOUNT_AccountNotFound);

                        var dbItem = rep.New<RoyaltyRepository.Models.ImportQueueRecord>(i => {
                            i.AccountUID = item.AccountUID;
                            i.ImportQueueRecordUID = item.ImportQueueRecordUID == Guid.Empty ? Guid.NewGuid() : item.ImportQueueRecordUID;
                            i.CreatedDate = DateTime.UtcNow;
                        });
                        rep.Add(dbItem, saveAfterInsert: false);

                        LoadFileInfoes(rep, dbItem, item.FileInfoes);

                        rep.SaveChanges();
                        var res = AutoMapper.Mapper.Map<Model.ImportQueueRecord>(dbItem);
                        return new ImportQueueRecordExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new ImportQueueRecordExecutionResult(ex);
                }
        }

        public ImportQueueRecordExecutionResult UpdateImportQueueRecord(Model.ImportQueueRecord item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var account = rep.Get<RoyaltyRepository.Models.Account>(p => p.AccountUID == item.AccountUID).SingleOrDefault();
                        if (account == null)
                            throw new Exception(Properties.Resources.SERVICES_ACCOUNT_AccountNotFound);

                        var dbItem = rep.Get<RoyaltyRepository.Models.ImportQueueRecord>((i) => i.ImportQueueRecordUID == item.ImportQueueRecordUID).FirstOrDefault();
                        if (dbItem == null)
                            throw new Exception(Properties.Resources.SERVICES_ACCOUNT_ImportQueueRecord_NotFound);

                        dbItem.CopyObjectFrom(item, new[] { nameof(dbItem.FileInfoes) });

                        UpdateFileInfoes(rep, dbItem, item.FileInfoes);

                        rep.SaveChanges();
                        var res = AutoMapper.Mapper.Map<Model.ImportQueueRecord>(dbItem);
                        return new ImportQueueRecordExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new ImportQueueRecordExecutionResult(ex);
                }
        }

        public GuidExecutionResult RemovemportQueueRecord(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = RemoveImportQueueRecordRange(new [] { identifier });
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

        public GuidExecutionResults RemoveImportQueueRecordRange(Guid[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account import queue records with ids = '{identifiers.Concat(i => i.ToString(), ",")}' from database...");
                        var itemsToRemove = rep.Get<RoyaltyRepository.Models.ImportQueueRecord>(a => identifiers.Contains(a.ImportQueueRecordUID)).ToArray();
                        logSession.Add($"Accounts import queue records found for remove: {itemsToRemove.Length}");
                        rep.RemoveRange(itemsToRemove);
                        rep.SaveChanges();
                        return new GuidExecutionResults(itemsToRemove.Select(i => i.ImportQueueRecordUID).ToArray());
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

        private void LoadFileInfoes(RoyaltyRepository.Repository rep, RoyaltyRepository.Models.ImportQueueRecord destination, IEnumerable<ImportQueueRecordFileInfo> fileInfoes)
        {
            var stateDafault = rep.ImportQueueRecordStateGetDefault();
            foreach(var item in fileInfoes)
            {
                var dbItem = rep.New<RoyaltyRepository.Models.ImportQueueRecordFileInfo>((i) => {
                    i.ImportQueueRecordFileInfoUID = Guid.NewGuid();
                    i.ForAnalize = item.ForAnalize;
                    i.ImportQueueRecord = destination;
                    i.SourceFilePath = item.SourceFilePath;
                    i.ImportQueueRecordState = stateDafault;
                });
                rep.Add(dbItem, saveAfterInsert: false);
                LoadFiles(rep, dbItem, item.Files);
            }
        }

        private void LoadFiles(RoyaltyRepository.Repository rep, RoyaltyRepository.Models.ImportQueueRecordFileInfo destination, IEnumerable<ImportQueueRecordFileInfoFile> files)
        {
            foreach (var item in files)
            {
                var dbItem = rep.New<RoyaltyRepository.Models.ImportQueueRecordFileInfoFile>((i) => {
                    i.FileUID = item.File != null ? item.File.FileUID : item.FileUID;
                    i.File = rep.Get<RoyaltyRepository.Models.File>(f => f.FileUID == i.FileUID).FirstOrDefault();
                    i.ImportQueueRecordFileInfo = destination;
                    i.Type = RoyaltyRepository.Models.ImportQueueRecordFileInfoFileType.Import;
                });
                rep.Add(dbItem, saveAfterInsert: false);
            }
        }

        private void UpdateFileInfoes(RoyaltyRepository.Repository rep, RoyaltyRepository.Models.ImportQueueRecord destination, IEnumerable<ImportQueueRecordFileInfo> fileInfoes)
        {
            throw new NotImplementedException();
        }
    }
}
