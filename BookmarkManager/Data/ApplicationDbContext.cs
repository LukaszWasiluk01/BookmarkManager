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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "youtube" },
                new Category { Id = 2, Name = "netflix" },
                new Category { Id = 3, Name = "blogi" }
            );
        }
    }
}