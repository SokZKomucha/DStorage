// Subject to change, will be deleted in the future.

using DStorage.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Server.Data;

namespace Server.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class TestController : ControllerBase {
    
    private readonly SQLiteContext database;
    private readonly DiscordBotService discordBot;

    public TestController(SQLiteContext database, DiscordBotService discordBot) {
      this.database = database;
      this.discordBot = discordBot;
    }

    [HttpGet]
    public async Task<IActionResult> Get() {  

      return Content(discordBot?.discordClient?.CurrentUser.Username ?? "Nouua");

      // // Test if it even sends the file
      // if (Request.Cookies["secret"] == null) return BadRequest();
      // var user = database.Users.Where(x => x.Secret == Request.Cookies["secret"]);
      // if (!user.Any()) return NotFound();
      // var filePath = Path.GetFullPath("./test.txt");      
      // var fs = new FileStream(filePath, FileMode.Open);      
      // return File(fs, "application/octet-stream", Path.GetFileName(filePath));
    }
  }
}