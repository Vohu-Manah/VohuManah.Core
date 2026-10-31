export type AttachmentResponse = {
  id: number;
  fileName: string;
  originalFileName: string;
  fileUrl: string;
  contentType: string;
  fileSize: number;
  description?: string;
  isMain: boolean;
  createdAt: string;
};


