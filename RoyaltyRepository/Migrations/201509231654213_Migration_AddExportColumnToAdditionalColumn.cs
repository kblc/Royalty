namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddExportColumnToAdditionalColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.data_additional_column", "export", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.data_additional_column", "export");
        }
    }
}
