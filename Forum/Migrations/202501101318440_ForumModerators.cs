namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForumModerators : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForumModerators",
                c => new
                    {
                        ForumId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ForumId, t.UserId })
                .ForeignKey("dbo.Fora", t => t.ForumId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ForumId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ForumModerators", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ForumModerators", "ForumId", "dbo.Fora");
            DropIndex("dbo.ForumModerators", new[] { "UserId" });
            DropIndex("dbo.ForumModerators", new[] { "ForumId" });
            DropTable("dbo.ForumModerators");
        }
    }
}
