using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.DataBase
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
              : base(options)
        {

        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            SeedUsers(builder);
            base.OnModelCreating(builder);
        }

        private static void SeedUsers(ModelBuilder builder)
        {
            var adminId = new Guid("07370b60-fb9c-46dc-98e4-0c7112a2c15f");
            var roalId = Guid.NewGuid();
            builder.Entity<IdentityRole<Guid>>().HasData(new IdentityRole<Guid> {
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN",
                Id = roalId,
                ConcurrencyStamp = roalId.ToString()
            });

            var appUser = new ApplicationUser {
                Id = adminId,
                Email = "Avihay@gmail.com",
                EmailConfirmed = true,
                UserName = "Avihay@gmail.com",
                NormalizedUserName = "Avihay@GMAIL.COM",
                SecurityStamp = roalId.ToString()
            };

            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            appUser.PasswordHash = ph.HashPassword(appUser, "admin");

            builder.Entity<ApplicationUser>().HasData(appUser);

            builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>() {

                RoleId = roalId,
                UserId = adminId
            });
        }
    }
}
