namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_Area_City_Relationship_Change : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.area", "city_id", "dbo.city");
            AddForeignKey("dbo.area", "city_id", "dbo.city", "city_id");

            DropForeignKey("dbo.city", "undefined_area_id", "dbo.area");
            Sql("ALTER TABLE city " + 
                "ADD CONSTRAINT FK_dbo_city_dbo_area_undefined_area_id " +
                "FOREIGN KEY(undefined_area_id) "+
                "REFERENCES area (area_id) "+
                "ON DELETE SET NULL");
        }
        
        public override void Down()
        {
            Sql("ALTER TABLE city DROP CONSTRAINT FK_dbo_city_dbo_area_undefined_area_id");
            AddForeignKey("dbo.city", "undefined_area_id", "dbo.area", "area_id");

            DropForeignKey("dbo.area", "city_id", "dbo.city");
            AddForeignKey("dbo.area", "city_id", "dbo.city", "city_id", cascadeDelete: true);
        }
    }
}
