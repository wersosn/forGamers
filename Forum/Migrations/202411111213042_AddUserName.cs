namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Threads", "UserName", c => c.String());
            AddColumn("dbo.Messages", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "UserName");
            DropColumn("dbo.Threads", "UserName");
        }
    }
}
