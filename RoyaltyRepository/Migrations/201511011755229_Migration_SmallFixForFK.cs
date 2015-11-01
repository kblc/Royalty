namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_SmallFixForFK : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE dbo.import_queue_file_data_records DROP CONSTRAINT [FK_dbo.import_queue_file_data_records_dbo.data_data_uid]");
            Sql("ALTER TABLE dbo.import_queue_file_data_records ADD CONSTRAINT "+
                "[FK_dbo.import_queue_file_data_records_dbo.data_data_uid] FOREIGN KEY(data_uid) REFERENCES dbo.data(data_uid) ON DELETE CASCADE");

            Sql("ALTER TABLE dbo.data_export DROP CONSTRAINT [FK_dbo.data_export_dbo.data_data_uid]");
            Sql("ALTER TABLE dbo.data_export ADD CONSTRAINT [FK_dbo.data_export_dbo.data_data_uid] " +
                "FOREIGN KEY (data_uid) REFERENCES dbo.data (data_uid) ON DELETE CASCADE");
        }
        
        public override void Down()
        {
            Sql("ALTER TABLE dbo.import_queue_file_data_records DROP CONSTRAINT [FK_dbo.import_queue_file_data_records_dbo.data_data_uid]");
            Sql("ALTER TABLE dbo.import_queue_file_data_records ADD CONSTRAINT " +
                "[FK_dbo.import_queue_file_data_records_dbo.data_data_uid] FOREIGN KEY(data_uid) REFERENCES dbo.data(data_uid) ON DELETE NO ACTION");

            Sql("ALTER TABLE dbo.data_export DROP CONSTRAINT [FK_dbo.data_export_dbo.data_data_uid]");
            Sql("ALTER TABLE dbo.data_export ADD CONSTRAINT [FK_dbo.data_export_dbo.data_data_uid] " +
                "FOREIGN KEY (data_uid) REFERENCES dbo.data (data_uid) ON DELETE NO ACTION");
        }
    }
}
