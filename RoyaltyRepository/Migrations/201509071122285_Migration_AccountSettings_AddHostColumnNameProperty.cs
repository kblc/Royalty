namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AccountSettings_AddHostColumnNameProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.account_settings", "host_column_name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.account_settings", "host_column_name");
        }
    }
}
