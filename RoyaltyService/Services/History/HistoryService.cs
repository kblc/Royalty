using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoyaltyService.Services.History
{
    public class HistoryService : Base.BaseService, IHistoryService
    {
        public long MaxHistoryCount { get; set; } = Config.Config.ServicesConfig.MaxHistoryCount;

        private object GetModelItemsFromRepository(RoyaltyRepository.Repository rep, Type entityType, Type returnType, RoyaltyRepository.Models.IHistoryRecordSourceIdentifier[][] ids, bool idsOnly)
        {
            var items = rep.GetHistoryElements(entityType, returnType, ids);
            if (items != null)
            {
                if (idsOnly)
                    return items;

                var typeMap = AutoMapper.Mapper.GetAllTypeMaps().FirstOrDefault(t => t.SourceType == entityType);
                if (typeMap != null)
                {
                    return ((Array)items).Cast<object>().Select(i => AutoMapper.Mapper.Map(i, entityType, typeMap.DestinationType)).ToArray();
                }
                else
                    return items;
            }
            return null;
        }

        public HistoryExecutionResult Get()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var historyData = rep.Get<RoyaltyRepository.Models.History>().OrderByDescending(i => i.HistoryID).Take(1).FirstOrDefault();
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
                    Model.History res = null;
                    while (true)
                    { 
                        using (var rep = GetNewRepository(logSession))
                        {
                            var historyData = rep.Get<RoyaltyRepository.Models.History>(h => h.HistoryID > identifier)
                                .Take((int)MaxHistoryCount)
                                .OrderBy(h => h.HistoryID)
                                .ToArray();
                            if (historyData.Length > 0)
                            {
                                var groupedItems = historyData
                                    .GroupBy(h => new { h.SourceID, h.SourceName })
                                    .ToArray()
                                    .Select(g => new { Items = g, Empty = g.Any(n => n.ActionType == RoyaltyRepository.Models.HistoryActionType.Add) && g.Any(n => n.ActionType == RoyaltyRepository.Models.HistoryActionType.Remove) })
                                    .Where(i => !i.Empty)
                                    .SelectMany(i => i.Items)
                                    .GroupBy(h => h.ActionType)
                                    .Select(g => new
                                    {
                                        Action = g.Key,
                                        Items = g.OrderBy(h => h.Date)
                                    })
                                    .Select(i => new
                                    {
                                        i.Action,
                                        Items = i.Items.Select(hi => new
                                        {
                                            hi.SourceName,
                                            Ids = hi.GetIdentifiers()
                                        })
                                    })
                                    .ToArray();

                                var getData = new Func<RoyaltyRepository.Models.HistoryActionType, object>((type) => 
                                {
                                    var idOnly = type == RoyaltyRepository.Models.HistoryActionType.Remove;

                                    var wasChanged = false;
                                    var resD = idOnly
                                        ? new Model.HistoryIdOnlytPart() as object
                                        : new Model.HistoryPart() as object;
                                    resD.GetType()
                                        .GetProperties()
                                        .Where(p => p.CanWrite)
                                        .Select(p => new { Property = p, Attr = p.GetCustomAttributes(typeof(Model.RepositoryHistoryLinkAttribute), true) })
                                        .Select(p => new { p.Property, Attr = p.Attr != null && p.Attr.Length == 1 ? p.Attr[0] as Model.RepositoryHistoryLinkAttribute : null })
                                        .Where(p => p.Attr != null)
                                        .ToList()
                                        .ForEach(p =>
                                        {
                                            var ids = groupedItems.Where(i => i.Action == type)
                                                .SelectMany(i => i.Items)
                                                .Where(i => string.Compare(i.SourceName, p.Attr.RepositoryEntityType.Name, true) == 0)
                                                .Select(i => i.Ids.ToArray())
                                                .ToArray();
                                            if (ids.Length > 0)
                                            { 
                                                var items = GetModelItemsFromRepository(rep, p.Attr.RepositoryEntityType, p.Property.PropertyType, ids, idOnly);
                                                if (items != null)
                                                {
                                                    p.Property.SetValue(resD, items);
                                                    wasChanged = true;
                                                }
                                            }
                                        });
                                    return (wasChanged) ? resD : null;
                                });

                                var addItems = getData(RoyaltyRepository.Models.HistoryActionType.Add) as Model.HistoryPart;
                                var chgItems = getData(RoyaltyRepository.Models.HistoryActionType.Change) as Model.HistoryPart;
                                var delItems = getData(RoyaltyRepository.Models.HistoryActionType.Remove) as Model.HistoryIdOnlytPart;

                                res = new Model.History()
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

        public HistoryExecutionResult RESTGetFrom(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetLongByString(identifier);
                    return GetFrom(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new HistoryExecutionResult(ex);
                }
        }
    }
}
