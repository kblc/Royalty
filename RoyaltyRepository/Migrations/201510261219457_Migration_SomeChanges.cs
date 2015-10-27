namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_SomeChanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.data", new[] { "phone_id" });
            DropIndex("dbo.data", new[] { "street_id" });
            CreateTable(
                "dbo.import_queue_file_data_records",
                c => new
                    {
                        import_queue_file_uid = c.Guid(nullable: false),
                        data_uid = c.Guid(nullable: false),
                        load_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.import_queue_file_uid, t.data_uid })
                .ForeignKey("dbo.data", t => t.data_uid, cascadeDelete: false)
                .ForeignKey("dbo.import_queue_file", t => t.import_queue_file_uid, cascadeDelete: false)
                .Index(t => t.import_queue_file_uid)
                .Index(t => t.data_uid);

            CreateIndex("dbo.data", new[] { "phone_id", "street_id", "house_number" }, unique: true, name: "UX_DATA_ACCOUNT_PHONE_STREET_HOUSE");

            Sql("ALTER TABLE dbo.data_additional DROP CONSTRAINT [FK_dbo.data_additional_dbo.data_data_uid]");
            Sql("ALTER TABLE dbo.data_additional ADD CONSTRAINT [FK_dbo.data_additional_dbo.data_data_uid] FOREIGN KEY (data_uid) REFERENCES dbo.data (data_uid) ON UPDATE NO ACTION ON DELETE CASCADE");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.import_queue_file_data_records", "import_queue_file_uid", "dbo.import_queue_file");
            DropForeignKey("dbo.import_queue_file_data_records", "data_uid", "dbo.data");
            DropIndex("dbo.import_queue_file_data_records", new[] { "data_uid" });
            DropIndex("dbo.import_queue_file_data_records", new[] { "import_queue_file_uid" });
            DropIndex("dbo.data", "UX_DATA_ACCOUNT_PHONE_STREET_HOUSE");
            DropTable("dbo.import_queue_file_data_records");
            CreateIndex("dbo.data", "street_id");
            CreateIndex("dbo.data", "phone_id");
        }
    }
}
