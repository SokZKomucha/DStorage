namespace Server {
  internal class Program {
    static void Main(string[] args) {

      WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
      builder.Services.AddControllers();
      
      WebApplication app = builder.Build();
      app.MapControllers();
      app.Run();

    }
  }
}
