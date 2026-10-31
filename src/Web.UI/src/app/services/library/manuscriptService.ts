import { apiClient } from '@/app/services/api/client';
import type { PagedList, ListQueryParams, PagedResult } from '@/app/models/paged';
import type { ManuscriptStatistics } from '@/app/models/statistics';
import type { Manuscript, CreateManuscriptRequest, UpdateManuscriptRequest, ManuscriptSearchRequest, ManuscriptSearchResponse } from '@/app/models/manuscript';

export const manuscriptService = {
  getAll: async (): Promise<Manuscript[]> => {
    const response = await apiClient.get<Manuscript[]>('/library/manuscripts');
    return response.data;
  },

  async getList(params?: ListQueryParams): Promise<PagedList<Manuscript>> {
    const response = await apiClient.get<PagedList<Manuscript>>('/library/manuscripts/list', { params });
    return response.data;
  },

  getById: async (id: number): Promise<Manuscript> => {
    const response = await apiClient.get<Manuscript>(`/library/manuscripts/${id}`);
    return response.data;
  },

  create: async (data: CreateManuscriptRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/manuscripts', data);
    return response.data;
  },

  update: async (id: number, data: UpdateManuscriptRequest): Promise<void> => {
    await apiClient.put(`/library/manuscripts/${id}`, { ...data, id } as UpdateManuscriptRequest);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/manuscripts/${id}`);
  },

  search: async (
    params: ManuscriptSearchRequest & { page?: number; pageSize?: number },
  ): Promise<PagedResult<ManuscriptSearchResponse>> => {
    const { year, page, pageSize, ...rest } = params;
    const response = await apiClient.post<PagedResult<ManuscriptSearchResponse>>('/library/manuscripts/search', {
      ...rest,
      fromYear: year || null,
      toYear: year || null,
      page: page ?? 1,
      pageSize: pageSize ?? 50,
    });
    return response.data;
  },

  getStatistics: async (): Promise<ManuscriptStatistics> => {
    const response = await apiClient.get('/library/manuscripts/statistics');
    return response.data;
  },
};

