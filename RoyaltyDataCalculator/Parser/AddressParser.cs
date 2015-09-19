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
        public bool ConditionsValid(uint houseNumber, ICollection<RoyaltyRepository.Models.AccountDictionaryRecordCondition> conditions, bool doNotAddAnyDataToDictionary)
        {
            //TODO: Проверка и изменение условий
            return true;
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
            verboseLog = verboseLog ?? new Action<string>((s) => { });
            var res = Account.Dictionary.Records
                .Where(r => r.Street != null && r.Street.Area != null)
                .Where(r => r.Street.Area.City == city)
                .AsParallel()
                .Select(i => new
                {
                    Street = i.ChangeStreetTo ?? i.Street,
                    i.Street.Area,
                    AllowedConditions = i.Conditions.Any() && address.House.Number.HasValue ? ConditionsValid(address.House.Number.Value, i.Conditions, doNotAddAnyDataToDictionary) : true,
                    Score = (decimal)new WordsMatching.MatchsMaker(address.Street, i.Street.Name).Score,
                    AreaScore = string.IsNullOrWhiteSpace(address.Area) 
                        ? Account.Dictionary.SimilarityForTrust 
                        : (decimal)new WordsMatching.MatchsMaker(address.Area, i.Street.Area.Name).Score
                })
                .Where(i => i.AllowedConditions)
                .Where(i => i.Score + i.AreaScore >= Account.Dictionary.SimilarityForTrust * 2)
                .OrderByDescending(i => i.AreaScore)
                .ThenByDescending(i => i.Score)
                .Select(i => i.Street)
                .FirstOrDefault();
            if (res == null)
                verboseLog(string.Format("street not found for address '{0}'", address.ToString()));
            else
                verboseLog(string.Format("founded street for address '{0}': '{1}'", address.ToString(), res.ToString()));
            return res;
        }

        /// <summary>
        /// Создает новую улицу и добавляет в словарь, если это необходимо
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <param name="city">Город</param>
        /// <param name="doNotAddAnyDataToDictionary">Флаг для отмены добавления данных в словарь</param>
        /// <param name="verboseLog">Action по логированию метода</param>
        /// <returns>Улицу</returns>
        public RoyaltyRepository.Models.Street GetNewStreet(Address address, RoyaltyRepository.Models.City city, bool doNotAddAnyDataToDictionary, Action<string> verboseLog = null)
        {
            verboseLog = verboseLog ?? new Action<string>((str) => { });
            verboseLog(string.Format("create new street with name '{0}' and area name '{1}'", address.Street, address.Area));
            var a = address.Area == string.Empty
                ? city.UndefinedArea
                : Repository.AreaGet(address.Area, city) ?? Repository.AreaNew(address.Area, city: city);
            verboseLog(string.Format("area for street: '{0}'", a.ToString()));

            var s = Repository.StreetNew(address.Street, a);
            verboseLog(string.Format("street: '{0}'", s.ToString()));
            if (Account.Dictionary.AllowAddToDictionaryAutomatically && !doNotAddAnyDataToDictionary)
            {
                verboseLog("Try add record to dictionary");
                verboseLog(string.Format("added dictionary record: {0}", Repository.AccountDictionaryRecordNew(Account.Dictionary, street: s).ToString()));
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
                        .Distinct()
                        .LeftOuterJoin(Account.Dictionary.Records.Where(r => r.Street != null && r.Street.Area != null),
                            s => new { Street = s.Street, Area = s.Area },
                            d => new { Street = d.Street.Name, Area = d.Street.Area.Name },
                            (s, d) => new
                            {
                                IncomingAddress = s,
                                Street = (d != null ? d.ChangeStreetTo ?? d.Street : null)
                            })
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
