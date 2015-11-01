namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddExportForColumnType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.column_type", "export_column_index", c => c.Long(nullable: false, defaultValue: 0));
            AddColumn("dbo.column_type", "export", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.column_type", "export");
            DropColumn("dbo.column_type", "export_column_index");
        }
    }
}
