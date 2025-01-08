namespace Forum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usuniecieRole : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Role");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Role", c => c.String());
        }
    }
}
