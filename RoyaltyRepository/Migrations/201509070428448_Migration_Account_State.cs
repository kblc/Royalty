namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_Account_State : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.account_state",
                c => new
                    {
                        account_uid = c.Guid(nullable: false),
                        last_batch = c.DateTime(),
                        is_active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.account_uid)
                .ForeignKey("dbo.account", t => t.account_uid)
                .Index(t => t.account_uid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.account_state", "account_uid", "dbo.account");
            DropIndex("dbo.account_state", new[] { "account_uid" });
            DropTable("dbo.account_state");
        }
    }
}
