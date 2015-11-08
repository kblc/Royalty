namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_RemoveAccountState : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.account_state", "account_uid", "dbo.account");
            DropIndex("dbo.account_state", new[] { "account_uid" });
            AddColumn("dbo.account", "is_active", c => c.Boolean(nullable: false));
            DropTable("dbo.account_state");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.account_state",
                c => new
                    {
                        account_uid = c.Guid(nullable: false),
                        last_batch = c.DateTime(),
                        is_active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.account_uid);
            
            DropColumn("dbo.account", "is_active");
            CreateIndex("dbo.account_state", "account_uid");
            AddForeignKey("dbo.account_state", "account_uid", "dbo.account", "account_uid");
        }
    }
}
