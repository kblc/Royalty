namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_FilesAddMessages : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.file", "Message_MessageID", "dbo.message");
            DropIndex("dbo.file", new[] { "Message_MessageID" });
            CreateTable(
                "dbo.MessageFiles",
                c => new
                    {
                        Message_MessageID = c.Guid(nullable: false),
                        File_FileID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Message_MessageID, t.File_FileID })
                .ForeignKey("dbo.message", t => t.Message_MessageID, cascadeDelete: true)
                .ForeignKey("dbo.file", t => t.File_FileID, cascadeDelete: true)
                .Index(t => t.Message_MessageID)
                .Index(t => t.File_FileID);
            
            DropColumn("dbo.file", "Message_MessageID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.file", "Message_MessageID", c => c.Guid());
            DropForeignKey("dbo.MessageFiles", "File_FileID", "dbo.file");
            DropForeignKey("dbo.MessageFiles", "Message_MessageID", "dbo.message");
            DropIndex("dbo.MessageFiles", new[] { "File_FileID" });
            DropIndex("dbo.MessageFiles", new[] { "Message_MessageID" });
            DropTable("dbo.MessageFiles");
            CreateIndex("dbo.file", "Message_MessageID");
            AddForeignKey("dbo.file", "Message_MessageID", "dbo.message", "message_uid");
        }
    }
}
