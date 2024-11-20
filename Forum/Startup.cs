using Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Web.Hosting;

[assembly: OwinStartupAttribute(typeof(Forum.Startup))]
namespace Forum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers();
        }
        private void CreateRolesAndUsers()
        {
            // Tworzenie roli i przypisanie jej do użytkownika
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            // Sprawdź, czy rola "Admin" już istnieje
            if (!roleManager.RoleExists("admin"))
            {
                // Tworzenie roli "Admin"
                var role = new IdentityRole("admin");
                roleManager.Create(role);
            }

            // Sprawdź, czy rola "User" już istnieje
            if (!roleManager.RoleExists("user"))
            {
                // Tworzenie roli "User"
                var role = new IdentityRole("user");
                roleManager.Create(role);
            }

            // Sprawdź, czy użytkownik o podanym adresie e-mail istnieje
            var user = userManager.FindByEmail("admin@admin.com");

            if (user == null)
            {
                // Tworzenie użytkownika
                user = new ApplicationUser()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                };

                var result = userManager.Create(user, "Admin123!");
                userManager.AddToRole(user.Id, "admin");

                /*if (result.Succeeded)
                {
                    // Przypisanie roli "Admin" do użytkownika
                    userManager.AddToRole(user.Id, "admin");
                }*/
            }
        }
    }
}
