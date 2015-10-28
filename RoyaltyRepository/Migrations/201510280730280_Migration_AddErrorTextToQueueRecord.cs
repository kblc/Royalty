namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddErrorTextToQueueRecord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.import_queue", "record_error", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.import_queue", "record_error");
        }
    }
}
