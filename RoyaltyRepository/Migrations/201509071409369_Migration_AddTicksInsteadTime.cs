namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddTicksInsteadTime : DbMigration
    {
        public override void Up()
        {
            //DropColumn("account_settings", "time_for_trust");
            //AddColumn("account_settings", "time_for_trust_ticks", c => c.Long());

            //DropColumn("account_settings", "ignore_export");
            //AddColumn("account_settings", "ignore_export_ticks", c => c.Long());

            //DropColumn("shedule_time", "time");
            //AddColumn("shedule_time", "time_ticks", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            //AddColumn("account_settings", "time_for_trust", c => c.Time(precision: 7));
            //DropColumn("account_settings", "time_for_trust_ticks");

            //AddColumn("account_settings", "ignore_export", c => c.Time(precision: 7));
            //DropColumn("account_settings", "ignore_export_ticks");

            //AddColumn("shedule_time", "time", c => c.Time(nullable: false, precision: 7));
            //DropColumn("shedule_time", "time_ticks");
        }
    }
}
