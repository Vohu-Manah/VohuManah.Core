import { apiClient } from '../api/client';
import type { Publisher, PublisherName, CreatePublisherRequest, UpdatePublisherRequest } from '../../models/publisher';

export const publisherService = {
  getAll: async (): Promise<Publisher[]> => {
    const response = await apiClient.get<Publisher[]>('/library/publishers');
    return response.data;
  },

  getList: async (): Promise<Publisher[]> => {
    const response = await apiClient.get<Publisher[]>('/library/publishers/list');
    return response.data;
  },

  getById: async (id: number): Promise<Publisher> => {
    const response = await apiClient.get<Publisher>(`/library/publishers/${id}`);
    return response.data;
  },

  getNames: async (): Promise<PublisherName[]> => {
    const response = await apiClient.get<PublisherName[]>('/library/publishers/names');
    return response.data;
  },

  create: async (data: CreatePublisherRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/publishers', data);
    return response.data;
  },

  update: async (data: UpdatePublisherRequest): Promise<void> => {
    await apiClient.put('/library/publishers', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/publishers/${id}`);
  },
};

