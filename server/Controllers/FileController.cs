using CommunityToolkit.HighPerformance.Helpers;
using DStorage.Services;
using Microsoft.AspNetCore.Mvc;
using Server.Data;

namespace Server.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class FileController : ControllerBase {

    private readonly SQLiteContext database;
    private readonly DiscordBotService discordBot;
    private readonly IConfiguration configuration;

    public FileController(SQLiteContext database, DiscordBotService discordBot, IConfiguration configuration) {
      this.database = database;
      this.discordBot = discordBot;
      this.configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Get() {
      return Content($"Oki");
    }

    [HttpPost("upload")]
    [RequestSizeLimit(1024 * 1024 * 1024 * (long)4)]
    public async Task<IActionResult> Upload() {
      using var fileStream = new FileStream($"./test/{Guid.NewGuid()}", FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096/*, FileOptions.DeleteOnClose*/);
      await HttpContext.Request.Body.CopyToAsync(fileStream);


      return Ok("Oki!!!");
    }
  }
}