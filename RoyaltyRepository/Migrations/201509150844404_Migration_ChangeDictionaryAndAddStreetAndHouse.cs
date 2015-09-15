namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_ChangeDictionaryAndAddStreetAndHouse : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.data", "area_id", "dbo.area");
            DropForeignKey("dbo.dictionary_record", "area_id", "dbo.area");
            DropIndex("dbo.data", new[] { "area_id" });
            DropIndex("dbo.dictionary_record", new[] { "area_id" });
            CreateTable(
                "dbo.house",
                c => new
                    {
                        house_id = c.Long(nullable: false, identity: true),
                        street_id = c.Long(nullable: false),
                        number = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.house_id)
                .ForeignKey("dbo.street", t => t.street_id, cascadeDelete: true)
                .Index(t => new { t.street_id, t.number }, unique: true, name: "UIX_STREET_HOUSE_NUMBER");
            
            CreateTable(
                "dbo.street",
                c => new
                    {
                        street_id = c.Long(nullable: false, identity: true),
                        area_id = c.Long(nullable: false),
                        name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.street_id)
                .ForeignKey("dbo.area", t => t.area_id, cascadeDelete: true)
                .Index(t => new { t.area_id, t.name }, unique: true, name: "UIX_AREA_STREET_NAME");
            
            AddColumn("dbo.data", "house_id", c => c.Long(nullable: false));
            AddColumn("dbo.dictionary_record", "street_id", c => c.Long(nullable: false));
            AddColumn("dbo.dictionary_record", "change_to_street_id", c => c.Long());
            CreateIndex("dbo.data", "house_id");
            CreateIndex("dbo.dictionary_record", "street_id");
            CreateIndex("dbo.dictionary_record", "change_to_street_id");
            AddForeignKey("dbo.data", "house_id", "dbo.house", "house_id", cascadeDelete: true);
            AddForeignKey("dbo.dictionary_record", "change_to_street_id", "dbo.street", "street_id");
            AddForeignKey("dbo.dictionary_record", "street_id", "dbo.street", "street_id", cascadeDelete: true);
            DropColumn("dbo.data", "area_id");
            DropColumn("dbo.data", "address");
            DropColumn("dbo.data", "id_dictionary");
            DropColumn("dbo.dictionary_record", "street");
            DropColumn("dbo.dictionary_record", "area_id");
            DropColumn("dbo.dictionary_record", "rename_street_to");
        }
        
        public override void Down()
        {
            AddColumn("dbo.dictionary_record", "rename_street_to", c => c.String());
            AddColumn("dbo.dictionary_record", "area_id", c => c.Long(nullable: false));
            AddColumn("dbo.dictionary_record", "street", c => c.String(nullable: false));
            AddColumn("dbo.data", "id_dictionary", c => c.Boolean(nullable: false));
            AddColumn("dbo.data", "address", c => c.String(nullable: false));
            AddColumn("dbo.data", "area_id", c => c.Long(nullable: false));
            DropForeignKey("dbo.dictionary_record", "street_id", "dbo.street");
            DropForeignKey("dbo.dictionary_record", "change_to_street_id", "dbo.street");
            DropForeignKey("dbo.data", "house_id", "dbo.house");
            DropForeignKey("dbo.house", "street_id", "dbo.street");
            DropForeignKey("dbo.street", "area_id", "dbo.area");
            DropIndex("dbo.dictionary_record", new[] { "change_to_street_id" });
            DropIndex("dbo.dictionary_record", new[] { "street_id" });
            DropIndex("dbo.street", "UIX_AREA_STREET_NAME");
            DropIndex("dbo.house", "UIX_STREET_HOUSE_NUMBER");
            DropIndex("dbo.data", new[] { "house_id" });
            DropColumn("dbo.dictionary_record", "change_to_street_id");
            DropColumn("dbo.dictionary_record", "street_id");
            DropColumn("dbo.data", "house_id");
            DropTable("dbo.street");
            DropTable("dbo.house");
            CreateIndex("dbo.dictionary_record", "area_id");
            CreateIndex("dbo.data", "area_id");
            AddForeignKey("dbo.dictionary_record", "area_id", "dbo.area", "area_id", cascadeDelete: true);
            AddForeignKey("dbo.data", "area_id", "dbo.area", "area_id", cascadeDelete: true);
        }
    }
}
