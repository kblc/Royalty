namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddDateToHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.history", "date", c => c.DateTime(nullable: false, defaultValue: DateTime.UtcNow ));
        }
        
        public override void Down()
        {
            DropColumn("dbo.history", "date");
        }
    }
}
