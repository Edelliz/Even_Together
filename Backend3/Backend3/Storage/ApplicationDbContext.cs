using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend3.Storage
{
    public class ApplicationDbContext
        : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public override DbSet<User> Users { get; set; }
        public override DbSet<Role> Roles { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Searching> Searching { get; set; }
        public DbSet<UsersEvents> UsersEvents { get; set; }
        public override DbSet<UserRole> UserRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(o =>
            {
                o.ToTable("Users");
            });
            builder.Entity<Role>(o =>
            {
                o.ToTable("Roles");
            });
            builder.Entity<UserRole>(o =>
            {
                o.ToTable("UserRoles");
                o.HasOne(x => x.Role)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                o.HasOne(x => x.User)
                    .WithMany(x => x.Roles)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<User>().HasKey(x => x.Id);
            builder.Entity<UsersEvents>().HasKey(u => new { u.EventId, u.UserId });
            builder.Entity<Searching>().HasKey(u => new { u.EventId, u.UserId });
            builder.Entity<UsersEvents>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(user => user.Id)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Event>().HasKey(x => x.Id);
            builder.Entity<UsersEvents>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Searching>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Searching>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Event>().HasKey(x => x.Id);
        }
    }

}
