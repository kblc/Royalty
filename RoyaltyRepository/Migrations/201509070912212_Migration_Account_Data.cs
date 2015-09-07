namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_Account_Data : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.data_additional_column",
                c => new
                    {
                        data_additional_column_id = c.Long(nullable: false, identity: true),
                        account_uid = c.Guid(nullable: false),
                        system_name = c.String(nullable: false, maxLength: 250),
                        name = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.data_additional_column_id)
                .ForeignKey("dbo.account", t => t.account_uid, cascadeDelete: true)
                .Index(t => new { t.account_uid, t.system_name }, unique: true, name: "UIX_DATA_ADDITIONAL_COLUMN_ACCOUNT_SYSTEMNAME");
            
            CreateTable(
                "dbo.data_additional",
                c => new
                    {
                        data_uid = c.Guid(nullable: false),
                        column00 = c.String(),
                        column01 = c.String(),
                        column02 = c.String(),
                        column03 = c.String(),
                        column04 = c.String(),
                        column05 = c.String(),
                        column06 = c.String(),
                        column07 = c.String(),
                        column08 = c.String(),
                        column09 = c.String(),
                        column10 = c.String(),
                        column11 = c.String(),
                        column12 = c.String(),
                        column13 = c.String(),
                        column14 = c.String(),
                        column15 = c.String(),
                        column16 = c.String(),
                        column17 = c.String(),
                        column18 = c.String(),
                        column19 = c.String(),
                    })
                .PrimaryKey(t => t.data_uid)
                .ForeignKey("dbo.data", t => t.data_uid)
                .Index(t => t.data_uid);
            
            CreateTable(
                "dbo.data",
                c => new
                    {
                        data_uid = c.Guid(nullable: false, identity: true),
                        account_uid = c.Guid(nullable: false),
                        phone_id = c.Long(nullable: false),
                        area_id = c.Long(nullable: false),
                        host_id = c.Long(nullable: false),
                        address = c.String(nullable: false),
                        created = c.DateTime(nullable: false),
                        changed = c.DateTime(nullable: false),
                        exported = c.DateTime(),
                        id_dictionary = c.Boolean(nullable: false),
                        mark_id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.data_uid)
                .ForeignKey("dbo.account", t => t.account_uid, cascadeDelete: true)
                .ForeignKey("dbo.area", t => t.area_id, cascadeDelete: true)
                .ForeignKey("dbo.host", t => t.host_id, cascadeDelete: true)
                .ForeignKey("dbo.mark", t => t.mark_id, cascadeDelete: true)
                .ForeignKey("dbo.phone", t => t.phone_id, cascadeDelete: true)
                .Index(t => t.account_uid, name: "IX_DATA_ACCOUNT_UID")
                .Index(t => t.phone_id)
                .Index(t => t.area_id)
                .Index(t => t.host_id)
                .Index(t => t.mark_id);
            
            CreateTable(
                "dbo.area",
                c => new
                    {
                        area_id = c.Long(nullable: false, identity: true),
                        city_id = c.Long(nullable: false),
                        name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.area_id)
                .ForeignKey("dbo.city", t => t.city_id, cascadeDelete: true)
                .Index(t => new { t.city_id, t.name }, unique: true, name: "UIX_AREA_NAME");
            
            CreateTable(
                "dbo.city",
                c => new
                    {
                        city_id = c.Long(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.city_id)
                .Index(t => t.name, unique: true, name: "UIX_CITY_NAME");
            
            CreateTable(
                "dbo.host",
                c => new
                    {
                        host_id = c.Long(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.host_id)
                .Index(t => t.name, unique: true, name: "UIX_HOST_NAME");
            
            CreateTable(
                "dbo.mark",
                c => new
                    {
                        mark_id = c.Long(nullable: false, identity: true),
                        system_name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.mark_id)
                .Index(t => t.system_name, unique: true, name: "UIX_MARK_SYSTEMNAME");
            
            CreateTable(
                "dbo.phone",
                c => new
                    {
                        phone_id = c.Long(nullable: false, identity: true),
                        phone_number = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.phone_id)
                .Index(t => t.phone_number, unique: true, name: "UIX_PHONE_NUMBER");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.data_additional", "data_uid", "dbo.data");
            DropForeignKey("dbo.data", "phone_id", "dbo.phone");
            DropForeignKey("dbo.data", "mark_id", "dbo.mark");
            DropForeignKey("dbo.data", "host_id", "dbo.host");
            DropForeignKey("dbo.data", "area_id", "dbo.area");
            DropForeignKey("dbo.area", "city_id", "dbo.city");
            DropForeignKey("dbo.data", "account_uid", "dbo.account");
            DropForeignKey("dbo.data_additional_column", "account_uid", "dbo.account");
            DropIndex("dbo.phone", "UIX_PHONE_NUMBER");
            DropIndex("dbo.mark", "UIX_MARK_SYSTEMNAME");
            DropIndex("dbo.host", "UIX_HOST_NAME");
            DropIndex("dbo.city", "UIX_CITY_NAME");
            DropIndex("dbo.area", "UIX_AREA_NAME");
            DropIndex("dbo.data", new[] { "mark_id" });
            DropIndex("dbo.data", new[] { "host_id" });
            DropIndex("dbo.data", new[] { "area_id" });
            DropIndex("dbo.data", new[] { "phone_id" });
            DropIndex("dbo.data", "IX_DATA_ACCOUNT_UID");
            DropIndex("dbo.data_additional", new[] { "data_uid" });
            DropIndex("dbo.data_additional_column", "UIX_DATA_ADDITIONAL_COLUMN_ACCOUNT_SYSTEMNAME");
            DropTable("dbo.phone");
            DropTable("dbo.mark");
            DropTable("dbo.host");
            DropTable("dbo.city");
            DropTable("dbo.area");
            DropTable("dbo.data");
            DropTable("dbo.data_additional");
            DropTable("dbo.data_additional_column");
        }
    }
}
