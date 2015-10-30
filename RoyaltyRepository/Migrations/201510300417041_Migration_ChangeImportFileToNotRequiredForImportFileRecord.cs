namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_ChangeImportFileToNotRequiredForImportFileRecord : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.import_queue_file", "import_file_uid", "dbo.file");
            DropIndex("dbo.import_queue_file", new[] { "import_file_uid" });
            AlterColumn("dbo.import_queue_file", "import_file_uid", c => c.Guid());
            CreateIndex("dbo.import_queue_file", "import_file_uid");
            AddForeignKey("dbo.import_queue_file", "import_file_uid", "dbo.file", "file_uid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.import_queue_file", "import_file_uid", "dbo.file");
            DropIndex("dbo.import_queue_file", new[] { "import_file_uid" });
            AlterColumn("dbo.import_queue_file", "import_file_uid", c => c.Guid(nullable: false));
            CreateIndex("dbo.import_queue_file", "import_file_uid");
            AddForeignKey("dbo.import_queue_file", "import_file_uid", "dbo.file", "file_uid", cascadeDelete: true);
        }
    }
}
