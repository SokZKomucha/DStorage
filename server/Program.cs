using System.Security.Cryptography;
using System.Text;
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




      // var original = "$2a$10$wjBpYhMPc9oeOVB0yyZ8S.HgTQc07bek5uE132zoVvZeyP2gCMDSe";
      // var good = BCrypt.Net.BCrypt.EnhancedVerify("asdfe", original);
      // Console.WriteLine(good);


    }
  }
}
