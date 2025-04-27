import type { IFile } from "./IFile";

/** Represents paginated files response */
export interface IPagedFiles {
  pageCount: number;
  pageNumber: number | null;
  previousPage: number | null;
  nextPage: number | null;
  pageFileCount: number;
  totalFileCount: number;
  filesPerPage: number;
  files: IFile[];
}