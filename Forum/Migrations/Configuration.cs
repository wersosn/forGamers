namespace Forum.Migrations
{
    using Forum.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Forum.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Forum.Models.ApplicationDbContext";
        }

        protected override void Seed(Forum.Models.ApplicationDbContext context)
        {
            context.Categories.AddOrUpdate(
                c => c.Name,
                new Category { Name = "Gry" },
                new Category { Name = "Filmy" },
                new Category { Name = "Muzyka" },
                new Category { Name = "Sport" }
            );
        }
    }
}
