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

      var users = database.Users.Where(x => x.Id <= 5);
      users.ToList().ForEach(x => {
        Console.WriteLine($"{x.Id}");
      });

      return Ok("Ok!!!");
    }
  }
}