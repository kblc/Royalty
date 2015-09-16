namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_City_AddPhoneNumberCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.city", "phone_code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.city", "phone_code");
        }
    }
}
