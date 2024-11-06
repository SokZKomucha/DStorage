using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data {
  public class SQLiteContext : DbContext {
    public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options) {}
    public DbSet<UserModel> Users { get; set; }
    public DbSet<FileModel> Files { get; set; }
    public DbSet<ChunkModel> Chunks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
          modelBuilder.Entity<UserModel>()
            .HasMany(x => x.Files)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

          modelBuilder.Entity<FileModel>()
            .HasMany(x => x.Chunks)
            .WithOne(x => x.File)
            .HasForeignKey(x => x.FileId)
            .IsRequired();
        }
    }
}