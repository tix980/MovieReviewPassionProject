namespace MovieReviewPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoviePoster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Actors", "ActorPoster", c => c.Boolean(nullable: false));
            AddColumn("dbo.Actors", "ActorPosterExtension", c => c.String());
            AddColumn("dbo.Movies", "MoviePoster", c => c.Boolean(nullable: false));
            AddColumn("dbo.Movies", "MoviePosterExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movies", "MoviePosterExtension");
            DropColumn("dbo.Movies", "MoviePoster");
            DropColumn("dbo.Actors", "ActorPosterExtension");
            DropColumn("dbo.Actors", "ActorPoster");
        }
    }
}
