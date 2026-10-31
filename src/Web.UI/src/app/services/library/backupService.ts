import { apiClient } from '@/app/services/api/client';

export type BackupInfo = {
  exists: boolean;
  fileName: string;
  takenAt: string;
  takenTime: string;
  sizeBytes: number;
  isRunning: boolean;
  percent: number;
  stage: string;
  error?: string | null;
  
  log?: string[];
};

export type BackupRestoreSource = 'Uploaded' | 'DailyFile';

export const backupService = {
  getInfo: async (): Promise<BackupInfo> => {
    const response = await apiClient.get<BackupInfo>('/library/backup');
    return response.data;
  },

  create: async (): Promise<void> => {
    await apiClient.post('/library/backup', {});
  },

  
  cancel: async (): Promise<boolean> => {
    const response = await apiClient.post<{ cancelled: boolean }>('/library/backup/cancel', {});
    return response.data.cancelled;
  },

  restoreDaily: async (): Promise<void> => {
    await apiClient.post('/library/backup/restore-daily', {});
  },

  restoreUpload: async (file: File): Promise<void> => {
    const form = new FormData();
    form.append('file', file);
    await apiClient.post('/library/backup/restore', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },

  download: async (fileName?: string): Promise<Blob> => {
    const params = fileName ? { fileName } : {};
    const response = await apiClient.get<Blob>('/library/backup/download', {
      params,
      responseType: 'blob',
    });
    return response.data;
  },
};
