namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zakazaneSlowa : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ForbiddenWords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Word = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ForbiddenWords");
        }
    }
}
