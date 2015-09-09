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
                using (var rc = new RoyaltyRepository.Repository("connectionString"))
                {
                    rc.Log = (s) => { Console.WriteLine(string.Format("[~] SQL: {0}", s)); };

                    var a2 = rc.AccountGet("default2", true);
                    var a3 = rc.AccountGet("default3", true);
                    if (a2 != null)
                        rc.AccountRemove(a2);
                    if (a3 != null)
                        rc.AccountRemove(a3);

                    var acc = rc.AccountNew(byDefault: true);
                    acc.Name = "default3";
                    rc.AccountAdd(acc);
                    try
                    { 
                        rc.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(09, 00, 00));
                        rc.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(12, 00, 00));
                        rc.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(18, 00, 00));
                        var st = rc.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(20, 00, 00));
                        rc.SaveChanges();
                        rc.AccountSettingsSheduleTimeRemove(st);

                        if (acc.Settings.SheduleTimes.Count() != 3)
                            throw new Exception("Shedule time error");
                    }
                    finally
                    { 
                        rc.AccountRemove(acc);
                    }
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
