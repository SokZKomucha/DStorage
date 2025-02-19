import type { IFileDetails } from "./IFileDetails";

/** Represents file details response. Used exclusively for file details page */
export interface IFileDetailsResponse {
  fileDetails: IFileDetails | null;
  // More properties to be added
}