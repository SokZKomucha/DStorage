using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Models;

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

    [HttpPost]
    public IActionResult Post([FromBody] TestEntity entity) {
      database.TestEntities.Add(entity);
      database.SaveChanges();
      return Ok();
    }
  }
}