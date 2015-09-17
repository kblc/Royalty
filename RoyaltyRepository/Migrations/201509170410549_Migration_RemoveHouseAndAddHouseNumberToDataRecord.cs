namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_RemoveHouseAndAddHouseNumberToDataRecord : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.house", "street_id", "dbo.street");
            DropForeignKey("dbo.data", "house_id", "dbo.house");
            DropIndex("dbo.data", new[] { "house_id" });
            DropIndex("dbo.house", "UIX_STREET_HOUSE_NUMBER");
            AddColumn("dbo.data", "street_id", c => c.Long(nullable: false));
            AddColumn("dbo.data", "house_number", c => c.String(maxLength: 20));
            CreateIndex("dbo.data", "street_id");
            AddForeignKey("dbo.data", "street_id", "dbo.street", "street_id", cascadeDelete: true);
            DropColumn("dbo.data", "house_id");
            DropTable("dbo.house");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.house",
                c => new
                    {
                        house_id = c.Long(nullable: false, identity: true),
                        street_id = c.Long(nullable: false),
                        number = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.house_id);
            
            AddColumn("dbo.data", "house_id", c => c.Long(nullable: false));
            DropForeignKey("dbo.data", "street_id", "dbo.street");
            DropIndex("dbo.data", new[] { "street_id" });
            DropColumn("dbo.data", "house_number");
            DropColumn("dbo.data", "street_id");
            CreateIndex("dbo.house", new[] { "street_id", "number" }, unique: true, name: "UIX_STREET_HOUSE_NUMBER");
            CreateIndex("dbo.data", "house_id");
            AddForeignKey("dbo.data", "house_id", "dbo.house", "house_id", cascadeDelete: true);
            AddForeignKey("dbo.house", "street_id", "dbo.street", "street_id", cascadeDelete: true);
        }
    }
}
