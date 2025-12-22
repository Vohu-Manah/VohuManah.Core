import { apiClient } from '../api/client';
import type { Language, LanguageName, CreateLanguageRequest, UpdateLanguageRequest } from '../../models/language';

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

  update: async (data: UpdateLanguageRequest): Promise<void> => {
    await apiClient.put('/library/languages', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/languages/${id}`);
  },
};

