namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_City_SetMaxLengthForPhoneNumberCode : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.city", "phone_code", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.city", "phone_code", c => c.String());
        }
    }
}
