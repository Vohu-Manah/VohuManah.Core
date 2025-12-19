import axios from 'axios';
import type { AxiosInstance, AxiosRequestConfig, AxiosRequestHeaders } from 'axios';
import { appConfig } from '../../config/app-config';
import { toApiError } from './errors';
import { getTokens, setTokens, clearTokens } from './tokenStore';

type ExtendedConfig = AxiosRequestConfig & { skipAuthRefresh?: boolean };

const baseConfig = {
  baseURL: appConfig.apiBaseUrl,
  headers: {
    'Content-Type': 'application/json',
  },
} satisfies AxiosRequestConfig;

// raw client without interceptors (for login/refresh)
export const rawClient = axios.create(baseConfig);

// main client with interceptors
export const apiClient: AxiosInstance = axios.create(baseConfig);

apiClient.interceptors.request.use((config) => {
  const tokens = getTokens();
  if (tokens?.accessToken) {
    config.headers = {
      ...config.headers,
      Authorization: `Bearer ${tokens.accessToken}`,
    } as AxiosRequestHeaders;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const axiosError = toApiError(error);
    const originalConfig = (error.config ?? {}) as ExtendedConfig;

    const tokens = getTokens();
    if (
      error.response?.status === 401 &&
      tokens?.refreshToken &&
      !originalConfig.skipAuthRefresh
    ) {
      try {
        const refreshed = await rawClient.post<{ accessToken: string; refreshToken: string }>(
          '/library/users/refresh-token',
          { refreshToken: tokens.refreshToken },
          { skipAuthRefresh: true } as ExtendedConfig,
        );

        setTokens({
          ...tokens,
          accessToken: refreshed.data.accessToken,
          refreshToken: refreshed.data.refreshToken,
        });

        originalConfig.headers = {
          ...(originalConfig.headers ?? {}),
          Authorization: `Bearer ${refreshed.data.accessToken}`,
        };
        originalConfig.skipAuthRefresh = true;
        return apiClient.request(originalConfig);
      } catch (refreshError) {
        clearTokens();
        return Promise.reject(toApiError(refreshError));
      }
    }

    return Promise.reject(axiosError);
  },
);

