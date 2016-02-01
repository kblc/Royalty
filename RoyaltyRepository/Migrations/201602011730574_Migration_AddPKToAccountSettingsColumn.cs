namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddPKToAccountSettingsColumn : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.account_settings_column", new[] { "account_uid" });
            DropIndex("dbo.account_settings_column", new[] { "column_type_id" });
            DropPrimaryKey("dbo.account_settings_column");
            AddColumn("dbo.account_settings_column", "account_settings_column_id", c => c.Long(nullable: false, identity: true));
            AddPrimaryKey("dbo.account_settings_column", "account_settings_column_id");
            CreateIndex("dbo.account_settings_column", new[] { "account_uid", "column_type_id" }, unique: true, name: "UIX_ACCOUNT_SETTINGS_COLUMN");
        }
        
        public override void Down()
        {
            DropIndex("dbo.account_settings_column", "UIX_ACCOUNT_SETTINGS_COLUMN");
            DropPrimaryKey("dbo.account_settings_column");
            DropColumn("dbo.account_settings_column", "account_settings_column_id");
            AddPrimaryKey("dbo.account_settings_column", new[] { "account_uid", "column_type_id" });
            CreateIndex("dbo.account_settings_column", "column_type_id");
            CreateIndex("dbo.account_settings_column", "account_uid");
        }
    }
}
