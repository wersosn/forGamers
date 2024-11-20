namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdminPanel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Role", c => c.String());
            AlterColumn("dbo.Threads", "Title", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Threads", "Title", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.AspNetUsers", "Role");
        }
    }
}
