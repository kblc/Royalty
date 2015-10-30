namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_SmallChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.import_queue_file", "import_file_uid", "dbo.file");
            DropForeignKey("dbo.import_queue_file", "log_file_uid", "dbo.file");
            DropIndex("dbo.import_queue_file", new[] { "import_file_uid" });
            DropIndex("dbo.import_queue_file", new[] { "log_file_uid" });
            CreateTable(
                "dbo.import_queue_file_file",
                c => new
                    {
                        import_queue_file_file_id = c.Long(nullable: false, identity: true),
                        import_queue_file_uid = c.Guid(nullable: false),
                        type = c.String(nullable: false, maxLength: 100),
                        file_uid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.import_queue_file_file_id)
                .ForeignKey("dbo.file", t => t.file_uid, cascadeDelete: true)
                .ForeignKey("dbo.import_queue_file", t => t.import_queue_file_uid, cascadeDelete: true)
                .Index(t => t.import_queue_file_uid)
                .Index(t => t.file_uid);
            
            DropColumn("dbo.import_queue_file", "import_file_uid");
            DropColumn("dbo.import_queue_file", "log_file_uid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.import_queue_file", "log_file_uid", c => c.Guid());
            AddColumn("dbo.import_queue_file", "import_file_uid", c => c.Guid());
            DropForeignKey("dbo.import_queue_file_file", "import_queue_file_uid", "dbo.import_queue_file");
            DropForeignKey("dbo.import_queue_file_file", "file_uid", "dbo.file");
            DropIndex("dbo.import_queue_file_file", new[] { "file_uid" });
            DropIndex("dbo.import_queue_file_file", new[] { "import_queue_file_uid" });
            DropTable("dbo.import_queue_file_file");
            CreateIndex("dbo.import_queue_file", "log_file_uid");
            CreateIndex("dbo.import_queue_file", "import_file_uid");
            AddForeignKey("dbo.import_queue_file", "log_file_uid", "dbo.file", "file_uid");
            AddForeignKey("dbo.import_queue_file", "import_file_uid", "dbo.file", "file_uid");
        }
    }
}
