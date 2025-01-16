namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class przypieteWatki : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Threads", "isPinned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Threads", "isPinned");
        }
    }
}
