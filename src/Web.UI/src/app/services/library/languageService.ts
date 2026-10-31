import { apiClient } from '@/app/services/api/client';
import type { Language, LanguageName, CreateLanguageRequest, UpdateLanguageRequest } from '@/app/models/language';

export const languageService = {
  getAll: async (): Promise<Language[]> => {
    const response = await apiClient.get<Language[]>('/library/languages');
    return response.data;
  },

  getList: async (): Promise<Language[]> => {
    const response = await apiClient.get<Language[]>('/library/languages/list');
    return response.data;
  },

  getById: async (id: number): Promise<Language> => {
    const response = await apiClient.get<Language>(`/library/languages/${id}`);
    return response.data;
  },

  getNames: async (): Promise<LanguageName[]> => {
    const response = await apiClient.get<LanguageName[]>('/library/languages/names');
    return response.data;
  },

  create: async (data: CreateLanguageRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/languages', data);
    return response.data;
  },

  update: async (id: number, data: UpdateLanguageRequest): Promise<void> => {
    await apiClient.put(`/library/languages/${id}`, { ...data, id } as UpdateLanguageRequest);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/languages/${id}`);
  },
};

