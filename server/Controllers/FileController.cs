using System.Text;
using CommunityToolkit.HighPerformance.Helpers;
using DSharpPlus.Entities;
using DStorage.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Server.Data;
using Server.DTOs;
using Server.Models;

namespace Server.Controllers {
  [Route("/api/[controller]")]
  [ApiController]
  public class FileController : ControllerBase {

    // The values below may be changed
    // Maximum file size is set to comfy 4 GiB, I don't see any problems increasing this value
    // Chunk size, however, determines how large the files sent to Discord will be. Obviously has to fit in 10 MiB limit.
    public const long maxFileSize = 4L * 1024 * 1024 * 1024;
    public const long chunkSize = 9 * 1024 * 1024;

    private readonly SQLiteContext database;
    private readonly DiscordBotService discordBot;
    private readonly IConfiguration configuration;

    public FileController(SQLiteContext database, DiscordBotService discordBot, IConfiguration configuration) {
      this.database = database;
      this.discordBot = discordBot;
      this.configuration = configuration;
    }

    // [HttpGet]
    // public async Task<IActionResult> Get() {
    //   return Content($"Oki");
    // }

    [HttpPost("upload")]
    [RequestSizeLimit(maxFileSize)]
    public async Task<IActionResult> Upload() {
      if (Request.Cookies["secret"] == null) {
        return BadRequest("Missing \"secret\" cookie.");
      }

      var user = database.Users.Where(x => x.Secret == Request.Cookies["secret"])?.First();
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
      return Ok(new FileInfoDTO(file.Id, file.UserId, file.Filename, file.FileSize, file.UploadDate));
    }
  }
}