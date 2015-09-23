using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Linq;
using RoyaltyRepository.Models;
using Helpers;

namespace RoyaltyDataCalculator.Parser
{
    /// <summary>
    /// Входящий параметр для парсинга данных
    /// </summary>
    public class AddressParserIncomingParameter
    {
        /// <summary>
        /// Адрес
        /// </summary>
        public Address Address { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public RoyaltyRepository.Models.City City { get; set; }
    }

    /// <summary>
    /// Экземпляр результата парсинга
    /// </summary>
    public class AddressParserResult
    {
        /// <summary>
        /// Адрес
        /// </summary>
        public Address Address { get; set; }
        /// <summary>
        /// Найденная улица
        /// </summary>
        public RoyaltyRepository.Models.Street Street { get; set; }
    }

    /// <summary>
    /// Парсер адресов. Используйте метод Parse()
    /// </summary>
    public class AddressParser
    {
        /// <summary>
        /// Аккаунт, к которому привязан парсер адресов
        /// </summary>
        public RoyaltyRepository.Models.Account Account { get; private set; }
        /// <summary>
        /// Репозиторий
        /// </summary>
        public RoyaltyRepository.Repository Repository { get; private set; }

        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="account">Аккаунт</param>
        /// <param name="repository">Репозиторий</param>
        public AddressParser(RoyaltyRepository.Models.Account account, RoyaltyRepository.Repository repository)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (account == null)
                throw new ArgumentNullException("account");

            Account = account;
            Repository = repository;
        }

        /// <summary>
        /// Поиск в соответствии со входными параметрами (Адрес и город) улиц и прочей информации
        /// </summary>
        /// <param name="incomingData">Входные параметры (Адрес и город)</param>
        /// <returns>Соответствующие выходные параметры (Найденные и созданные улицы и прочее)</returns>
        public IDictionary<AddressParserIncomingParameter, AddressParserResult> Parse(IEnumerable<AddressParserIncomingParameter> incomingData, bool doNotAddAnyDataToDictionary = false, Action<decimal> reportProgress = null, Action<string> verboseLog = null)
        {
            if (incomingData == null)
                throw new ArgumentNullException(nameof(incomingData));

            var log = new Action<string>((str) => { if (verboseLog != null) verboseLog($"{GetType().Name}.{nameof(Parse)}() {str}"); });

            log($"Incoming data length: {incomingData.Count()}");

            var pp = new Helpers.PercentageProgress();
            pp.Change += (s, e) => { if (reportProgress != null) reportProgress(e.Value); };

            var res = incomingData
                .GroupBy(i => i.City)
                .Select(g => new
                {
                    City = g.Key,
                    Items = g.ToArray(),
                    Progress = pp.GetChild()
                })
                .ToArray()
                .AsParallel()
                .Select(g => new
                {
                    g.City,
                    Streets = GetStreets(g.Items.Select(i => i.Address), g.City, doNotAddAnyDataToDictionary, (progress) => g.Progress.Value = progress, str => log(str)),
                    g.Items,
                })
                .SelectMany(g => g.Items.Join(g.Streets, r => r.Address, i => i.Key, (r, i) => new
                {
                    IncomingParameters = r,
                    Data = new
                    {
                        Address = i.Key,
                        Street = i.Value
                    }
                }))
                .ToDictionary(item => item.IncomingParameters, item => new AddressParserResult()
                {
                    Address = item.Data.Address,
                    Street = item.Data.Street
                });

            return res;
        }

        /// <summary>
        /// Находит отношение адреса к условиям из словаря
        /// </summary>
        /// <param name="houseNumber">Номер дома</param>
        /// <param name="conditions">Условия</param>
        /// <param name="doNotAddAnyDataToDictionary">Флаг для отмены добавления данных в словарь</param>
        /// <returns>True если условия соблюдены</returns>
        internal decimal GetConditionsScore(uint? houseNumber, IEnumerable<RoyaltyRepository.Models.AccountDictionaryRecordCondition> conditions, bool doNotAddAnyDataToDictionary)
        {
            if (conditions == null || !conditions.Any() || !houseNumber.HasValue)
                return 1m;
            bool isHouseNumberEven = houseNumber.Value % 2 == 0;

            var resultLength = conditions
                .Where(c => c.Even.HasValue ? (c.Even.Value == isHouseNumberEven) : true)
                .Select(c => new { c.From, c.To, c.Even, IsInside = houseNumber.Value >= c.From && houseNumber <= c.To })
                .AsParallel()
                .Select(c => new
                {
                    LengthBetween = c.IsInside
                        ? 0
                        : new long[] { c.From, c.To }
                            .Distinct()
                            .Select(i => AreaMap.LengthBetween(houseNumber.Value, (uint)i))
                            .Min(),
                    Radius = Math.Abs(c.From - c.To) + 1,
                    Even = c.Even,
                })
                .ToArray();

            var resultWeight = resultLength
                .AsParallel()
                .Select(c => (c.LengthBetween == 0 ? 1m : AreaMap.Weight(c.LengthBetween, c.Radius)) +
                             (c.Even.HasValue ? 0.2m : 0.0m))
                .OrderByDescending(i => i)
                .FirstOrDefault();

            return resultWeight;
        }

        /// <summary>
        /// Получает улицу по адресу в соответствии со словарем
        /// </summary>
        /// <param name="address">Адрес для поиска</param>
        /// <param name="city">Город</param>
        /// <param name="doNotAddAnyDataToDictionary">Флаг для отмены добавления данных в словарь</param>
        /// <param name="log">Action по логированию поиска</param>
        /// <returns>Улица из БД. Если NULL, значит улица в соответствии со словарем не найдена</returns>
        internal RoyaltyRepository.Models.Street GetStreetByDictionary(Address address, RoyaltyRepository.Models.City city, bool doNotAddAnyDataToDictionary, Action<string> verboseLog = null)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            var log = new Action<string>((str) => { if (verboseLog != null) verboseLog($"{GetType().Name}.{nameof(GetStreetByDictionary)}() {str}"); });

            //lock (Account.Dictionary)
            try
            { 
                log($"Get all account records");

                ICollection<RoyaltyRepository.Models.AccountDictionaryRecord> recs = null;
                lock (Account.Dictionary)
                    recs = Account.Dictionary.Records.ToArray();

                log($"Get all streets in city '{city}'");

                var dictionary = city.Areas
                    //.AsParallel()
                    .SelectMany(a => a.Streets.Select(s => new
                    {
                        Area = a,
                        Street = s
                    }))
                    .LeftOuterJoin(recs, s => s.Street, r => r.Street, (s, r) => new
                    {
                        Street = s.Street,
                        Area = s.Area,
                        ChangeStreetTo = r?.ChangeStreetTo,
                        Conditions = r?.Conditions,
                        DictionaryRecord = r,
                    })
                    .ToArray();

                log($"Try to add street for sort dictionary");

                var dS = dictionary.AsParallel().Where(i => i.Street.Name.ToUpper() == address.Street.ToUpper()).ToArray();
                if (dS.Any())
                {
                    dictionary = dS;
                    log($"Street added for sort");
                }

                if (!string.IsNullOrWhiteSpace(address.Area))
                {
                    log($"Try to add area for sort dictionary");
                    var dA = dictionary.AsParallel().Where(i => i.Area.Name.ToUpper() == address.Area.ToUpper()).ToArray();
                    if (dA.Any())
                    {
                        dictionary = dA;
                        log($"Area added for sort");
                    }
                }

                log($"Get scores for dictionary and incoming address '{address}'");
                var subRes = dictionary
                    .AsParallel()
                    .Select(i => new
                    {
                        Street = i.ChangeStreetTo ?? i.Street,
                        i.Street.Area,
                        ConditionsScore = GetConditionsScore(address.House.Number, i.Conditions, doNotAddAnyDataToDictionary),
                        StreetScore = (decimal)new WordsMatching.MatchsMaker(address.Street, i.Street.Name).Score,
                        AreaScore = string.IsNullOrWhiteSpace(address.Area)
                            ? Account.Dictionary.SimilarityForTrust
                            : (decimal)new WordsMatching.MatchsMaker(address.Area, i.Street.Area.Name).Score,
                        i.DictionaryRecord,
                    });

                log($"Get nearest data");
                var res = subRes
                    .Where(i => i.StreetScore + i.AreaScore >= Account.Dictionary.SimilarityForTrust * 2 && i.ConditionsScore >= Account.Dictionary.ConditionsScoreForTrust)
                    .OrderByDescending(i => i.StreetScore + i.AreaScore + i.ConditionsScore / 2m)
                    .ThenByDescending(i => i.DictionaryRecord != null ? 1 : 0)
                    .ThenByDescending(i => i.Area.Streets.Count)
                    .FirstOrDefault();

                if (res == null)
                    log($"Street not found for address '{address}'");
                else
                {
                    log($"Founded street {(res.DictionaryRecord != null ? "in dictionary" : string.Empty)}for address '{address}': '{res}'");
                    if (Account.Dictionary.AllowAddToDictionaryAutomatically && !doNotAddAnyDataToDictionary)
                    {
                        if (res.DictionaryRecord == null)
                        {
                            lock (Account.Dictionary)
                               AddNewOrUpdateDictionaryRecord(address, res.Street, (str) => log(str));
                        }
                        else
                            ConcatDictionaryRecordConditions(address, new RoyaltyRepository.Models.AccountDictionaryRecord[] { res.DictionaryRecord }, (str) => log(str));
                    }
                }
                return res?.Street;
            }
            catch(Exception ex)
            {
                log(ex.GetExceptionText());
                throw;
            }
        }

        /// <summary>
        /// Получает улицу по адресу в соответствии со словарем
        /// </summary>
        /// <param name="address">Адрес для поиска</param>
        /// <param name="city">Город</param>
        /// <param name="verboseLog">Action по логированию поиска</param>
        /// <returns>Улица из БД. Если NULL, значит улица в соответствии со словарем не найдена</returns>
        public RoyaltyRepository.Models.Street GetStreetByDictionary(Address address, RoyaltyRepository.Models.City city, Action<string> verboseLog = null)
        {
            return GetStreetByDictionary(address, city, true, verboseLog);
        }

        /// <summary>
        /// Добавляет к условиям в словаре новое по адресу
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <param name="dictionaryRecords">Список из записей в словаре, куда добавлять условие</param>
        /// <param name="verboseLog">Action по логированию метода</param>
        internal void ConcatDictionaryRecordConditions(Address address, IEnumerable<RoyaltyRepository.Models.AccountDictionaryRecord> dictionaryRecords, Action<string> verboseLog = null)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var log = new Action<string>((str) => { if (verboseLog != null) verboseLog($"{GetType().Name}.{nameof(ConcatDictionaryRecordConditions)}() {str}"); });

            log(dictionaryRecords.Concat(i => $"Dictionary record already exists: '{i}'", Environment.NewLine));
            if (address.House.Number.HasValue)
            {
                // Если записей больше 1, то нужно определить, которую из них выбрать
                log("House number exists. Try to update conditions");
                var rowAndCondition = dictionaryRecords
                    .Select(ad => new
                    {
                        Record = ad,
                        Conditions = ad.Conditions.Select(c => new { Condition = c, Score = GetConditionsScore(address.House.Number, new RoyaltyRepository.Models.AccountDictionaryRecordCondition[] { c }, false) })
                    })
                    .Select(i => new
                    {
                        i.Record,
                        NearestCondition = i.Conditions.OrderByDescending(c => c.Score).FirstOrDefault()
                    })
                    .Select(i => new
                    {
                        i.Record,
                        NearestCondition = i.NearestCondition?.Condition,
                        NearestScore = i.NearestCondition?.Score ?? 0,
                    })
                    .OrderByDescending(i => i.NearestScore)
                    .OrderByDescending(i => i.Record.ChangeStreetTo == null ? 1 : 0)
                    .FirstOrDefault();

                var nearestCondition = rowAndCondition?.NearestScore >= Account.Dictionary.ConditionsScoreForTrust ? rowAndCondition?.NearestCondition : null;
                var adr = rowAndCondition.Record;

                if (nearestCondition != null)
                {
                    log($"Condition found: '{nearestCondition}'");
                    var isUpdated = false;
                    if (address.House.Number.Value < nearestCondition.From)
                    {
                        nearestCondition.From = address.House.Number.Value;
                        isUpdated = true;
                    }
                    else
                    if (address.House.Number.Value > nearestCondition.To)
                    {
                        nearestCondition.To = address.House.Number.Value;
                        isUpdated = true;
                    }

                    if (isUpdated)
                    {
                        log($"Condition update to '{nearestCondition}'");
                        log("Try to concat conditions");
                        var condLst = adr.Conditions.OrderBy(c => c.From).ToList();
                        for (int i = condLst.Count - 1; i >= 1; i--)
                        {
                            var lengthBetween = AreaMap.LengthBetween((uint)condLst[i - 1].To, (uint)condLst[i].From);
                            var maxScore = new decimal[] { AreaMap.Weight(lengthBetween, Math.Abs(condLst[i - 1].From - condLst[i - 1].To) + 1m), AreaMap.Weight(lengthBetween, Math.Abs(condLst[i].From - condLst[i].To) + 1m) }.Max();

                            if (maxScore >= Account.Dictionary.ConditionsScoreForTrust || condLst[i - 1].To >= condLst[i].From)
                            {
                                log($"Concat conditions '{condLst[i]}' and '{condLst[i - 1]}'");
                                condLst[i - 1].To = condLst[i].To;
                                condLst[i - 1].From = Math.Min(condLst[i].From, condLst[i - 1].From);
                                log($"New condition is '{condLst[i - 1]}'");
                                adr.Conditions.Remove(condLst[i]);
                            }
                        }
                    } else
                        log($"No one update needed");
                }
                else
                {
                    var cond = Repository.AccountDictionaryRecordConditionNew(adr, address.House.Number, address.House.Number);
                    log($"Add condition for dictionary record {cond}");
                }
            }
            else
            {
                log("House number not setted. Leave as is.");
            }
        }

        /// <summary>
        /// Добавляет новую запись или обновляет существующую запись в словаре для адреса
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <param name="street">Улица</param>
        /// <param name="verboseLog">Action по логированию метода</param>
        internal void AddNewOrUpdateDictionaryRecord(Address address, RoyaltyRepository.Models.Street street, Action<string> verboseLog = null)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var log = new Action<string>((str) => { if (verboseLog != null) verboseLog($"{GetType().Name}.{nameof(AddNewOrUpdateDictionaryRecord)}() {str}"); });

            log("Try get record to dictionary");
            var dictionaryRecords = Account.Dictionary.Records.Where(ad => ad.Street == street);
            if (!dictionaryRecords.Any())
            {
                log("Dictionary record not found. Create new and add it");
                var adr = Repository.AccountDictionaryRecordNew(Account.Dictionary, street: street);
                if (address.House.Number.HasValue)
                {
                    var cond = Repository.AccountDictionaryRecordConditionNew(adr, address.House.Number, address.House.Number);
                    log($"Add condition for dictionary record {cond.ToString()}");
                }
                log($"Add dictionary record: {adr.ToString()}");
            }
            else
            {
                ConcatDictionaryRecordConditions(address, dictionaryRecords, (str) => log(str));
            }
        }

        /// <summary>
        /// Создает новую улицу и добавляет в словарь, если это необходимо
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <param name="city">Город</param>
        /// <param name="doNotAddAnyDataToDictionary">Флаг для отмены добавления данных в словарь</param>
        /// <param name="verboseLog">Action по логированию метода</param>
        /// <returns>Улицу</returns>
        internal RoyaltyRepository.Models.Street GetNewStreet(Address address, RoyaltyRepository.Models.City city, bool doNotAddAnyDataToDictionary, Action<string> verboseLog = null)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            var log = new Action<string>((str) => { if (verboseLog != null) verboseLog($"{GetType().Name}.{nameof(GetNewStreet)}() {str}"); });
            bool isNewArea = false;

            try
                {
                    log($"Incoming parameters: address is '{address}' and city is '{city}'");
                    RoyaltyRepository.Models.Area a = null;
                    if (string.IsNullOrWhiteSpace(address.Area))
                    {
                        log($"Area is unsetted. Use default aread for city: '{city.UndefinedArea}'");
                        a = city.UndefinedArea;
                    } else
                    {
                        log($"Try get area with name '{address.Area}' from database");
                        a = Repository.AreaGet(address.Area, city);
                        if (a == null)
                        {
                            log($"Area not found. Create new");
                            a = Repository.AreaNew(address.Area, city: city);
                            log($"New area for addres is {a}");
                            isNewArea = true;
                        } else
                            log($"Area found: {a}");
                    }

                    RoyaltyRepository.Models.Street s = null;
                    if (!isNewArea)
                    {
                        log($"Try to get street from database");
                        s = Repository.StreetGet(address.Street, a);
                        if (s != null)
                            log($"Street found in database: {s}"); else
                            log($"Street '{address.Street}' with area '{a}' not found in database");
                    }
                    if (s == null)
                    {
                        s = a != city.UndefinedArea ? Repository.StreetGet(address.Street, city.UndefinedArea) : null;
                        if (s != null)
                        {
                            log($"Street found in undefined area in database: {s}. Change area for this street to '{a}'");
                            s.Area = a;
                        }
                        else
                        {
                            s = Repository.StreetNew(address.Street, a);
                            log($"New street created: {s}");
                        }
                    }
                    else
                        log($"Street found in database: {s}");

                    if (Account.Dictionary.AllowAddToDictionaryAutomatically && !doNotAddAnyDataToDictionary)
                    lock (Account.Dictionary)
                        AddNewOrUpdateDictionaryRecord(address, s, (str) => log(str));

                    return s;
                }
                catch(Exception)
                {
                    throw;
                }
        }

        /// <summary>
        /// Получение списка улиц в соответствии с адресами
        /// </summary>
        /// <param name="incomingAddresses">Адреса, для которых нужно определить улицы</param>
        /// <param name="city">Город</param>
        /// <param name="doNotAddAnyDataToDictionary">Флаг для отмены добавления данных в словарь</param>
        /// <param name="reportProgress">Action для отслеживания процесса выполнения</param>
        /// <returns>Список соответствия входящему адресу найденной улицы</returns>
        internal IDictionary<Address, RoyaltyRepository.Models.Street> GetStreets(
            IEnumerable<Address> incomingAddresses,
            RoyaltyRepository.Models.City city,
            bool doNotAddAnyDataToDictionary = false,
            Action<decimal> reportProgress = null,
            Action<string> verboseLog = null)
        {
            var pp = new Helpers.PercentageProgress();
            var ppLoad = pp.GetChild(weight: 0.5m);
            var ppSubitem = pp.GetChild(weight: 9m);
            var ppEnd = pp.GetChild(weight: 0.5m);
            pp.Change += (s, e) => { if (reportProgress != null) reportProgress(e.Value); };
            verboseLog = verboseLog ?? new Action<string>(s => { });

            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(GetStreets)}()"))
                try
                {
                    logSession.Output = new Action<IEnumerable<string>>( (strs) => strs.ToList().ForEach(s => verboseLog(s)) );

                    var findStreet = incomingAddresses
                        .Distinct()
                        .OrderByDescending(a => a.Area)
                        .ThenByDescending(a => a.Street)
                        .ThenByDescending(a => a.House.ToString())
                        .LeftOuterJoin(Account.Dictionary.Records, //.Where(r => r.Street != null && r.Street.Area != null),
                            s => new { StreetName = s.Street, AreaName = s.Area },
                            d => new { StreetName = d.Street.Name, AreaName = d.Street.Area.Name },
                            (s, d) => new
                            {
                                IncomingAddress = s,
                                Street = (d != null ? d.ChangeStreetTo ?? d.Street : null),
                                ConditionsScore = GetConditionsScore(s.House.Number, d == null ? null : d.Conditions, doNotAddAnyDataToDictionary),
                            })
                        .ToArray();

                    var count = findStreet.Length;
                    int current = 0;
                    ppLoad.Value = 90;

                    var res = findStreet
                        .Select(i =>
                        {
                            var subRes = new
                            {
                                i.IncomingAddress,
                                Street = i.Street
                                    ?? GetStreetByDictionary(i.IncomingAddress, city, doNotAddAnyDataToDictionary, (s) => logSession.Add(s))
                                    ?? GetNewStreet(i.IncomingAddress, city, doNotAddAnyDataToDictionary, (s) => logSession.Add(s))
                            };
                            current++;
                            ppSubitem.Value = (decimal)current / (decimal)count * 100m;
                            return subRes;
                        }
                        )
                        .GroupBy(i => i.IncomingAddress)
                        .Select(g => new { IncomingAddress = g.Key, Items = g })
                        //Исключаем случаи, когда в словаре более одной записи на входящий адрес (что-то пошло не так, и мы генерируем ошибку)
                        .Select(g => 
                        {
                            if (g.Items.Count() > 1)
                            {
                                var ex = new Exception($"For incoming address '{g.IncomingAddress.ToString()}' found more then 1 street. See data for details");
                                int index = 0;
                                foreach (var i in g.Items.Select(n => n.Street))
                                    ex.Data.Add(index++, i.ToString());

                                throw ex;
                            }

                            return new
                            {
                                g.IncomingAddress,
                                g.Items.FirstOrDefault().Street
                            };
                        })
                        .ToArray();

                    ppLoad.Value = 100;

                    logSession.Add($"Array constructed with {res.Length} elements for city '{city}'. Try to create dictionary.");
                    return res.ToDictionary(i => i.IncomingAddress, i => i.Street);
                }
                catch(Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw;
                }
                finally
                {
                    ppEnd.Value = 100;
                }
        }

        /// <summary>
        /// Получение списка улиц в соответствии с адресами
        /// </summary>
        /// <param name="incomingAddresses">Адреса, для которых нужно определить улицы</param>
        /// <param name="city">Город</param>
        /// <param name="reportProgress">Action для отслеживания процесса выполнения</param>
        /// <returns>Список соответствия входящему адресу найденной улицы</returns>
        public IDictionary<Address, RoyaltyRepository.Models.Street> GetStreets(
            IEnumerable<Address> incomingAddresses,
            RoyaltyRepository.Models.City city,
            Action<decimal> reportProgress = null,
            Action<string> verboseLog = null)
        {
            return GetStreets(incomingAddresses, city, true, reportProgress, verboseLog);
        }
    }
}
