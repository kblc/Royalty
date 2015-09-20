namespace RoyaltyRepository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration_AddDictionaryConditionScoreTrusField : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE dictionary SET condition_score_for_trust = 0.9");
        }
        
        public override void Down()
        {
        }
    }
}
