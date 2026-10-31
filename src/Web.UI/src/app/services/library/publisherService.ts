import { apiClient } from '@/app/services/api/client';
import type { Publisher, PublisherName, CreatePublisherRequest, UpdatePublisherRequest } from '@/app/models/publisher';

type PublisherApiResponse = Omit<Publisher, 'cityId'> & { placeId: number };

export const publisherService = {
  getAll: async (): Promise<Publisher[]> => {
    const response = await apiClient.get<PublisherApiResponse[]>('/library/publishers');
    return response.data.map((p) => ({
      ...p,
      cityId: p.placeId,
    }));
  },

  getList: async (): Promise<Publisher[]> => {
    const response = await apiClient.get<Publisher[]>('/library/publishers/list');
    return response.data;
  },

  getById: async (id: number): Promise<Publisher> => {
    const response = await apiClient.get<PublisherApiResponse>(`/library/publishers/${id}`);
    return {
      ...response.data,
      cityId: response.data.placeId,
    };
  },

  getNames: async (): Promise<PublisherName[]> => {
    const response = await apiClient.get<PublisherName[]>('/library/publishers/names');
    return response.data;
  },

  create: async (data: CreatePublisherRequest): Promise<number> => {
    const { cityId, website, email, ...rest } = data;
    const requestData = { 
      ...rest, 
      placeId: cityId,
      website: website || '',
      email: email || ''
    };
    const response = await apiClient.post<number>('/library/publishers', requestData);
    return response.data;
  },

  update: async (id: number, data: UpdatePublisherRequest): Promise<void> => {
    const { cityId, website, email, ...rest } = data;
    const requestData = { 
      ...rest, 
      id,
      placeId: cityId,
      website: website || '',
      email: email || ''
    };
    await apiClient.put(`/library/publishers/${id}`, requestData);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/publishers/${id}`);
  },
};

