namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_RecordstateChangeMaxLengthTo401 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.import_queue", "has_error", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.import_queue", "has_error");
        }
    }
}
