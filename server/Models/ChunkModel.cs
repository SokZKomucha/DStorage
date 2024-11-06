namespace Server.Models {
  public class ChunkModel {
    public long Id { get; set; }
    public long FileId { get; set; }
    public ulong DiscordMessageId { get; set; }

    public FileModel File { get; set; } = null!;    
  }
}