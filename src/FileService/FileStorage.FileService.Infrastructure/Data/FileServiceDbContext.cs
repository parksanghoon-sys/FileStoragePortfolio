using FileStorage.FileService.Domain;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.FileService.Infrastructure.Data
{
    public class FileServiceDbContext : DbContext
    {
        public FileServiceDbContext(DbContextOptions<FileServiceDbContext> options) : base(options) { }

        public DbSet<FileEntity> Files { get; set; }
        public DbSet<FileShare> FileShares { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FileEntity>()
                .HasMany(f => f.Shares)
                .WithOne(fs => fs.File)
                .HasForeignKey(fs => fs.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FileEntity>()
                .HasMany(f => f.Comments)
                .WithOne(c => c.File)
                .HasForeignKey(c => c.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasMany(c => c.Replies)
                .WithOne(c => c.ParentComment)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}