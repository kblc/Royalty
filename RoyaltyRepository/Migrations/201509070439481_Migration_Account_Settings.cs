namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_Account_Settings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.account_settings",
                c => new
                    {
                        account_uid = c.Guid(nullable: false),
                        folder_import_main = c.String(nullable: false),
                        folder_import_analize = c.String(nullable: false),
                        folder_export_analize = c.String(nullable: false),
                        folder_export_phones = c.String(nullable: false),
                        ignore_export = c.Time(nullable: false, precision: 7),
                        time_for_trust = c.Time(nullable: false, precision: 7),
                        delete_file_after_import = c.Boolean(),
                        execute_after_analize_command = c.String(),
                        wait_execution_after_analize = c.Boolean(),
                        recursive_folder_search_for_csv_files = c.Boolean(),
                        phones_column_name = c.String(nullable: false),
                        address_column_name = c.String(nullable: false),
                        area_column_name = c.String(nullable: false),
                        mark_column_name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.account_uid)
                .ForeignKey("dbo.account", t => t.account_uid)
                .Index(t => t.account_uid);
            
            CreateTable(
                "dbo.shedule_time",
                c => new
                    {
                        shedule_time_id = c.Long(nullable: false, identity: true),
                        account_uid = c.Guid(nullable: false),
                        time = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.shedule_time_id)
                .ForeignKey("dbo.account_settings", t => t.account_uid, cascadeDelete: true)
                .Index(t => t.account_uid, name: "IX_ACCOUNT_SETTINGS_TIMER_ACCOUNT_UID");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.shedule_time", "account_uid", "dbo.account_settings");
            DropForeignKey("dbo.account_settings", "account_uid", "dbo.account");
            DropIndex("dbo.shedule_time", "IX_ACCOUNT_SETTINGS_TIMER_ACCOUNT_UID");
            DropIndex("dbo.account_settings", new[] { "account_uid" });
            DropTable("dbo.shedule_time");
            DropTable("dbo.account_settings");
        }
    }
}
