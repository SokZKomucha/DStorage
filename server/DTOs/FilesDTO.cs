namespace Server.DTOs {
  public record FilesDTO(int pageCount, int? pageNumber, int? previousPage, int? nextPage, int pageFileCount, int totalFileCount, FileDTO[] files);
}