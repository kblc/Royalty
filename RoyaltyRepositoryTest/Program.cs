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
                using (var rc = new RoyaltyRepository.Repository("connectionStringHome"))
                {
                    rc.Log = (s) => { Console.WriteLine(string.Format("[~] SQL: {0}", s)); };

                    rc.AccountRemove(rc.AccountGet("default0", true));
                    //rc.AccountRemove(rc.AccountGet("default1", true));
                    //rc.AccountRemove(rc.AccountGet("default2", true));

                    foreach (var m in rc.MarkGet())
                        Console.WriteLine(string.Format("[~] mark found: {0}", m.ToString()));

                    #region Account
                    var acc = rc.AccountNew(byDefault: true, accountName: "default0");
                    rc.AccountAdd(acc);
                    try
                    {
                        acc.State.IsActive = true;
                        acc.State.LastBatch = DateTime.Now;
                        rc.SaveChanges();

                        rc.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(09, 00, 00));
                        rc.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(12, 00, 00));
                        rc.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(18, 00, 00));
                        var st = rc.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(20, 00, 00));
                        rc.SaveChanges();
                        rc.AccountSettingsSheduleTimeRemove(st);

                        if (acc.Settings.SheduleTimes.Count() != 3)
                            throw new Exception("Shedule time error");

                        rc.AccountSeriesOfNumbersRecordNew(acc, TimeSpan.FromDays(600), 5);
                        rc.AccountSeriesOfNumbersRecordNew(acc, TimeSpan.FromDays(300), 4);
                        rc.SaveChanges();
                        var son = acc.SeriesOfNumbers.FirstOrDefault();
                        rc.AccountSeriesOfNumbersRecordRemove(son);

                        if (acc.SeriesOfNumbers.Count() != 1)
                            throw new Exception("Series of numbers error");
                    }
                    finally
                    {
                        rc.AccountRemove(acc);
                    }
                    #endregion
                    #region Host

                    var hCnt = rc.HostGet().Count();
                    var h = rc.HostNew("test0.host.com");
                    rc.HostAdd(h);

                    if (rc.HostGet().Count() != hCnt + 1)
                        throw new Exception("Host error");

                    rc.HostRemove(h);

                    if (rc.HostGet().Count() != hCnt)
                        throw new Exception("Host error");

                    #endregion
                    #region Phone

                    var hPhn = rc.PhoneGet().Count();
                    var p = rc.PhoneNew("08-000-000-0000");
                    rc.PhoneAdd(p);

                    if (rc.PhoneGet().Count() != hPhn + 1)
                        throw new Exception("Phone error");

                    rc.PhoneRemove(p);

                    if (rc.PhoneGet().Count() != hPhn)
                        throw new Exception("Phone error");
                    #endregion
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
