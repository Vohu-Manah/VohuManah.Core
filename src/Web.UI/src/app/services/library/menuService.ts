import { apiClient } from '@/app/services/api/client';
import type { MenuItem, CreateMenuItemRequest, UpdateMenuItemRequest, ReorderMenuItemsRequest } from '@/app/models/menu';

export const menuService = {
  getAll: async (): Promise<MenuItem[]> => {
    const response = await apiClient.get<MenuItem[]>('/library/menu');
    return response.data;
  },

  create: async (data: CreateMenuItemRequest): Promise<MenuItem> => {
    const response = await apiClient.post<MenuItem>('/library/menu', data);
    return response.data;
  },

  update: async (id: number, data: Omit<UpdateMenuItemRequest, 'id'>): Promise<MenuItem> => {
    const response = await apiClient.put<MenuItem>(`/library/menu/${id}`, data);
    return response.data;
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/menu/${id}`);
  },

  reorder: async (data: ReorderMenuItemsRequest): Promise<void> => {
      await apiClient.post('/library/menu/reorder', data);
    },

    resetToDefault: async (): Promise<void> => {
      await apiClient.post('/library/menu/reset-to-default', {});
    },
  };

