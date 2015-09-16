namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AccountDictionaryRecord_ChangeIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.dictionary_record", "IX_DICTIONARY_RECORD_DICTIONARY_UID");
            DropIndex("dbo.dictionary_record", new[] { "street_id" });
            CreateIndex("dbo.dictionary_record", new[] { "dictionary_uid", "street_id" }, unique: true, name: "IX_DICTIONARY_RECORD_DICTIONARY_UID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.dictionary_record", "IX_DICTIONARY_RECORD_DICTIONARY_UID");
            CreateIndex("dbo.dictionary_record", "street_id");
            CreateIndex("dbo.dictionary_record", "dictionary_uid", name: "IX_DICTIONARY_RECORD_DICTIONARY_UID");
        }
    }
}
