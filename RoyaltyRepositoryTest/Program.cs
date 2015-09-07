using RoyaltyRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepositoryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            { 
                using ( var rc = new RepositoryContext("name=connectionString"))
                {
                    Account acc = new Account() { AccountUID = Guid.NewGuid(), Name = ".default", IsHidden = true };
                    acc.Settings.AddressColumnName = "Адрес объекта";
                    acc.Settings.AreaColumnName = "Район";
                    acc.Settings.MarkColumnName = "Метка";
                    acc.Settings.HostColumnName = "URL";
                    acc.Settings.PhoneColumnName = "Контакты для связи";
                    acc.Settings.ExecuteAfterAnalizeCommand = string.Empty;
                    acc.Settings.FolderExportAnalize = string.Empty;
                    acc.Settings.FolderExportPhones = string.Empty;
                    acc.Settings.FolderImportAnalize = string.Empty;
                    acc.Settings.FolderImportMain = string.Empty;
                    acc.Settings.IgnoreExportTime = TimeSpan.FromHours(1);
                    acc.Settings.DeleteFileAfterImport = false;
                    acc.Settings.RecursiveFolderSearch = true;
                    acc.State.IsActive = false;
                    acc.Settings.TimeForTrust = TimeSpan.FromDays(30);
                    acc.Settings.WaitExecutionAfterAnalize = true;
                    acc.Dictionary.AllowAddToDictionaryAutomatically = true;
                    acc.Dictionary.AllowCalcAreasIfStreetExistsOnly = false;
                    acc.Dictionary.SimilarityForTrust = 0.6m;
                    foreach (var i in new string[] { 
                        ".",",","|","(",")",@"\","/","~","!","@","#","$","%","^","&","*","<",">","?",";","'","\"",":","[","]","{","}","+","_","`",
                        "ул",
                        "ул.",
                        "улица",
                        "пр",
                        "пр.",
                        "про.",
                        "прос",
                        "проспект",
                        "пл",
                        "пл.",
                        "площадь",
                        "ст.",
                        "стр.",
                        "ст",
                        "стр",
                        "строение",
                        "-ое",
                        "-ая",
                        "-е",
                        "-ья",
                        "-я",
                        "-ый",
                        "-ой",
                        "-ий",
                        "-е",
                        }
                        .Select(s => new AccountDictionaryExclude() { Dictionary = acc.Dictionary, Exclude = s }))
                        acc.Dictionary.Excludes.Add(i);
                    rc.Accounts.Add(acc);
                    rc.SaveChanges();
                }
            }
            catch(System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var e in ex.EntityValidationErrors)
                    foreach(var e2 in e.ValidationErrors)
                        Console.WriteLine(string.Format("[-] Validation error for {0}. Property '{1}' error: {2}", e.Entry.Entity, e2.PropertyName, e2.ErrorMessage));
            }
            catch(Exception ex)
            {
                var e = ex;
                while (e != null)
                {
                    Console.WriteLine(string.Format("[-] Exception: {0}", e.Message));
                    e = e.InnerException;
                }
            }
            Console.WriteLine("[~] Test end");
            Console.ReadKey();
        }
    }
}
