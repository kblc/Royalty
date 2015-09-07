namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_Account : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.account",
                c => new
                    {
                        account_uid = c.Guid(nullable: false),
                        name = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.account_uid)
                .Index(t => t.name, unique: true, name: "UIX_ACCOUNT_NAME");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.account", "UIX_ACCOUNT_NAME");
            DropTable("dbo.account");
        }
    }
}
