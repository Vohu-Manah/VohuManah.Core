import { apiClient } from '../api/client';
import type { Setting, UpdateSettingRequest } from '../../models/settings';

export const settingsService = {
  getAll: async (): Promise<Setting[]> => {
    const response = await apiClient.get<Setting[]>('/library/settings');
    return response.data;
  },

  getById: async (id: number): Promise<Setting> => {
    const response = await apiClient.get<Setting>(`/library/settings/${id}`);
    return response.data;
  },

  getByName: async (name: string): Promise<Setting> => {
    const response = await apiClient.get<Setting>(`/library/settings/name/${name}`);
    return response.data;
  },

  getMainTitle: async (): Promise<string> => {
    const response = await apiClient.get<string>('/library/settings/main-title');
    return response.data;
  },

  update: async (data: UpdateSettingRequest): Promise<void> => {
    await apiClient.put('/library/settings', data);
  },
};

