namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_Account_AddIsHiddenField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.account", "is_hidden", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.account", "is_hidden");
        }
    }
}
