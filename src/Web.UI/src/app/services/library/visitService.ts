import { apiClient } from '@/app/services/api/client';

export type VisitStatItem = {
  date: string;
  count: number;
};

export type VisitStats = {
  items: VisitStatItem[];
};

export const visitService = {
  
  record: async (path: string): Promise<void> => {
    try {
      await apiClient.post('/library/visits', { path });
    } catch {
      
    }
  },

  getStats: async (days = 30): Promise<VisitStats> => {
    const response = await apiClient.get<VisitStats>('/library/visits/stats', {
      params: { days },
    });
    return response.data;
  },
};
