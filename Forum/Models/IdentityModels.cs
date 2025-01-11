using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Forum.Models
{
    // Możesz dodać dane profilu dla użytkownika, dodając więcej właściwości do klasy ApplicationUser. Odwiedź stronę https://go.microsoft.com/fwlink/?LinkID=317594, aby dowiedzieć się więcej.
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ForumModerator> ForumsModerating { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Element authenticationType musi pasować do elementu zdefiniowanego w elemencie CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Dodaj tutaj niestandardowe oświadczenia użytkownika
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        { }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Thread> Threads { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<ForumModerator> ForumModerators { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<ForbiddenWord> ForbiddenWords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ForumModerator>()
                .HasKey(fm => new { fm.ForumId, fm.UserId });

            modelBuilder.Entity<ForumModerator>()
                .HasRequired(fm => fm.Forum)
                .WithMany(f => f.Moderators)
                .HasForeignKey(fm => fm.ForumId);

            modelBuilder.Entity<ForumModerator>()
                .HasRequired(fm => fm.User)
                .WithMany(u => u.ForumsModerating)
                .HasForeignKey(fm => fm.UserId);
        }
    }
}