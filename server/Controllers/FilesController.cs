using System.Text;
using CommunityToolkit.HighPerformance.Helpers;
using DSharpPlus.Entities;
using DStorage.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Server.Data;
using Server.DTOs;
using Server.Models;

namespace Server.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class FilesController : ControllerBase {

    // The values below may be changed
    // Maximum file size is set to comfy 4 GiB, I don't see any problems increasing this value
    // Chunk size, however, determines how large the files sent to Discord will be. Obviously has to fit in 10 MiB limit.
    public const long maxFileSize = 4L * 1024 * 1024 * 1024;
    public const long chunkSize = 9 * 1024 * 1024;

    private readonly SQLiteContext database;
    private readonly DiscordBotService discordBot;
    private readonly IConfiguration configuration;

    public FilesController(SQLiteContext database, DiscordBotService discordBot, IConfiguration configuration) {
      this.database = database;
      this.discordBot = discordBot;
      this.configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int pageNumber = 0) {
      if (pageNumber < 0) {
        return BadRequest("pageNumber may not be smaller than 0.");
      }
      
      if (Request.Cookies["secret"] == null) {
        return BadRequest("Missing \"secret\" cookie.");
      }

      var user = database.Users.Where(x => x.Secret == Request.Cookies["secret"])?.FirstOrDefault();
      if (user == null) {
        return StatusCode(403, "User not found");
      }

      int filesPerPage = 3;
      var allUserFiles = database.Files.Where(x => x.UserId == user.Id);
      var totalUserFileCount = await allUserFiles.CountAsync();
      var totalPageCount = (int)Math.Ceiling((decimal)totalUserFileCount / filesPerPage);

      var pageFiles = allUserFiles
        .OrderBy(x => x.Id)
        .Skip(pageNumber * filesPerPage)
        .Take(filesPerPage)
        .ToList()
        .Select(x => new FileDTO(x.Id, x.UserId, x.Filename, x.FileSize, x.UploadDate))
        .ToArray();
      var pageFileCount = pageFiles.Length; 

      return Ok(new FilesDTO(
        totalPageCount,
        pageFileCount != 0 ? pageNumber : null,
        (pageNumber > 0 && pageNumber - totalPageCount <= 0) ? pageNumber - 1 : null,
        pageNumber + 1 < totalPageCount ? pageNumber + 1 : null,
        pageFileCount,
        totalUserFileCount,
        pageFiles
      ));
    }

    [HttpGet("{fileId}")]
    public async Task<IActionResult> GetOne([FromRoute] long fileId) {
      if (Request.Cookies["secret"] == null) {
        return BadRequest("Missing \"secret\" cookie.");
      }

      var user = database.Users.Where(x => x.Secret == Request.Cookies["secret"])?.FirstOrDefault();
      if (user == null) {
        return StatusCode(403, "User not found");
      }

      var file = database.Files.Where(x => x.Id == fileId && x.UserId == user.Id)?.FirstOrDefault();
      if (file == null) {
        return NotFound("File not found.");
      } 

      return Ok(new FileDTO(file.Id, file.UserId, file.Filename, file.FileSize, file.UploadDate));
    }

    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> Download([FromRoute] long fileId) {
      return Content($"Download; fileId={fileId}");
      // Obviously check whether the file belongs to the user
    }

    [HttpPost("upload")]
    [RequestSizeLimit(512 * 1024)]
    public async Task<IActionResult> Upload() {
      if (Request.Cookies["secret"] == null) {
        return BadRequest("Missing \"secret\" cookie.");
      }

      var user = database.Users.Where(x => x.Secret == Request.Cookies["secret"])?.FirstOrDefault();
      if (user == null) {
        return StatusCode(403, "User not found.");
      }

      using var fileStream = new FileStream($"{Guid.NewGuid()}", FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
      await Request.Body.CopyToAsync(fileStream);
      if (fileStream.Length != Request.ContentLength) {
        await fileStream.DisposeAsync();
        return BadRequest("Content length does not match with received length.");
      }

      if (discordBot.discordClient == null) {
        await fileStream.DisposeAsync();
        return StatusCode(500);
      }

      var filename = Path.GetFileName(Request.Headers.TryGetValue("X-Filename", out var values) ? values.FirstOrDefault() ?? Guid.NewGuid().ToString() : Guid.NewGuid().ToString());
      var chunkCount = (long)Math.Ceiling((double)fileStream.Length / chunkSize);
      var discordChannel = await discordBot.discordClient.GetChannelAsync(ulong.Parse(configuration["DiscordChannelId"] ?? "0"));

      var file = new FileModel();
      file.UserId = user.Id;
      file.Filename = filename;
      file.FileSize = fileStream.Length;
      file.UploadDate = DateTime.Now;
      await database.Files.AddAsync(file);
      await database.SaveChangesAsync();

      fileStream.Position = 0;
      for (long i = 0; i < chunkCount; i++) {

        // FileStream initialization could probably be moved outside the loop, as to not create new temp file with every iteration
        long byteCount = Math.Min(chunkSize, fileStream.Length - fileStream.Position);
        byte[] buffer = new byte[byteCount];
        var chunkFileStream = new FileStream($"{Guid.NewGuid()}", FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
        await fileStream.ReadExactlyAsync(buffer);        
        await chunkFileStream.WriteAsync(buffer);
        chunkFileStream.Position = 0;

        var messageBuilder = new DiscordMessageBuilder();
        messageBuilder.Content = $"FileID: {file.Id}\nChunk {i + 1}/{chunkCount}";
        messageBuilder.AddFile(chunkFileStream);
        var message = await discordChannel.SendMessageAsync(messageBuilder);

        var chunk = new ChunkModel();
        chunk.FileId = file.Id;
        chunk.ByteCount = byteCount;
        chunk.DiscordMessageId = message.Id;
        await database.Chunks.AddAsync(chunk);
        await chunkFileStream.DisposeAsync();
      }

      // Error handling for D#+ errors is probably not needed; if something were
      // to happen on D#+ side, it'd most likely throw and end the request with HTTP 500
      // Though, in case of error, the filestream will most likely get disposed only after application restart. That may need to be worked on.

      await fileStream.DisposeAsync();
      await database.SaveChangesAsync();
      return Ok(new FileDTO(file.Id, file.UserId, file.Filename, file.FileSize, file.UploadDate));
    }
  }
}