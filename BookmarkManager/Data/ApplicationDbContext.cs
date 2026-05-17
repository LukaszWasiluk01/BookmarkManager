using BookmarkManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookmarkManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories
        {
            get; set;
        }
        public DbSet<Bookmark> Bookmarks
        {
            get; set;
        }
        public DbSet<Report> Reports
        {
            get; set;
        }
        public DbSet<Friendship> Friendships
        {
            get; set;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "youtube" },
                new Category { Id = 2, Name = "netflix" },
                new Category { Id = 3, Name = "blogi" }
            );

            builder.Entity<Friendship>()
                .HasOne(f => f.Requester)
                .WithMany()
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friendship>()
                .HasOne(f => f.Addressee)
                .WithMany()
                .HasForeignKey(f => f.AddresseeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}