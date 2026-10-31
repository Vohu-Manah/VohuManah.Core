import { apiClient } from '@/app/services/api/client';
import type { AttachmentResponse } from '@/app/models/attachment';

export const attachmentService = {
  upload: async (
    entityType: string,
    entityId: number,
    file: File,
    description?: string,
    isMain?: boolean,
  ): Promise<number> => {
    const formData = new FormData();
    formData.append('entityType', entityType);
    formData.append('entityId', String(entityId));
    formData.append('file', file);
    if (description) {
      formData.append('description', description);
    }
    if (isMain !== undefined) {
      formData.append('isMain', String(isMain));
    }

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

  update: async (
    id: number,
    description?: string,
    isMain?: boolean,
  ): Promise<void> => {
    await apiClient.put(`/library/attachments/${id}`, {
      description,
      isMain,
    });
  },
};
