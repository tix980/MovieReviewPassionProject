namespace MovieReviewPassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeMovieImg : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Movies", "MovieImg");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Movies", "MovieImg", c => c.String());
        }
    }
}
