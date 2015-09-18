using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Linq;

namespace RoyaltyDataCalculator.Parser
{
    public static class AddressParser
    {
        public static string GetStreet(
            string incomingStreet,
            RoyaltyRepository.Repository repository,
            RoyaltyRepository.Models.AccountDictionary accountDictionary,
            RoyaltyRepository.Models.City city,
            bool doNotAddAnyDataToDictionary = false)
        {
            return GetStreets(new string[] { incomingStreet }, repository, accountDictionary, city, doNotAddAnyDataToDictionary)
                .Select(i => i.Value)
                .FirstOrDefault();
        }

        public static IDictionary<string, string> GetStreets(
            IEnumerable<string> incomingStreets,
            RoyaltyRepository.Repository repository,
            RoyaltyRepository.Models.AccountDictionary accountDictionary,
            RoyaltyRepository.Models.City city,
            bool doNotAddAnyDataToDictionary = false)
        {
            if (incomingStreets == null)
                throw new ArgumentNullException("incomingStreets");
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (accountDictionary == null)
                throw new ArgumentNullException("accountDictionary");
            if (city == null)
                throw new ArgumentNullException("city");

            var dictionary = accountDictionary.Records
                .Join(repository.StreetGet(), r => r.StreetID, s => s.StreetID, (r,s) => new { r.Street, r.ChangeStreetTo })
                .Join(repository.AreaGet().Where(a => a.CityID == city.CityID), r => r.Street.AreaID, a => a.AreaID, (r,a) => new { r.Street, r.ChangeStreetTo, Area = a })
                .ToArray();

            var getNewStreet = new Func<string, RoyaltyRepository.Models.Street>((streetName) => 
            {
                if (accountDictionary.AllowAddToDictionaryAutomatically && !doNotAddAnyDataToDictionary)
                    return repository.StreetNew(streetName, city.UndefinedArea);
                else
                    return null;
            });

            var result = incomingStreets
                .Distinct()
                .LeftOuterJoin(dictionary,
                    a => a,
                    d => d.Street.Name,
                    (a, d) => new
                    {
                        IncomingStreet = a,
                        Street = (d != null ? d.ChangeStreetTo ?? d.Street : null) ??
                        (
                            dictionary.AsParallel().Select(i => new 
                            {
                                i.Street,
                                i.ChangeStreetTo,
                                Score =  (decimal)new WordsMatching.MatchsMaker(a, i.Street.Name).Score
                            })
                            .Where(i => i.Score >= accountDictionary.SimilarityForTrust)
                            .OrderByDescending(i => i.Score)
                            .Select(i => i.ChangeStreetTo ?? i.Street)
                            .FirstOrDefault()
                        ) ?? getNewStreet(a)
                    })
                .GroupBy(i => i.IncomingStreet)
                .Select(g => new { g.FirstOrDefault().IncomingStreet, Street = g.FirstOrDefault().Street.Name })
                .ToDictionary(i => i.IncomingStreet, i => i.Street);

            return result;
        }


    }
}
