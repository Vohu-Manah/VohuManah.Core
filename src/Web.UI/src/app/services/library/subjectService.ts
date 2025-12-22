import { apiClient } from '../api/client';
import type { Subject, SubjectName, CreateSubjectRequest, UpdateSubjectRequest } from '../../models/subject';

export const subjectService = {
  getAll: async (): Promise<Subject[]> => {
    const response = await apiClient.get<Subject[]>('/library/subjects');
    return response.data;
  },

  getList: async (): Promise<Subject[]> => {
    const response = await apiClient.get<Subject[]>('/library/subjects/list');
    return response.data;
  },

  getById: async (id: number): Promise<Subject> => {
    const response = await apiClient.get<Subject>(`/library/subjects/${id}`);
    return response.data;
  },

  getNames: async (): Promise<SubjectName[]> => {
    const response = await apiClient.get<SubjectName[]>('/library/subjects/names');
    return response.data;
  },

  create: async (data: CreateSubjectRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/subjects', data);
    return response.data;
  },

  update: async (data: UpdateSubjectRequest): Promise<void> => {
    await apiClient.put('/library/subjects', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/subjects/${id}`);
  },
};

