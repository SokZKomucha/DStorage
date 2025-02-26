// Subject to change, will be deleted in the future.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
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
    public async Task<IActionResult> Get() {  
      // Test if it even sends the file

      if (Request.Cookies["secret"] == null) return BadRequest();
      var user = database.Users.Where(x => x.Secret == Request.Cookies["secret"]);
      if (!user.Any()) return NotFound();

      var filePath = Path.GetFullPath("./test.txt");      
      var fs = new FileStream(filePath, FileMode.Open);      
      return File(fs, "application/octet-stream", Path.GetFileName(filePath));
    }
  }
}