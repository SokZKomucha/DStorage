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

      using var fileStream = new FileStream($"./test/{Guid.NewGuid()}", FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
      await Request.Body.CopyToAsync(fileStream);
      if (fileStream.Length != Request.ContentLength) {
        return BadRequest("Content length does not match with received length.");
      }

      if (discordBot.discordClient == null) {
        return StatusCode(500);
      }

      var filename = Path.GetFileName(Request.Headers.TryGetValue("X-Filename", out var values) ? values.FirstOrDefault() ?? Guid.NewGuid().ToString() : Guid.NewGuid().ToString()); // Optional filename, fallback if not provided
      var chunkCount = (long)Math.Ceiling((double)fileStream.Length / chunkSize);
      var discordChannel = await discordBot.discordClient.GetChannelAsync(ulong.Parse(configuration["DiscordChannelId"] ?? "0")); // Will throw if a channel doesn't exist/is inaccessible, so that's good

      var file = new FileModel();
      file.UserId = user.Id;
      file.Filename = filename;
      file.FileSize = fileStream.Length;
      file.UploadDate = DateTime.Now;
      await database.Files.AddAsync(file);
      await database.SaveChangesAsync(); // To get file's ID

      fileStream.Position = 0; // Obligatory position reset
      for (long i = 0; i < chunkCount; i++) {

        long byteCount = Math.Min(chunkSize, fileStream.Length - fileStream.Position);
        byte[] buffer = new byte[byteCount];
        var chunkFileStream = new FileStream($"{Guid.NewGuid()}", FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose);
        // FileStream initialization could probably be moved outside the loop, as to not create new temp file with every iteration

        await fileStream.ReadExactlyAsync(buffer);        
        await chunkFileStream.WriteAsync(buffer);
        chunkFileStream.Position = 0;
        // Console.WriteLine($"{buffer.Length} {chunkFileStream.Length}/{chunkFileStream.Position} {fileStream.Length}/{fileStream.Position}");

        var messageBuilder = new DiscordMessageBuilder();
        messageBuilder.Content = $"FileID: {file.Id}\nChunk {i + 1}/{chunkCount}";
        messageBuilder.AddFile(chunkFileStream);
        var message = await discordChannel.SendMessageAsync(messageBuilder); // Again, will throw by itself if something was to happen

        var chunk = new ChunkModel();
        chunk.FileId = file.Id;
        chunk.ByteCount = byteCount;
        chunk.DiscordMessageId = message.Id;
        await database.Chunks.AddAsync(chunk);
      }

      await database.SaveChangesAsync();
      return Ok(new FileInfoDTO(file.Id, file.UserId, file.Filename, file.FileSize, file.UploadDate));
    }
  }
}