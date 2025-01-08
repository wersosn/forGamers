namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zmianaModeli : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Threads", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Threads", new[] { "CategoryId" });
            CreateTable(
                "dbo.Fora",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            AddColumn("dbo.Threads", "ForumId", c => c.Int(nullable: false));
            CreateIndex("dbo.Threads", "ForumId");
            AddForeignKey("dbo.Threads", "ForumId", "dbo.Fora", "Id", cascadeDelete: true);
            DropColumn("dbo.Threads", "CategoryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Threads", "CategoryId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Threads", "ForumId", "dbo.Fora");
            DropForeignKey("dbo.Fora", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Threads", new[] { "ForumId" });
            DropIndex("dbo.Fora", new[] { "CategoryId" });
            DropColumn("dbo.Threads", "ForumId");
            DropTable("dbo.Fora");
            CreateIndex("dbo.Threads", "CategoryId");
            AddForeignKey("dbo.Threads", "CategoryId", "dbo.Categories", "Id", cascadeDelete: true);
        }
    }
}
