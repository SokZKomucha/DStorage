namespace Server.DTOs {
  public record FileInfoDTO(long Id, long UserId, string Filename, long FileSize, DateTime UploadDate);

}