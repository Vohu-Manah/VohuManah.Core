import { apiClient } from '../api/client';
import type { LibraryUser, CreateUserRequest, UpdateUserRequest } from '../../models/user';

export const userService = {
  getAll: async (): Promise<LibraryUser[]> => {
    const response = await apiClient.get<LibraryUser[]>('/library/users');
    return response.data;
  },

  getList: async (): Promise<LibraryUser[]> => {
    const response = await apiClient.get<LibraryUser[]>('/library/users/list');
    return response.data;
  },

  getByUserName: async (userName: string): Promise<LibraryUser> => {
    const response = await apiClient.get<LibraryUser>(`/library/users/${userName}`);
    return response.data;
  },

  create: async (data: CreateUserRequest): Promise<void> => {
    await apiClient.post('/library/users', data);
  },

  update: async (data: UpdateUserRequest): Promise<void> => {
    await apiClient.put('/library/users', data);
  },

  delete: async (userName: string): Promise<void> => {
    await apiClient.delete(`/library/users/${userName}`);
  },

  revoke: async (userName: string): Promise<void> => {
    await apiClient.post(`/library/users/${userName}/revoke`);
  },
};

