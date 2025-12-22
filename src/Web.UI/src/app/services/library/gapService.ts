import { apiClient } from '../api/client';
import type { Gap, GapName, CreateGapRequest, UpdateGapRequest } from '../../models/gap';

export const gapService = {
  getAll: async (): Promise<Gap[]> => {
    const response = await apiClient.get<Gap[]>('/library/gaps');
    return response.data;
  },

  getList: async (): Promise<Gap[]> => {
    const response = await apiClient.get<Gap[]>('/library/gaps/list');
    return response.data;
  },

  getById: async (id: number): Promise<Gap> => {
    const response = await apiClient.get<Gap>(`/library/gaps/${id}`);
    return response.data;
  },

  getNames: async (): Promise<GapName[]> => {
    const response = await apiClient.get<GapName[]>('/library/gaps/names');
    return response.data;
  },

  create: async (data: CreateGapRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/gaps', data);
    return response.data;
  },

  update: async (data: UpdateGapRequest): Promise<void> => {
    await apiClient.put('/library/gaps', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/gaps/${id}`);
  },
};

