import { apiClient, rawClient } from '../api/client';
import { setTokens, clearTokens, getTokens } from '../api/tokenStore';
import { toApiError } from '../api/errors';

export type LoginRequest = {
  userName: string;
  password: string;
};

export type LoginResponse = {
  userName: string;
  fullName: string;
  accessToken: string;
  refreshToken: string;
};

export async function login(payload: LoginRequest): Promise<LoginResponse> {
  try {
    const response = await rawClient.post<LoginResponse>('/library/users/login', payload);
    setTokens({
      accessToken: response.data.accessToken,
      refreshToken: response.data.refreshToken,
      userName: response.data.userName,
      fullName: response.data.fullName,
    });
    return response.data;
  } catch (error) {
    throw toApiError(error);
  }
}

export async function refresh(): Promise<LoginResponse> {
  const tokens = getTokens();
  if (!tokens?.refreshToken) {
    throw toApiError(new Error('Refresh token not found'));
  }

  try {
    const response = await rawClient.post<{ accessToken: string; refreshToken: string }>(
      '/library/users/refresh-token',
      { refreshToken: tokens.refreshToken },
    );

    const updated: LoginResponse = {
      userName: tokens.userName ?? '',
      fullName: tokens.fullName ?? '',
      accessToken: response.data.accessToken,
      refreshToken: response.data.refreshToken,
    };
    setTokens({
      ...tokens,
      accessToken: updated.accessToken,
      refreshToken: updated.refreshToken,
    });
    return updated;
  } catch (error) {
    clearTokens();
    throw toApiError(error);
  }
}

export async function revoke(): Promise<void> {
  const tokens = getTokens();
  if (!tokens?.refreshToken) {
    clearTokens();
    return;
  }

  try {
    await apiClient.post('/library/users/revoke-token', { refreshToken: tokens.refreshToken });
  } finally {
    clearTokens();
  }
}

