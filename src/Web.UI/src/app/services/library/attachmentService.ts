import { apiClient } from '../api/client';
import type { AttachmentResponse } from '../../models/attachment';

export const attachmentService = {
  upload: async (
    entityType: string,
    entityId: number,
    file: File,
    description?: string,
  ): Promise<number> => {
    const formData = new FormData();
    formData.append('entityType', entityType);
    formData.append('entityId', String(entityId));
    formData.append('file', file);
    if (description) {
      formData.append('description', description);
    }

    // Don't set Content-Type header - let browser set it with boundary
    const response = await apiClient.post<number>('/library/attachments', formData);
    return response.data;
  },

  getByEntity: async (entityType: string, entityId: number): Promise<AttachmentResponse[]> => {
    const response = await apiClient.get<AttachmentResponse[]>(
      `/library/attachments/${entityType}/${entityId}`,
    );
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/attachments/${id}`);
  },
};

