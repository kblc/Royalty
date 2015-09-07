namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_Account_Dictionary : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.dictionary",
                c => new
                    {
                        account_uid = c.Guid(nullable: false),
                        similarity_for_trust = c.Decimal(nullable: false, precision: 18, scale: 2),
                        allow_add_to_dictionary_automatically = c.Boolean(nullable: false),
                        allow_calc_areas_if_street_exists_only = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.account_uid)
                .ForeignKey("dbo.account", t => t.account_uid)
                .Index(t => t.account_uid);
            
            CreateTable(
                "dbo.dictionary_exclude",
                c => new
                    {
                        dictionary_exclude_id = c.Long(nullable: false, identity: true),
                        dictionary_uid = c.Guid(nullable: false),
                        exclude = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.dictionary_exclude_id)
                .ForeignKey("dbo.dictionary", t => t.dictionary_uid, cascadeDelete: true)
                .Index(t => t.dictionary_uid, name: "IX_DICTIONARY_EXCLUDE_DICTIONARY_UID");
            
            CreateTable(
                "dbo.dictionary_record",
                c => new
                    {
                        dictionary_record_id = c.Long(nullable: false, identity: true),
                        dictionary_uid = c.Guid(nullable: false),
                        street = c.String(nullable: false),
                        city = c.String(nullable: false),
                        rename_street_to = c.String(),
                        area = c.String(),
                    })
                .PrimaryKey(t => t.dictionary_record_id)
                .ForeignKey("dbo.dictionary", t => t.dictionary_uid, cascadeDelete: true)
                .Index(t => t.dictionary_uid, name: "IX_DICTIONARY_RECORD_DICTIONARY_UID");
            
            CreateTable(
                "dbo.dictionary_record_conditions",
                c => new
                    {
                        dictionary_record_conditions_id = c.Long(nullable: false, identity: true),
                        dictionary_record_id = c.Long(nullable: false),
                        from = c.Long(nullable: false),
                        to = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.dictionary_record_conditions_id)
                .ForeignKey("dbo.dictionary_record", t => t.dictionary_record_id, cascadeDelete: true)
                .Index(t => t.dictionary_record_id, name: "IX_DICTIONARY_RECORD_CONDITIONS_DICTIONARY_RECORD_ID");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.dictionary_record_conditions", "dictionary_record_id", "dbo.dictionary_record");
            DropForeignKey("dbo.dictionary_record", "dictionary_uid", "dbo.dictionary");
            DropForeignKey("dbo.dictionary_exclude", "dictionary_uid", "dbo.dictionary");
            DropForeignKey("dbo.dictionary", "account_uid", "dbo.account");
            DropIndex("dbo.dictionary_record_conditions", "IX_DICTIONARY_RECORD_CONDITIONS_DICTIONARY_RECORD_ID");
            DropIndex("dbo.dictionary_record", "IX_DICTIONARY_RECORD_DICTIONARY_UID");
            DropIndex("dbo.dictionary_exclude", "IX_DICTIONARY_EXCLUDE_DICTIONARY_UID");
            DropIndex("dbo.dictionary", new[] { "account_uid" });
            DropTable("dbo.dictionary_record_conditions");
            DropTable("dbo.dictionary_record");
            DropTable("dbo.dictionary_exclude");
            DropTable("dbo.dictionary");
        }
    }
}
