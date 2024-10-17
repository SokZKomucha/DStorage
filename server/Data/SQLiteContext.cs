using Microsoft.EntityFrameworkCore;

namespace Server.Data {
  public class SQLiteContext : DbContext {
    public SQLiteContext(DbContextOptions<SQLiteContext> options) : base(options) {}
    public DbSet<TestEntity> TestEntities { get; set; }
  }

  public class TestEntity {
    public int Id { get; set; }
    public string? Name { get; set; }
  }
}