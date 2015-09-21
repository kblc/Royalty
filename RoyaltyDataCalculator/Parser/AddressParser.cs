using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Linq;

namespace RoyaltyDataCalculator.Parser
{
    public class AddressParserIncomingParameter
    {
        public Address Address { get; set; }
        public RoyaltyRepository.Models.City City { get; set; }
    }

    public class AddressParserResult
    {
        #region AddressParserIncomingParameter
        public Address IncomingAddress { get; set; }
        public RoyaltyRepository.Models.City City { get; set; }
        #endregion
        public Address Address { get; set; }
        public RoyaltyRepository.Models.Area Area { get; set; }
        public RoyaltyRepository.Models.Street Street { get; set; }
        public bool IsNewArea { get; set; }
        public bool IsNewStreet { get; set; }
    }

    /// <summary>
    /// Парсер адресов. Используйте метод Parse()
    /// </summary>
    public class AddressParser
    {
        public RoyaltyRepository.Models.Account Account { get; private set; }
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
        public IEnumerable<AddressParserResult> Parse(IEnumerable<AddressParserIncomingParameter> incomingData)
        {
            using (var logSession = Helpers.Log.Session(GetType().Name + ".Parse()", isEnabled: false))
                try
                {
                    var res = incomingData
                        .GroupBy(i => i.City)
                        .Select(g => new
                        {
                            City = g.FirstOrDefault().City,
                            Streets = GetStreets(g.Select(i => i.Address), g.FirstOrDefault().City),
                            Items = g,
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
                        .Select(item => new AddressParserResult()
                        {
                            City = item.IncomingParameters.City,
                            IncomingAddress = item.IncomingParameters.Address,
                            Address = item.Data.Address,
                            Street = item.Data.Street,
                            Area = item.Data.Street.Area,
                            IsNewStreet = item.Data.Street.StreetID == 0,
                            IsNewArea = item.Data.Street.Area.AreaID == 0
                        })
                        .ToArray();

                    return res;
                }
                catch(Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw;
                }
        }

        /// <summary>
        /// Проверяет условия, при которых назначается адрес. Если условия немного не совпадают - обновляет условия
        /// </summary>
        /// <param name="houseNumber">Номер дома</param>
        /// <param name="conditions">Условия</param>
        /// <param name="doNotAddAnyDataToDictionary">Флаг для отмены добавления данных в словарь</param>
        /// <returns>True если условия соблюдены</returns>
        public decimal GetConditionsScore(uint? houseNumber, IEnumerable<RoyaltyRepository.Models.AccountDictionaryRecordCondition> conditions, bool doNotAddAnyDataToDictionary)
        {
            if (conditions == null || !conditions.Any() || !houseNumber.HasValue)
                return 1m;
            bool isHouseNumberEven = houseNumber.Value % 2 == 0;

            var resultWeight = conditions
                .AsParallel()
                .Where(c => c.Even.HasValue ? (c.Even.Value == isHouseNumberEven) : true)
                .Select(c => new { c.From, c.To, c.Even, IsInside = houseNumber.Value >= c.From && houseNumber <= c.To })
                .Select(c => new
                {
                    LengthBetween = c.IsInside
                        ? 0
                        : new long[] { c.From, c.To }.Distinct().Select(i => AreaMap.LengthBetween(houseNumber.Value, (uint)i)).Union(new uint[] { uint.MaxValue }).Min(),
                    Radius = Math.Abs(c.From - c.To) + 1,
                    Even = c.Even,
                })
                .Where(c => c.LengthBetween != uint.MaxValue)
                .Select(c => (c.LengthBetween == 0 ? 1m : AreaMap.Weight(c.LengthBetween, c.Radius)) +
                             (c.Even.HasValue ? 0.2m : 0.0m))
                .OrderByDescending(i => i)
                .FirstOrDefault();

            //TODO: Проверка и изменение условий
            return resultWeight;
        }

        /// <summary>
        /// ПОлучает услицу по адресу в соответствии со словарем
        /// </summary>
        /// <param name="address">Адрес для поиска</param>
        /// <param name="city">Город</param>
        /// <param name="doNotAddAnyDataToDictionary">Флаг для отмены добавления данных в словарь</param>
        /// <param name="log">Action по логированию поиска</param>
        /// <returns>Улица из БД. Если NULL, значит улица в соответствии со словарем не найдена</returns>
        public RoyaltyRepository.Models.Street GetStreetByDictionary(Address address, RoyaltyRepository.Models.City city, bool doNotAddAnyDataToDictionary, Action<string> verboseLog = null)
        {
            try
            { 
                if (address == null)
                    throw new ArgumentNullException(nameof(address));
                if (city == null)
                    throw new ArgumentNullException(nameof(city));

                var t = System.Threading.Thread.CurrentThread.ManagedThreadId;

                var log = new Action<string>((str) => { if (verboseLog != null) verboseLog($"{GetType().Name}.{nameof(GetStreetByDictionary)}() {str}"); });

                log($"Get all streets in city '{city}'");
                var dictionary = city.Areas
                    .SelectMany(a => a.Streets.Select(s => new
                    {
                        Area = a,
                        Street = s
                    }))
                    .LeftOuterJoin(Account.Dictionary.Records, s => s.Street, r => r.Street, (s, r) => new
                    {
                        Street = s.Street,
                        Area = s.Area,
                        ChangeStreetTo = r != null ? r.ChangeStreetTo : null,
                        Conditions = r != null ? r.Conditions : null,
                        InDictionary = r != null,
                    })
                    .ToArray();

                var res = dictionary //Account.Dictionary.Records
                    //.Where(r => r.Street != null && r.Street.Area != null)
                    //.Where(r => r.Street.Area.City == city)
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
                        i.InDictionary,
                    })
                    .Where(i => i.StreetScore + i.AreaScore >= Account.Dictionary.SimilarityForTrust * 2 && i.ConditionsScore >= Account.Dictionary.ConditionsScoreForTrust)
                    .OrderByDescending(i => i.StreetScore + i.AreaScore + i.ConditionsScore / 2m)
                    .ThenByDescending(i => i.InDictionary ? 1 : 0)
                    .ThenByDescending(i => i.Area.Streets.Count)
                    .FirstOrDefault();

                if (res == null)
                    log($"Street not found for address '{address}'");
                else
                {
                    log($"Founded street {(res.InDictionary ? "in dictionary" : string.Empty)}for address '{address}': '{res}'");
                    if (!res.InDictionary && Account.Dictionary.AllowAddToDictionaryAutomatically && !doNotAddAnyDataToDictionary)
                    {
                        log($"Try add record to dictionary");
                        var adr = Repository.AccountDictionaryRecordNew(Account.Dictionary, street: res.Street);
                        log($"Added dictionary record: {adr}");
                    }
                }
                return res?.Street;
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Создает новую улицу и добавляет в словарь, если это необходимо
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <param name="city">Город</param>
        /// <param name="doNotAddAnyDataToDictionary">Флаг для отмены добавления данных в словарь</param>
        /// <param name="log">Action по логированию метода</param>
        /// <returns>Улицу</returns>
        public RoyaltyRepository.Models.Street GetNewStreet(Address address, RoyaltyRepository.Models.City city, bool doNotAddAnyDataToDictionary, Action<string> verboseLog = null)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            var log = new Action<string>((str) => { if (verboseLog != null) verboseLog($"{GetType().Name}.{nameof(GetNewStreet)}() {str}"); });
            bool isNewArea = false;

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
            {
                log("Try get record to dictionary");
                var adr = Account.Dictionary.Records.FirstOrDefault(ad => ad.Street == s);
                if (adr == null)
                {
                    log("Dictionary record not found. Create new and add it");
                    adr = Repository.AccountDictionaryRecordNew(Account.Dictionary, street: s);
                    if (address.House.Number.HasValue)
                    {
                        var cond = Repository.AccountDictionaryRecordConditionNew(adr, address.House.Number, address.House.Number);
                        log($"Add condition for dictionary record {cond.ToString()}");
                    } 
                    log($"Add dictionary record: {adr.ToString()}");
                } else
                {
                    log($"Dictionary record already exists: '{adr.ToString()}'");
                    if (address.House.Number.HasValue)
                    {
                        log("House number exists. Try to update conditions");
                        var nearestCondition = adr.Conditions
                            .Select(c => new { Condition = c, Score = GetConditionsScore(address.House.Number, new RoyaltyRepository.Models.AccountDictionaryRecordCondition[] { c }, doNotAddAnyDataToDictionary) })
                            .OrderByDescending(i => i.Score)
                            .Where(i => i.Score > Account.Dictionary.ConditionsScoreForTrust)
                            .Select(i => i.Condition)
                            .FirstOrDefault();

                        if (nearestCondition != null)
                        {
                            if (address.House.Number.Value < nearestCondition.From)
                                nearestCondition.From = address.House.Number.Value;
                            else
                            if (address.House.Number.Value > nearestCondition.To)
                                nearestCondition.To = address.House.Number.Value;
                            log($"Condition found and update to '{nearestCondition.ToString()}'");
                            log("Try to concat conditions");

                            var condLst = adr.Conditions.OrderBy(c => c.From).ToList();
                            for (int i = condLst.Count - 1; i >= 1; i--)
                            {
                                var lengthBetween = AreaMap.LengthBetween((uint)condLst[i - 1].To, (uint)condLst[i].From);
                                if (new decimal[] { AreaMap.Weight(lengthBetween, Math.Abs(condLst[i - 1].From - condLst[i - 1].To) + 1m), AreaMap.Weight(lengthBetween, Math.Abs(condLst[i].From - condLst[i].To) + 1m) }.Max() >= Account.Dictionary.ConditionsScoreForTrust)
                                {
                                    log($"Concat conditions '{condLst[i].ToString()}' and '{condLst[i-1].ToString()}'");
                                    condLst[i - 1].To = condLst[i].To;
                                    log($"New condition is '{condLst[i - 1].To}'");
                                    adr.Conditions.Remove(condLst[i]);
                                }
                            }
                        }
                        else
                        {
                            var cond = Repository.AccountDictionaryRecordConditionNew(adr, address.House.Number, address.House.Number);
                            log($"Add condition for dictionary record {cond.ToString()}");
                        }
                    } else
                        log("House number not setted. Leave as is.");
                }
            }
            return s;
        }

        /// <summary>
        /// Получение списка улиц в соответствии с адресами
        /// </summary>
        /// <param name="incomingAddresses">Адреса, для которых нужно определить улицы</param>
        /// <param name="city">Город</param>
        /// <param name="doNotAddAnyDataToDictionary"></param>
        /// <returns>Флаг для отмены добавления данных в словарь</returns>
        public IDictionary<Address, RoyaltyRepository.Models.Street> GetStreets(
            IEnumerable<Address> incomingAddresses,
            RoyaltyRepository.Models.City city,
            bool doNotAddAnyDataToDictionary = false)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(GetStreets)}()", true))
                try
                {
                    var res = incomingAddresses
                        //.Distinct(GenericEqualityComparer<Address>.Get(a => a.ToString()))
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
                        .ToArray()
                        .Select(i => new
                        {
                            i.IncomingAddress,
                            Street = i.Street
                                ?? GetStreetByDictionary(i.IncomingAddress, city, doNotAddAnyDataToDictionary, (s) => logSession.Add(s)) 
                                ?? GetNewStreet(i.IncomingAddress, city, doNotAddAnyDataToDictionary, (s) => logSession.Add(s))
                        })
                        //Если было 2 одинаковых входящих адреса (речь об объектах, а не о строке с адресом)
                        .Distinct()
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

                    logSession.Add($"Array constructed with {res.Length} elements for city '{city}'. Try to create dictionary.");
                    return res.ToDictionary(i => i.IncomingAddress, i => i.Street);
                }
                catch(Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw;
                }
        }
    }
}
