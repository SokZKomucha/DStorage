/** Represents a file response */
export interface IFile {
  id: number;
  userId: number;
  filename: string;
  fileSize: number;
  uploadDate: string;
}