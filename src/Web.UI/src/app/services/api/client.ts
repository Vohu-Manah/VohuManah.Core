import axios, { AxiosHeaders } from 'axios';
import type { AxiosInstance, AxiosRequestConfig } from 'axios';
import { appConfig } from '@/app/config/app-config';
import { toApiError } from '@/app/services/api/errors';
import { clearAuthMetadata } from '@/app/services/api/tokenStore';
import { handleApiError } from '@/app/helpers/errorHandler';

type ExtendedConfig = AxiosRequestConfig & {
  skipAuthRefresh?: boolean;
  skipErrorHandling?: boolean;
};

const baseConfig = {
  baseURL: appConfig.apiBaseUrl,
  withCredentials: true,
  xsrfCookieName: 'XSRF-TOKEN',
  xsrfHeaderName: 'X-XSRF-TOKEN',
  headers: {
    'Content-Type': 'application/json',
  },
} satisfies AxiosRequestConfig;

export const rawClient = axios.create(baseConfig);

export const apiClient: AxiosInstance = axios.create(baseConfig);

apiClient.interceptors.request.use((config) => {
  if (config.data instanceof FormData) {
    const headers = AxiosHeaders.from(config.headers);
    headers.delete('Content-Type');
    config.headers = headers;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const axiosError = toApiError(error);
    const originalConfig = (error.config ?? {}) as ExtendedConfig;

    if (error.response?.status === 401 && !originalConfig.skipAuthRefresh) {
      try {
        await rawClient.post('/library/users/refresh-token', null, { skipAuthRefresh: true } as ExtendedConfig);
        originalConfig.skipAuthRefresh = true;
        return apiClient.request(originalConfig);
      } catch (refreshError) {
        clearAuthMetadata();
        const refreshApiError = toApiError(refreshError);
        handleApiError(refreshApiError, 'خطا در احراز هویت');
        if (typeof window !== 'undefined') {
          window.location.hash = '#/login';
        }
        return Promise.reject(refreshApiError);
      }
    }

    if (!originalConfig.skipErrorHandling) {
      handleApiError(axiosError, 'خطا در انجام عملیات');
    }

    return Promise.reject(axiosError);
  },
);

export async function fetchAllPaged<T>(
  fetchPage: (page: number, pageSize: number) => Promise<{ items?: unknown; totalCount?: number }>,
  opts: { pageSize?: number; cap?: number; onProgress?: (done: number, total: number) => void } = {},
): Promise<T[]> {
  const { pageSize = 500, cap = 50000, onProgress } = opts;
  const out: T[] = [];
  let page = 1;
  let target = cap;
  for (;;) {
    const res = await fetchPage(page, pageSize);
    const raw = res?.items;
    const items = (Array.isArray(raw) ? raw : []) as T[];
    out.push(...items);
    if (page === 1 && typeof res?.totalCount === 'number' && res.totalCount > 0) {
      target = Math.min(res.totalCount, cap);
    }
    onProgress?.(out.length, target);
    if (items.length < pageSize || out.length >= cap) break;
    page += 1;
  }
  return out.slice(0, cap);
}
