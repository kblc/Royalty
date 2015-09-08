using RoyaltyRepository.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
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
                using ( var rc = new RepositoryContext("connectionString"))
                {
                    rc.Log = (s) => { Console.WriteLine(string.Format("[~] SQL: {0}", s)); };

                    Account acc = new Account() 
                    { 
                        AccountUID = Guid.NewGuid(), 
                        Name = ".default2", 
                        IsHidden = true 
                    };
                    acc.Settings = new AccountSettings()
                    {
                        AddressColumnName = "Адрес объекта",
                        AreaColumnName = "Район",
                        MarkColumnName = "Метка",
                        HostColumnName = "URL",
                        PhoneColumnName = "Контакты для связи",
                        ExecuteAfterAnalizeCommand = string.Empty,
                        FolderExportAnalize = string.Empty,
                        FolderExportPhones = string.Empty,
                        FolderImportAnalize = string.Empty,
                        FolderImportMain = string.Empty,
                        IgnoreExportTime = TimeSpan.FromHours(1),
                        DeleteFileAfterImport = false,
                        RecursiveFolderSearch = true,
                        TimeForTrust = TimeSpan.FromDays(30),
                        WaitExecutionAfterAnalize = true
                    };
                    acc.State = new AccountState() 
                    { 
                        IsActive = false 
                    };
                    acc.Dictionary = new AccountDictionary() 
                    {
                        AllowAddToDictionaryAutomatically = true,
                        AllowCalcAreasIfStreetExistsOnly = false,
                        SimilarityForTrust = 0.6m
                    };

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

                    rc.Accounts.Remove(acc);
                    rc.SaveChanges();
                }
            }
            catch(System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var e in ex.EntityValidationErrors)
                    foreach(var e2 in e.ValidationErrors)
                        Console.WriteLine(string.Format("[-] Validation error for {0}. Property '{1}' error: {2}", e.Entry.Entity, e2.PropertyName, e2.ErrorMessage));
            }
            catch(System.Data.DataException ex)
            {
                Exception e = ex;
                while (e != null)
                {
                    Console.WriteLine(string.Format("[-] Exception: {0}", e.Message));

                    if (e is System.Data.Entity.Validation.DbEntityValidationException)
                    {
                        var ex2 = e as System.Data.Entity.Validation.DbEntityValidationException;
                        foreach (var e3 in ex2.EntityValidationErrors)
                            foreach(var e4 in e3.ValidationErrors)
                                Console.WriteLine(string.Format("[-] Validation error for {0}. Property '{1}' error: {2}", e3.Entry.Entity, e4.PropertyName, e4.ErrorMessage));
                    }

                    e = e.InnerException;
                }
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
