import { apiClient } from '../api/client';
import type { City, CityName, CreateCityRequest, UpdateCityRequest } from '../../models/city';

export const cityService = {
  getAll: async (): Promise<City[]> => {
    const response = await apiClient.get<City[]>('/library/cities');
    return response.data;
  },

  getList: async (): Promise<City[]> => {
    const response = await apiClient.get<City[]>('/library/cities/list');
    return response.data;
  },

  getById: async (id: number): Promise<City> => {
    const response = await apiClient.get<City>(`/library/cities/${id}`);
    return response.data;
  },

  getNames: async (): Promise<CityName[]> => {
    const response = await apiClient.get<CityName[]>('/library/cities/names');
    return response.data;
  },

  create: async (data: CreateCityRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/cities', data);
    return response.data;
  },

  update: async (data: UpdateCityRequest): Promise<void> => {
    await apiClient.put('/library/cities', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/cities/${id}`);
  },
};

