namespace Server.DTOs {
  public record FilesDTO(int PageCount, int? PageNumber, int? PreviousPage, int? NextPage, int PageFileCount, int TotalFileCount, int filesPerPage, FileDTO[] Files);
}