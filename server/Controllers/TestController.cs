// Subject to change, will be deleted in the future.

using System.Runtime.CompilerServices;
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
    private readonly IConfiguration configuration;

    public TestController(SQLiteContext database, DiscordBotService discordBot, IConfiguration configuration) {
      this.database = database;
      this.discordBot = discordBot;
      this.configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string message) {  
      if (discordBot?.discordClient == null) return StatusCode(500);

      var guild = await discordBot.discordClient.GetGuildAsync(Convert.ToUInt64(configuration["DiscordGuildId"]));
      var channel = await guild.GetChannelAsync(Convert.ToUInt64(configuration["DiscordChannelId"]));
      await channel.SendMessageAsync(message);    

      return Content("Oki");

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