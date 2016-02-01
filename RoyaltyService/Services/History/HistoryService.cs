using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoyaltyService.Services.History
{
    public partial class HistoryService : Base.BaseService, IHistoryService
    {
        public long MaxHistoryCount { get; set; } = Config.Config.ServicesConfig.MaxHistoryCount;

        private Array GetModelItemsFromRepository(RoyaltyRepository.Repository rep, Type entitySourceType, Type entitySourceTypeResult, Type propertyType, string[] ids, bool idsOnly)
        {
            var items = rep.GetHistoryElements(entitySourceType, entitySourceTypeResult, ids);
            if (items != null)
            {
                if (idsOnly)
                    return items;

                var propertyElementType = propertyType.IsArray
                    ? propertyType.GetElementType()
                    : propertyType;

                var typeMap = AutoMapper.Mapper.GetAllTypeMaps().FirstOrDefault(t => t.SourceType == entitySourceType && t.DestinationType == propertyElementType);
                if (typeMap != null)
                {
                    var resArray = Array.CreateInstance(propertyElementType, items.Length);
                    var mappedArray = items
                        .Cast<object>()
                        .Select(i => AutoMapper.Mapper.Map(i, entitySourceType, propertyElementType))
                        .ToArray();
                    Array.Copy(mappedArray, resArray, items.Length);
                    return resArray;
                }
                else
                    return items;
            }
            return null;
        }

        private object GetData(RoyaltyRepository.Models.HistoryActionType type, RoyaltyRepository.Models.History[] historyData, RoyaltyRepository.Repository rep)
        {
            var idOnly = type == RoyaltyRepository.Models.HistoryActionType.Remove;

            var wasChanged = false;
            var resD = idOnly
                ? new Model.HistoryRemovePart() as object
                : new Model.HistoryUpdatePart() as object;

#pragma warning disable 618
            var groupedItems = historyData.Distinct().ToArray()
                .GroupBy(h => new { h.SourceID, h.SourceName, h.ActionType })
                .Select(h => h.Key)
                .ToArray();
#pragma warning restore 618

            resD.GetType()
                .GetProperties()
                .Where(p => p.CanWrite)
                .Select(p => new { Property = p, Attr = p.GetCustomAttributes(typeof(Model.RepositoryHistoryLinkAttribute), true) })
                .Select(p => new { p.Property, Attr = p.Attr != null && p.Attr.Length == 1 ? p.Attr[0] as Model.RepositoryHistoryLinkAttribute : null })
                .Where(p => p.Attr != null)
                .ToList()
                .ForEach(p =>
                {
                    var ids = groupedItems
                        .Where(i => i.ActionType == type && string.Compare(i.SourceName, p.Attr.RepositoryEntityType.Name, true) == 0)
                        .Select(i => i.SourceID)
                        .ToArray();

                    if (ids.Length > 0)
                    {
                        var items = GetModelItemsFromRepository(rep, p.Attr.RepositoryEntityType, p.Attr.RepositoryArrayElementEntitySourceType, p.Property.PropertyType, ids, idOnly);
                        if (items != null)
                        {
                            p.Property.SetValue(resD, items);
                            wasChanged = true;
                        }
                    }
                });
            return (wasChanged) ? resD : null;
        }

        #region IHistoryService

        public HistoryExecutionResult Get()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var historyData = rep.Get<RoyaltyRepository.Models.History>()
                            .OrderByDescending(i => i.HistoryID)
                            .Take(1)
                            .FirstOrDefault();
                        Model.History res = new Model.History() { EventId = historyData?.HistoryID ?? 0 };
                        return new HistoryExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new HistoryExecutionResult(ex);
                }
        }

        public HistoryExecutionResult GetFrom(long identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    while (true)
                    { 
                        using (var rep = GetNewRepository(logSession))
                        {
                            var historyData = rep.Get<RoyaltyRepository.Models.History>(h => h.HistoryID > identifier)
                                .Take((int)MaxHistoryCount)
                                .OrderBy(h => h.HistoryID)
                                .ToArray();

                            var addItems = GetData(RoyaltyRepository.Models.HistoryActionType.Add, historyData, rep) as Model.HistoryUpdatePart;
                            var chgItems = GetData(RoyaltyRepository.Models.HistoryActionType.Change, historyData, rep) as Model.HistoryUpdatePart;
                            var delItems = GetData(RoyaltyRepository.Models.HistoryActionType.Remove, historyData, rep) as Model.HistoryRemovePart;

                            if (addItems != null || chgItems != null || delItems != null)
                            { 
                                var res = new Model.History()
                                {
                                    EventId = historyData.Last().HistoryID,
                                    Add = addItems,
                                    Change = chgItems,
                                    Remove = delItems
                                };
                                return new HistoryExecutionResult(res);
                            }
                        }
                        Thread.Sleep(500);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new HistoryExecutionResult(ex);
                }
        }

        #endregion
    }
}
