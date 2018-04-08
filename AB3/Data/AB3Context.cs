using AB3.Models;
using Microsoft.EntityFrameworkCore;

namespace AB3.Data
{
    public class AB3Context : DbContext
    {
        public AB3Context (DbContextOptions<AB3Context> options)
            : base(options)
        {
        }

        public DbSet<Project> Project { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ProjectCategory> ProjectCategory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectCategory>()
                .HasKey(c => new { c.ProjectId, c.CategoryId });

            modelBuilder.Entity<ProjectCategory>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.ProjectCategories)
                .HasForeignKey(pc => pc.ProjectId);

            modelBuilder.Entity<ProjectCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProjectCategories)
                .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
