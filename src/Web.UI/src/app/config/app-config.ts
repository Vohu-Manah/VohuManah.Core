export type AppConfig = {
  apiBaseUrl: string;
  appName: string;
  defaultLocale: 'fa';
};

const apiBaseUrl =
  import.meta.env.VITE_API_BASE_URL?.toString() ?? 'http://localhost:5000';

export const appConfig: AppConfig = {
  apiBaseUrl,
  appName: 'VohuManah',
  defaultLocale: 'fa',
};

