namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_RemoveMessage : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MessageFiles", "Message_MessageID", "dbo.message");
            DropForeignKey("dbo.MessageFiles", "File_FileID", "dbo.file");
            DropForeignKey("dbo.import_queue_file", "message_uid", "dbo.message");
            DropIndex("dbo.import_queue_file", new[] { "message_uid" });
            DropIndex("dbo.MessageFiles", new[] { "Message_MessageID" });
            DropIndex("dbo.MessageFiles", new[] { "File_FileID" });
            AddColumn("dbo.import_queue_file", "error", c => c.String());
            DropColumn("dbo.import_queue_file", "message_uid");
            DropTable("dbo.message");
            DropTable("dbo.MessageFiles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MessageFiles",
                c => new
                    {
                        Message_MessageID = c.Guid(nullable: false),
                        File_FileID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Message_MessageID, t.File_FileID });
            
            CreateTable(
                "dbo.message",
                c => new
                    {
                        message_uid = c.Guid(nullable: false),
                        message_text = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.message_uid);
            
            AddColumn("dbo.import_queue_file", "message_uid", c => c.Guid());
            DropColumn("dbo.import_queue_file", "error");
            CreateIndex("dbo.MessageFiles", "File_FileID");
            CreateIndex("dbo.MessageFiles", "Message_MessageID");
            CreateIndex("dbo.import_queue_file", "message_uid");
            AddForeignKey("dbo.import_queue_file", "message_uid", "dbo.message", "message_uid");
            AddForeignKey("dbo.MessageFiles", "File_FileID", "dbo.file", "file_uid", cascadeDelete: true);
            AddForeignKey("dbo.MessageFiles", "Message_MessageID", "dbo.message", "message_uid", cascadeDelete: true);
        }
    }
}
