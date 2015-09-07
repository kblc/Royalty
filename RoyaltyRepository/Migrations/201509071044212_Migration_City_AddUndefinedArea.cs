namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_City_AddUndefinedArea : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.dictionary_record", "area_id", c => c.Long(nullable: false));
            AddColumn("dbo.city", "undefined_area_id", c => c.Long(nullable: false));
            CreateIndex("dbo.dictionary_record", "area_id");
            CreateIndex("dbo.city", "undefined_area_id");
            AddForeignKey("dbo.city", "undefined_area_id", "dbo.area", "area_id", cascadeDelete: false);
            AddForeignKey("dbo.dictionary_record", "area_id", "dbo.area", "area_id", cascadeDelete: true);
            DropColumn("dbo.dictionary_record", "city");
            DropColumn("dbo.dictionary_record", "area");
        }
        
        public override void Down()
        {
            AddColumn("dbo.dictionary_record", "area", c => c.String());
            AddColumn("dbo.dictionary_record", "city", c => c.String(nullable: false));
            DropForeignKey("dbo.dictionary_record", "area_id", "dbo.area");
            DropForeignKey("dbo.city", "undefined_area_id", "dbo.area");
            DropIndex("dbo.city", new[] { "undefined_area_id" });
            DropIndex("dbo.dictionary_record", new[] { "area_id" });
            DropColumn("dbo.city", "undefined_area_id");
            DropColumn("dbo.dictionary_record", "area_id");
        }
    }
}
