// Subject to change, will be deleted in the future.

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
      var users = database.Users;
      return Ok(users);
    }
  }
}