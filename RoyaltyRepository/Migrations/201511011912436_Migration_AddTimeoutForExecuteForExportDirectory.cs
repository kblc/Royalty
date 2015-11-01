namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddTimeoutForExecuteForExportDirectory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.export_directory", "timeout_for_execute", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.export_directory", "wait_execution_after_analize");
        }
        
        public override void Down()
        {
            AddColumn("dbo.export_directory", "wait_execution_after_analize", c => c.Boolean());
            DropColumn("dbo.export_directory", "timeout_for_execute");
        }
    }
}
