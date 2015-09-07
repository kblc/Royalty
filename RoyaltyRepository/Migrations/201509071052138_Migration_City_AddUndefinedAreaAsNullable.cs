namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_City_AddUndefinedAreaAsNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.city", "undefined_area_id", "dbo.area");
            DropIndex("dbo.city", new[] { "undefined_area_id" });
            AlterColumn("dbo.city", "undefined_area_id", c => c.Long());
            CreateIndex("dbo.city", "undefined_area_id");
            AddForeignKey("dbo.city", "undefined_area_id", "dbo.area", "area_id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.city", "undefined_area_id", "dbo.area");
            DropIndex("dbo.city", new[] { "undefined_area_id" });
            AlterColumn("dbo.city", "undefined_area_id", c => c.Long(nullable: false));
            CreateIndex("dbo.city", "undefined_area_id");
            AddForeignKey("dbo.city", "undefined_area_id", "dbo.area", "area_id", cascadeDelete: true);
        }
    }
}
