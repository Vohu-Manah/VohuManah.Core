import { apiClient } from '@/app/services/api/client';
import type { LibraryUser, CreateUserRequest, UpdateUserRequest } from '@/app/models/user';

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

  update: async (id: number, data: UpdateUserRequest): Promise<void> => {
    await apiClient.put(`/library/users/${id}`, data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/users/${id}`);
  },

  revoke: async (userName: string): Promise<void> => {
    await apiClient.post(`/library/users/${userName}/revoke`);
  },

  updateProfile: async (data: { name: string; lastName: string; email?: string | null; phoneNumber?: string | null }): Promise<void> => {
    await apiClient.put('/library/users/profile', data);
  },

  changePassword: async (data: { currentPassword: string; newPassword: string }): Promise<void> => {
    await apiClient.put('/library/users/change-password', data);
  },

  getCurrentProfile: async (): Promise<{ id: number; userName: string; name: string; lastName: string; fullName: string; email?: string; phoneNumber?: string }> => {
    const response = await apiClient.get<{ id: number; userName: string; name: string; lastName: string; fullName: string; email?: string; phoneNumber?: string }>('/library/users/profile');
    return response.data;
  },
};

