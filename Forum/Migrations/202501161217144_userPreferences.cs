namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userPreferences : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPreferences",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        MessagesPerPage = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserPreferences");
        }
    }
}
