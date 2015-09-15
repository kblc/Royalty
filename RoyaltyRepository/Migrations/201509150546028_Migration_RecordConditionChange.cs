namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_RecordConditionChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.dictionary_record_conditions", "even", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.dictionary_record_conditions", "even");
        }
    }
}
