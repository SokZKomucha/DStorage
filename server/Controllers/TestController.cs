using Microsoft.AspNetCore.Mvc;
using Server.Data;

namespace Server.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class TestController : ControllerBase {
    
    SQLiteContext database;

    public TestController(SQLiteContext database) {
      this.database = database;
    }
    
    [HttpGet]
    public IActionResult Get() {
      var entries = database.TestEntities.ToList();
      return Ok(entries);
    }
  }
}