using Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Web.Hosting;

[assembly: OwinStartupAttribute(typeof(Forum.Startup))]
namespace Forum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
            CreateRolesAndUsers();
        }
        private async void CreateRolesAndUsers()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            // Tworzenie roli "admin" (jeśli nie istnieje)
            var roleExist = await roleManager.RoleExistsAsync("admin");
            if (!roleExist)
            {
                var role = new IdentityRole("admin");
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    return;
                }
            }

            // Tworzenie domyślnego admina
            var user = await userManager.FindByEmailAsync("admin@admin.pl");

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = "admin@admin.pl",
                    Email = "admin@admin.pl",
                };
                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user.Id, "admin");
                }
                else
                {
                    return;
                }
            }

            // Tworzenie domyślnego moderatora:
            var mod = await userManager.FindByEmailAsync("mod@mod.pl");

            if (mod == null)
            {
                mod = new ApplicationUser()
                {
                    UserName = "mod@mod.pl",
                    Email = "mod@mod.pl",
                };
                var result = await userManager.CreateAsync(mod, "Abc123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(mod.Id, "moderator");
                }
                else
                {
                    return;
                }
            }
        }
        private void CreateRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                // Sprawdź, czy rola "admin" istnieje, jeśli nie, dodaj ją
                if (!roleManager.RoleExists("admin"))
                {
                    roleManager.Create(new IdentityRole("admin"));
                }

                // Sprawdź, czy rola "user" istnieje, jeśli nie, dodaj ją
                if (!roleManager.RoleExists("user"))
                {
                    roleManager.Create(new IdentityRole("user"));
                }

                // Sprawdź, czy rola "moderator" istnieje, jeśli nie, dodaj ją
                if (!roleManager.RoleExists("moderator"))
                {
                    roleManager.Create(new IdentityRole("moderator"));
                }
            }
        }
    }
}
