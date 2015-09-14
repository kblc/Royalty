namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_RecordstateChangeMaxLengthTo40 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.import_queue_record_state", "UIX_IMPORT_QUEUE_RECORD_STATE_SYSTEMNAME");
            AlterColumn("dbo.import_queue_record_state", "system_name", c => c.String(nullable: false, maxLength: 40));
            CreateIndex("dbo.import_queue_record_state", "system_name", unique: true, name: "UIX_IMPORT_QUEUE_RECORD_STATE_SYSTEMNAME");
        }
        
        public override void Down()
        {
            DropIndex("dbo.import_queue_record_state", "UIX_IMPORT_QUEUE_RECORD_STATE_SYSTEMNAME");
            AlterColumn("dbo.import_queue_record_state", "system_name", c => c.String(nullable: false, maxLength: 20));
            CreateIndex("dbo.import_queue_record_state", "system_name", unique: true, name: "UIX_IMPORT_QUEUE_RECORD_STATE_SYSTEMNAME");
        }
    }
}
