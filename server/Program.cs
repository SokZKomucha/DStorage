using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server {
  internal class Program {
    static void Main(string[] args) {

      WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
      builder.Services.AddControllers();
      
      builder.Services.AddDbContext<SQLiteContext>(options => {
        options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnectionString"));
      });

      WebApplication app = builder.Build();
      app.MapControllers();
      app.Run();

    }
  }
}
