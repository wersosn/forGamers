namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WatkiMinDlugosc2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Threads", "Title", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Threads", "Title", c => c.String(nullable: false));
        }
    }
}
