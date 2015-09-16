using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Linq;

namespace RoyaltyDataCalculator.AddressParser
{
    public class AddressParser
    {
        public static RoyaltyRepository.Models.Street GetStreet(
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

        public static IDictionary<string, RoyaltyRepository.Models.Street> GetStreets(
            IEnumerable<string> incomingStreets,
            RoyaltyRepository.Repository repository,
            RoyaltyRepository.Models.AccountDictionary accountDictionary,
            RoyaltyRepository.Models.City city,
            bool doNotAddAnyDataToDictionary = false)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (accountDictionary == null)
                throw new ArgumentNullException("accountDictionary");
            if (city == null)
                throw new ArgumentNullException("city");

            var dictionary = accountDictionary.Records
                .Select(r => new { Street = r.Street, ChangeStreetTo = r.ChangeStreetTo })
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
                .ToDictionary(i => i.IncomingStreet, i => i.Street);

            return result;
        }
    }
}
