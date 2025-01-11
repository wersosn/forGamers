namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sesje : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "InactivityTimeout", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "InactivityTimeout");
        }
    }
}
