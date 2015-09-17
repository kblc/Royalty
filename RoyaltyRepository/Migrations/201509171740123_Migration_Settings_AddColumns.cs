namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_Settings_AddColumns : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.account_settings_column",
                c => new
                    {
                        account_uid = c.Guid(nullable: false),
                        column_type_id = c.Long(nullable: false),
                        column_name = c.String(nullable: false),
                    })
                .PrimaryKey(t => new { t.account_uid, t.column_type_id })
                .ForeignKey("dbo.account_settings", t => t.account_uid, cascadeDelete: true)
                .ForeignKey("dbo.column_type", t => t.column_type_id, cascadeDelete: true)
                .Index(t => t.account_uid)
                .Index(t => t.column_type_id);
            
            CreateTable(
                "dbo.column_type",
                c => new
                    {
                        column_type_id = c.Long(nullable: false, identity: true),
                        system_name = c.String(nullable: false, maxLength: 100),
                        import_table_validation = c.Boolean(nullable: false),
                        import_row_validation = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.column_type_id)
                .Index(t => t.system_name, unique: true, name: "UIX_COLUMNTYPE_SYSTEMNAME");
            
            DropColumn("dbo.account_settings", "phones_column_name");
            DropColumn("dbo.account_settings", "address_column_name");
            DropColumn("dbo.account_settings", "area_column_name");
            DropColumn("dbo.account_settings", "mark_column_name");
            DropColumn("dbo.account_settings", "host_column_name");
            DropColumn("dbo.account_settings", "city_column_name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.account_settings", "city_column_name", c => c.String(nullable: false));
            AddColumn("dbo.account_settings", "host_column_name", c => c.String(nullable: false));
            AddColumn("dbo.account_settings", "mark_column_name", c => c.String(nullable: false));
            AddColumn("dbo.account_settings", "area_column_name", c => c.String(nullable: false));
            AddColumn("dbo.account_settings", "address_column_name", c => c.String(nullable: false));
            AddColumn("dbo.account_settings", "phones_column_name", c => c.String(nullable: false));
            DropForeignKey("dbo.account_settings_column", "column_type_id", "dbo.column_type");
            DropForeignKey("dbo.account_settings_column", "account_uid", "dbo.account_settings");
            DropIndex("dbo.column_type", "UIX_COLUMNTYPE_SYSTEMNAME");
            DropIndex("dbo.account_settings_column", new[] { "column_type_id" });
            DropIndex("dbo.account_settings_column", new[] { "account_uid" });
            DropTable("dbo.column_type");
            DropTable("dbo.account_settings_column");
        }
    }
}
