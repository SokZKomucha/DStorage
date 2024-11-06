namespace Server.Models {
  public class FileModel {
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Filename { get; set; } = "";
    public long FileSize { get; set; }
    public DateTime UploadDate { get; set; }

    public ICollection<ChunkModel> Chunks { get; } = new List<ChunkModel>();
    public UserModel User { get; set; } = null!;
  }
}