import { apiClient } from '../api/client';
import type { PublicationType, PublicationTypeName, CreatePublicationTypeRequest, UpdatePublicationTypeRequest } from '../../models/publicationType';

export const publicationTypeService = {
  getAll: async (): Promise<PublicationType[]> => {
    const response = await apiClient.get<PublicationType[]>('/library/publication-types');
    return response.data;
  },

  getList: async (): Promise<PublicationType[]> => {
    const response = await apiClient.get<PublicationType[]>('/library/publication-types/list');
    return response.data;
  },

  getById: async (id: number): Promise<PublicationType> => {
    const response = await apiClient.get<PublicationType>(`/library/publication-types/${id}`);
    return response.data;
  },

  getNames: async (): Promise<PublicationTypeName[]> => {
    const response = await apiClient.get<PublicationTypeName[]>('/library/publication-types/names');
    return response.data;
  },

  create: async (data: CreatePublicationTypeRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/publication-types', data);
    return response.data;
  },

  update: async (data: UpdatePublicationTypeRequest): Promise<void> => {
    await apiClient.put('/library/publication-types', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/publication-types/${id}`);
  },
};

