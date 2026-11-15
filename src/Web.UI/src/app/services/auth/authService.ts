import { apiClient, rawClient } from '@/app/services/api/client';
import { setAuthMetadata, clearAuthMetadata } from '@/app/services/api/tokenStore';
import { toApiError } from '@/app/services/api/errors';

export type LoginRequest = {
  userName: string;
  password: string;
  humanVerifyToken?: string;
};

export type LoginResponse = {
  userName: string;
  fullName: string;
};

export async function login(payload: LoginRequest): Promise<LoginResponse> {
  try {
    const response = await rawClient.post<LoginResponse>('/library/users/login', payload);
    setAuthMetadata({
      userName: response.data.userName,
      fullName: response.data.fullName,
    });
    return response.data;
  } catch (error) {
    throw toApiError(error);
  }
}

export async function refresh(): Promise<LoginResponse> {
  try {
    const response = await rawClient.post<LoginResponse>('/library/users/refresh-token');
    return response.data;
  } catch (error) {
    clearAuthMetadata();
    throw toApiError(error);
  }
}

export async function revoke(): Promise<void> {
  try {
    await apiClient.post('/library/users/revoke-token');
  } finally {
    clearAuthMetadata();
  }
}
