import { appConfig } from '@/app/config/app-config';

export function getFullImageUrl(url: string | undefined | null): string | undefined {
  if (!url) {
    return undefined;
  }

  if (url.startsWith('http://') || url.startsWith('https://')) {
    return url;
  }

  const baseUrl = appConfig.apiBaseUrl.endsWith('/')
    ? appConfig.apiBaseUrl.slice(0, -1)
    : appConfig.apiBaseUrl;

  const relativeUrl = url.startsWith('/') ? url : `/${url}`;
  return `${baseUrl}${relativeUrl}`;
}

