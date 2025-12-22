import { apiClient } from '../api/client';
import type { Publication, CreatePublicationRequest, UpdatePublicationRequest, PublicationSearchRequest, PublicationSearchResponse } from '../../models/publication';

export const publicationService = {
  getAll: async (): Promise<Publication[]> => {
    const response = await apiClient.get<Publication[]>('/library/publications');
    return response.data;
  },

  getList: async (): Promise<Publication[]> => {
    const response = await apiClient.get<Publication[]>('/library/publications/list');
    return response.data;
  },

  getById: async (id: number): Promise<Publication> => {
    const response = await apiClient.get<Publication>(`/library/publications/${id}`);
    return response.data;
  },

  create: async (data: CreatePublicationRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/publications', data);
    return response.data;
  },

  update: async (data: UpdatePublicationRequest): Promise<void> => {
    await apiClient.put('/library/publications', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/publications/${id}`);
  },

  search: async (params: PublicationSearchRequest): Promise<PublicationSearchResponse[]> => {
    const response = await apiClient.post<PublicationSearchResponse[]>('/library/publications/search', params);
    return response.data;
  },

  getStatistics: async (): Promise<any> => {
    const response = await apiClient.get('/library/publications/statistics');
    return response.data;
  },
};

