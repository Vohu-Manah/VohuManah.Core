export type Setting = {
  id: number;
  name: string;
  value: string;
};

export type UpdateSettingRequest = {
  id?: number;
  name: string;
  value: string;
};

export type UserSettings = {
  id: number;
  userId: number;
  profileImageAttachmentId?: number;
  profileImageUrl?: string;
  themePrimaryColor?: string;
  themeSecondaryColor?: string;
  themeAccentColor?: string;
  themeMode?: string;
};

export type UpdateUserSettingsRequest = {
  profileImageAttachmentId?: number;
  themePrimaryColor?: string;
  themeSecondaryColor?: string;
  themeAccentColor?: string;
};

export type AppInfo = {
  id: number;
  appName: string;
  version: string;
  author: string;
  description?: string;
  lastUpdateDate?: string;
  copyright?: string;
  license?: string;
  website?: string;
  supportEmail?: string;
};

export type SystemInfo = {
  operatingSystem: string;
  frameworkVersion: string;
  machineName: string;
  processorCount: string;
  totalMemory: string;
  availableMemory: string;
  serverTime: string;
  timeZone: string;
};


