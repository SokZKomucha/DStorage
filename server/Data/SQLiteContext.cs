using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data {
  public class SQLiteContext : DbContext {
    public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options) {}
    public DbSet<TestEntity> TestEntities { get; set; }
  }
}