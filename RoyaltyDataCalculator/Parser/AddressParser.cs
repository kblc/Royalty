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
        public decimal GetConditionsScore(uint? houseNumber, ICollection<RoyaltyRepository.Models.AccountDictionaryRecordCondition> conditions, bool doNotAddAnyDataToDictionary)
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
                    Radius = Math.Abs(c.From - c.To),
                    Even = c.Even,
                })
                .Where(c => c.LengthBetween != uint.MaxValue)
                .Select(c => AreaMap.Weight(c.LengthBetween, c.Radius) + ((c.Even.HasValue ? (c.Even.Value == isHouseNumberEven) : false) ? 0.2m : 0.0m))
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
        /// <param name="verboseLog">Action по логированию поиска</param>
        /// <returns>Улица из БД. Если NULL, значит улица в соответствии со словарем не найдена</returns>
        public RoyaltyRepository.Models.Street GetStreetByDictionary(Address address, RoyaltyRepository.Models.City city, bool doNotAddAnyDataToDictionary, Action<string> verboseLog = null)
        {
            try
            { 
                if (address == null)
                    throw new ArgumentNullException(nameof(address));
                if (city == null)
                    throw new ArgumentNullException(nameof(city));

                verboseLog = verboseLog ?? new Action<string>((str) => { });

                verboseLog(string.Format(nameof(GetStreetByDictionary) + "() get all streets in city '{0}'", city));
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
                        Score = (decimal)new WordsMatching.MatchsMaker(address.Street, i.Street.Name).Score,
                        AreaScore = string.IsNullOrWhiteSpace(address.Area) 
                            ? Account.Dictionary.SimilarityForTrust 
                            : (decimal)new WordsMatching.MatchsMaker(address.Area, i.Street.Area.Name).Score,
                        i.InDictionary,
                    })
                    .Where(i => i.Score + i.AreaScore >= Account.Dictionary.SimilarityForTrust * 2 && i.ConditionsScore >= Account.Dictionary.ConditionsScoreForTrust)
                    .OrderByDescending(i => i.Score + i.AreaScore + i.ConditionsScore / 2m)
                    .FirstOrDefault();

                if (res == null)
                    verboseLog(string.Format(nameof(GetStreetByDictionary) + "() street not found for address '{0}'", address.ToString()));
                else
                {
                    verboseLog(string.Format(nameof(GetStreetByDictionary) + "() founded street {2}for address '{0}': '{1}'", address.ToString(), res.ToString(), res.InDictionary ? "in dictionary" : string.Empty));
                    if (!res.InDictionary && Account.Dictionary.AllowAddToDictionaryAutomatically && !doNotAddAnyDataToDictionary)
                    {
                        verboseLog(nameof(GetStreetByDictionary) + "() Try add record to dictionary");
                        verboseLog(string.Format(nameof(GetStreetByDictionary) + "() added dictionary record: {0}", Repository.AccountDictionaryRecordNew(Account.Dictionary, street: res.Street).ToString()));
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
            var log = new Action<string>((str) => { if (verboseLog != null) verboseLog(nameof(GetNewStreet) + "() " + str); });

            log(string.Format("Create new street with name '{0}' and area name '{1}'", address.Street, address.Area));
            var a = string.IsNullOrWhiteSpace(address.Area)
                ? city.UndefinedArea
                : Repository.AreaGet(address.Area, city) ?? Repository.AreaNew(address.Area, city: city);
            log(string.Format("Area for street: '{0}'", a.ToString()));

            var s = Repository.StreetGet(address.Street, a) ?? Repository.StreetNew(address.Street, a);
            log(string.Format("Street: '{0}'", s.ToString()));
            if (Account.Dictionary.AllowAddToDictionaryAutomatically && !doNotAddAnyDataToDictionary)
            {
                log("Try add record to dictionary");
                var adr = Account.Dictionary.Records.FirstOrDefault(ad => ad.Street == s);
                if (adr == null)
                    log(string.Format("Add dictionary record: {0}", Repository.AccountDictionaryRecordNew(Account.Dictionary, street: s).ToString())); else
                {
                    log(string.Format("Dictionary record already exists: '{0}'", adr.ToString()));
                    if (address.House.Number != null)
                    {
                        log(string.Format("House number exists. Try to update conditions"));
                        //TODO: Update conditions
                    } else
                        log(string.Format("House number not exists. Leave as is."));
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
            using (var logSession = Helpers.Log.Session(this.GetType().Name + ".GetStreets()", true))
                try
                {
                    var result = incomingAddresses
                        .Distinct(GenericEqualityComparer<Address>.Get(a => a.ToString()))
                        .LeftOuterJoin(Account.Dictionary.Records, //.Where(r => r.Street != null && r.Street.Area != null),
                            s => new { StreetName = s.Street, AreaName = s.Area },
                            d => new { StreetName = d.Street.Name, AreaName = d.Street.Area.Name },
                            (s, d) => new
                            {
                                IncomingAddress = s,
                                Street = (d != null ? d.ChangeStreetTo ?? d.Street : null),
                                ConditionsScore = GetConditionsScore(s.House.Number, d == null ? null : d.Conditions, doNotAddAnyDataToDictionary),
                            })
                        //.GroupBy(i => i.IncomingAddress)
                        ////Исключаем случаи, когда в словаре более одной записи на входящий адрес (2 разных условия)
                        //.Select(g => new { IncomingAddress = g.Key, Items = g.OrderBy(i => i.IsConditionsValid ? 0 : 1) })
                        //.Select(g => new { g.IncomingAddress, Street = (g.Items.Count() == 1 ? g.Items.FirstOrDefault().Street : null) })
                        .ToArray()
                        .Select(i => new
                        {
                            i.IncomingAddress,
                            Street = i.Street
                                ?? GetStreetByDictionary(i.IncomingAddress, city, doNotAddAnyDataToDictionary, (s) => logSession.Add(s)) 
                                ?? GetNewStreet(i.IncomingAddress, city, doNotAddAnyDataToDictionary, (s) => logSession.Add(s))
                        })
                        .ToArray()
                        .ToDictionary(i => i.IncomingAddress, i => i.Street);

                    return result;
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
