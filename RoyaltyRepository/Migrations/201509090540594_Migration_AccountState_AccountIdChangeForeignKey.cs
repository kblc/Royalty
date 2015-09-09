namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AccountState_AccountIdChangeForeignKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.account_state", "account_uid", "dbo.account");
            AddForeignKey("dbo.account_state", "account_uid", "dbo.account", cascadeDelete: true);

            DropForeignKey("dbo.account_settings", "account_uid", "dbo.account");
            AddForeignKey("dbo.account_settings", "account_uid", "dbo.account", cascadeDelete: true);

            DropForeignKey("dbo.dictionary", "account_uid", "dbo.account");
            AddForeignKey("dbo.dictionary", "account_uid", "dbo.account", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.account_state", "account_uid", "dbo.account");
            AddForeignKey("dbo.account_state", "account_uid", "dbo.account");

            DropForeignKey("dbo.account_settings", "account_uid", "dbo.account");
            AddForeignKey("dbo.account_settings", "account_uid", "dbo.account");

            DropForeignKey("dbo.dictionary", "account_uid", "dbo.account");
            AddForeignKey("dbo.dictionary", "account_uid", "dbo.account");
        }
    }
}
