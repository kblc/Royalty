namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AccountPhoneMarkChangeIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.account_phone_mark", new[] { "account_uid" });
            DropIndex("dbo.account_phone_mark", new[] { "phone_id" });
            CreateIndex("dbo.account_phone_mark", new[] { "account_uid", "phone_id" }, unique: true, name: "UIX_Account_Phone");
        }
        
        public override void Down()
        {
            DropIndex("dbo.account_phone_mark", "UIX_Account_Phone");
            CreateIndex("dbo.account_phone_mark", "phone_id", unique: true);
            CreateIndex("dbo.account_phone_mark", "account_uid", unique: true);
        }
    }
}
