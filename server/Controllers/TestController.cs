using Microsoft.AspNetCore.Mvc;
using Server.Data;

namespace Server.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class TestController : ControllerBase {
    
    private readonly SQLiteContext database;

    public TestController(SQLiteContext database) {
      this.database = database;
    }

    [HttpGet]
    public IActionResult Get() {

      var files = database.Users
        .Where(x => x.Id == 1)
        .SelectMany(x => x.Files);

      files.ToList().ForEach(x => {
        Console.WriteLine($"{x.Id}\t{x.Filename}");
      });

      return Ok("Ok!!!");
    }
  }
}