using DSharpPlus;
using DStorage.Services;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server {
  internal class Program {
    public static async Task Main(string[] args) {

      WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
      builder.Services.AddControllers();
      
      builder.Services.AddCors(options => {
        options.AddPolicy("allow", policyBuilder => {
          policyBuilder.AllowAnyHeader();
          policyBuilder.AllowAnyMethod();
          policyBuilder.AllowCredentials();
          policyBuilder.WithOrigins("http://localhost:5173", "https://localhost:5173");
        });
      });

      builder.Services.AddDbContext<SQLiteContext>(options => {
        options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnectionString"));
      });

      builder.Services.AddSingleton<DiscordBotService>();
      builder.Services.AddHostedService(provider => provider.GetRequiredService<DiscordBotService>());



      WebApplication app = builder.Build();
      app.MapControllers();
      app.UseCors("allow");
      app.Run();

    }
  }
}
