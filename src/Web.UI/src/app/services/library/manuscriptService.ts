import { apiClient } from '../api/client';
import type { Manuscript, CreateManuscriptRequest, UpdateManuscriptRequest, ManuscriptSearchRequest, ManuscriptSearchResponse } from '../../models/manuscript';

export const manuscriptService = {
  getAll: async (): Promise<Manuscript[]> => {
    const response = await apiClient.get<Manuscript[]>('/library/manuscripts');
    return response.data;
  },

  getList: async (): Promise<Manuscript[]> => {
    const response = await apiClient.get<Manuscript[]>('/library/manuscripts/list');
    return response.data;
  },

  getById: async (id: number): Promise<Manuscript> => {
    const response = await apiClient.get<Manuscript>(`/library/manuscripts/${id}`);
    return response.data;
  },

  create: async (data: CreateManuscriptRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/manuscripts', data);
    return response.data;
  },

  update: async (data: UpdateManuscriptRequest): Promise<void> => {
    await apiClient.put('/library/manuscripts', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/manuscripts/${id}`);
  },

  search: async (params: ManuscriptSearchRequest): Promise<ManuscriptSearchResponse[]> => {
    const response = await apiClient.post<ManuscriptSearchResponse[]>('/library/manuscripts/search', params);
    return response.data;
  },

  getStatistics: async (): Promise<any> => {
    const response = await apiClient.get('/library/manuscripts/statistics');
    return response.data;
  },
};

