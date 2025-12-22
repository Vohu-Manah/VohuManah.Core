import { apiClient } from '../api/client';
import type { Role, CreateRoleRequest, UpdateRolePermissionsRequest, UserRoleResponse } from '../../models/role';

export const roleService = {
  getAll: async (): Promise<Role[]> => {
    const response = await apiClient.get<Role[]>('/library/settings/roles');
    return response.data;
  },

  create: async (data: CreateRoleRequest): Promise<Role> => {
    const response = await apiClient.post<Role>('/library/settings/roles', data);
    return response.data;
  },

  updatePermissions: async (data: UpdateRolePermissionsRequest): Promise<void> => {
    await apiClient.put(`/library/settings/roles/${data.roleId}/endpoints`, {
      endpointNames: data.endpointNames,
    });
  },

  getUserRoles: async (userId: number): Promise<UserRoleResponse[]> => {
    const response = await apiClient.get<UserRoleResponse[]>(`/library/settings/users/${userId}/roles`);
    return response.data;
  },

  getUserRoleIds: async (userId: number): Promise<number[]> => {
    const response = await apiClient.get<number[]>(`/library/settings/users/${userId}/role-ids`);
    return response.data;
  },

  getRolePermissions: async (roleId: number): Promise<string[]> => {
    const response = await apiClient.get<string[]>(`/library/settings/roles/${roleId}/permissions`);
    return response.data;
  },

  assignRoleToUser: async (userId: number, roleId: number): Promise<void> => {
    await apiClient.post(`/library/settings/users/${userId}/roles`, { roleId });
  },

  removeRoleFromUser: async (userId: number, roleId: number): Promise<void> => {
    await apiClient.delete(`/library/settings/users/${userId}/roles/${roleId}`);
  },
};

