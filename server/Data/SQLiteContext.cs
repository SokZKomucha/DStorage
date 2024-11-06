using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data {
  public class SQLiteContext : DbContext {
    public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options) {}
    public DbSet<UserModel> Users { get; set; }
    public DbSet<FileModel> Files { get; set; }
    public DbSet<ChunkModel> Chunks { get; set; }
  }
}