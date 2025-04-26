namespace Server.DTOs {
  public record FilesDTO(int pageCount, int? pageNumber, int? previousPage, int? nextPage, int fileCount, FileDTO[] files);
}