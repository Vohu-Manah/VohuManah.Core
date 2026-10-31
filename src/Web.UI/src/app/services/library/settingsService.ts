import { apiClient } from '@/app/services/api/client';
import type { Setting, UpdateSettingRequest, UserSettings, AppInfo, SystemInfo, UpdateUserSettingsRequest } from '@/app/models/settings';

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

  getUserSettings: async (): Promise<UserSettings> => {
    const response = await apiClient.get<UserSettings>('/library/settings/user-settings');
    return response.data;
  },

  updateUserSettings: async (data: UpdateUserSettingsRequest): Promise<void> => {
    await apiClient.put('/library/settings/user-settings', data);
  },

  getAppInfo: async (): Promise<AppInfo> => {
    const response = await apiClient.get<AppInfo>('/library/settings/app-info');
    return response.data;
  },

  getSystemInfo: async (): Promise<SystemInfo> => {
    const response = await apiClient.get<SystemInfo>('/library/settings/system-info');
    return response.data;
  },
};

