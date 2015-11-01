namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_SmallChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.export_type", "account_uid", "dbo.account");
            DropForeignKey("dbo.export_type", "mark_id", "dbo.mark");
            DropIndex("dbo.export_type", "IX_DATA_ACCOUNT_UID");
            DropIndex("dbo.export_type", new[] { "mark_id" });
            AddColumn("dbo.export_directory", "file_name", c => c.String());
            AddColumn("dbo.export_directory", "mark_id", c => c.Long(nullable: false));
            CreateIndex("dbo.export_directory", "mark_id");
            AddForeignKey("dbo.export_directory", "mark_id", "dbo.mark", "mark_id", cascadeDelete: true);
            DropColumn("dbo.export_directory", "export_data");
            DropColumn("dbo.export_directory", "export_phones");
            DropTable("dbo.export_type");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.export_type",
                c => new
                    {
                        export_type_id = c.Long(nullable: false, identity: true),
                        account_uid = c.Guid(nullable: false),
                        mark_id = c.Long(nullable: false),
                        filename = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.export_type_id);
            
            AddColumn("dbo.export_directory", "export_phones", c => c.Boolean(nullable: false));
            AddColumn("dbo.export_directory", "export_data", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.export_directory", "mark_id", "dbo.mark");
            DropIndex("dbo.export_directory", new[] { "mark_id" });
            DropColumn("dbo.export_directory", "mark_id");
            DropColumn("dbo.export_directory", "file_name");
            CreateIndex("dbo.export_type", "mark_id");
            CreateIndex("dbo.export_type", "account_uid", name: "IX_DATA_ACCOUNT_UID");
            AddForeignKey("dbo.export_type", "mark_id", "dbo.mark", "mark_id", cascadeDelete: true);
            AddForeignKey("dbo.export_type", "account_uid", "dbo.account", "account_uid", cascadeDelete: true);
        }
    }
}
