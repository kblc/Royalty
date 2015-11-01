namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddExportForEveryDataRecord : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.data_export",
                c => new
                    {
                        data_export_id = c.Long(nullable: false, identity: true),
                        data_uid = c.Guid(nullable: false),
                        export_date = c.DateTime(nullable: false),
                        host_id = c.Long(nullable: false),
                        file_uid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.data_export_id)
                .ForeignKey("dbo.data", t => t.data_uid, cascadeDelete: false)
                .ForeignKey("dbo.file", t => t.file_uid, cascadeDelete: false)
                //.ForeignKey("dbo.host", t => t.host_id, cascadeDelete: false)
                .Index(t => t.data_uid)
                .Index(t => t.host_id)
                .Index(t => t.file_uid);

            Sql("ALTER TABLE [dbo].[data_export] ADD CONSTRAINT [FK_dbo.data_export_dbo.host_host_id] FOREIGN KEY ([host_id]) REFERENCES [dbo].[host] ([host_id]) ON DELETE NO ACTION");
            //ALTER TABLE[dbo].[data_export] ADD CONSTRAINT[FK_dbo.data_export_dbo.data_data_uid] FOREIGN KEY([data_uid]) REFERENCES[dbo].[data] ([data_uid]) ON DELETE CASCADE
            //ALTER TABLE[dbo].[data_export] ADD CONSTRAINT[FK_dbo.data_export_dbo.file_file_uid] FOREIGN KEY([file_uid]) REFERENCES[dbo].[file] ([file_uid]) ON DELETE CASCADE
    }
        
        public override void Down()
        {
            DropForeignKey("dbo.data_export", "host_id", "dbo.host");
            DropForeignKey("dbo.data_export", "file_uid", "dbo.file");
            DropForeignKey("dbo.data_export", "data_uid", "dbo.data");
            DropIndex("dbo.data_export", new[] { "file_uid" });
            DropIndex("dbo.data_export", new[] { "host_id" });
            DropIndex("dbo.data_export", new[] { "data_uid" });
            DropTable("dbo.data_export");
        }
    }
}
