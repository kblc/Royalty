namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_ChangeRelationshipBetweenAreaAndCity : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.city", new[] { "undefined_area_id" });
            AddColumn("dbo.area", "is_default", c => c.Boolean(nullable: false));
            Sql("IF object_id('FK_dbo_city_dbo_area_undefined_area_id', N'F') IS NOT NULL " +
                "ALTER TABLE city DROP CONSTRAINT FK_dbo_city_dbo_area_undefined_area_id");
            DropColumn("dbo.city", "undefined_area_id");
            DropIndex("dbo.area", new[] { "city_id" });
            DropForeignKey("dbo.area", "city_id", "dbo.city");
            AddForeignKey("dbo.area", "city_id", "dbo.city", "city_id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            AddColumn("dbo.city", "undefined_area_id", c => c.Long());
            DropColumn("dbo.area", "is_default");
            CreateIndex("dbo.city", "undefined_area_id");
            Sql("ALTER TABLE city WITH CHECK ADD CONSTRAINT FK_dbo_city_dbo_area_undefined_area_id FOREIGN KEY(undefined_area_id) "+
                "REFERENCES area(area_id) "+
                "ON DELETE SET NULL");
            DropForeignKey("dbo.area", "city_id", "dbo.city");
            //AddForeignKey("dbo.area", "city_id", "dbo.city", "city_id");
        }
    }
}
