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
            var t = new RoyaltyDataCalculatorTest.DataCalculatorTest();
            try
            {
                t.Initialization();
                t.DataCalculator_Preview();
            }
            catch(Exception ex)
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
            finally
            {
                t.Finalization();
            }
            Console.WriteLine("[~] Test end");
            Console.ReadKey();
        }
    }
}
