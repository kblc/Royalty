namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddSourceFilePathToQueueFileRecord : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.import_queue_file", "source_file_path", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.import_queue_file", "source_file_path");
        }
    }
}
