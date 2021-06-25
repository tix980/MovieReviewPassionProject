namespace MovieReviewPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeActorImg : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Actors", "ActorImg");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Actors", "ActorImg", c => c.String());
        }
    }
}
