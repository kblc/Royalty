namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<RoyaltyRepository.Models.RepositoryContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(RoyaltyRepository.Models.RepositoryContext context)
        {
            foreach (var t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces().Any(i => i.Name == "IDefaultRepositoryInitialization")))
            {
                var ci = t.GetConstructor(new Type[] { });
                if (ci != null)
                {
                    var el = ci.Invoke(new object[] { }) as RoyaltyRepository.Models.IDefaultRepositoryInitialization;
                    if (el != null)
                        el.InitializeDefault(context);
                }
            }
        }
    }
}
