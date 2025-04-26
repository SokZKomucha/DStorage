namespace Server.DTOs {
  public record FileDTO(long Id, long UserId, string Filename, long FileSize, DateTime UploadDate);
}