namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodanieViews : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Threads", "Views", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Threads", "Views");
        }
    }
}
