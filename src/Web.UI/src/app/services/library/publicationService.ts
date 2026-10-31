import { apiClient } from '@/app/services/api/client';
import type { PagedList, ListQueryParams, PagedResult } from '@/app/models/paged';
import type { PublicationStatistics } from '@/app/models/statistics';
import type { Publication, CreatePublicationRequest, UpdatePublicationRequest, PublicationSearchRequest, PublicationSearchResponse } from '@/app/models/publication';

export const publicationService = {
  getAll: async (): Promise<Publication[]> => {
    const response = await apiClient.get<Publication[]>('/library/publications');
    return response.data;
  },

  async getList(params?: ListQueryParams): Promise<PagedList<Publication>> {
    const response = await apiClient.get<PagedList<Publication>>('/library/publications/list', { params });
    return response.data;
  },

  getById: async (id: number): Promise<Publication> => {
    const response = await apiClient.get<Publication>(`/library/publications/${id}`);
    return response.data;
  },

  create: async (data: CreatePublicationRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/publications', data);
    return response.data;
  },

  update: async (id: number, data: UpdatePublicationRequest): Promise<void> => {
    await apiClient.put(`/library/publications/${id}`, { ...data, id } as UpdatePublicationRequest);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/publications/${id}`);
  },

  search: async (
    params: PublicationSearchRequest & { page?: number; pageSize?: number },
  ): Promise<PagedResult<PublicationSearchResponse>> => {
    const { year, page, pageSize, ...rest } = params;
    const response = await apiClient.post<PagedResult<PublicationSearchResponse>>('/library/publications/search', {
      ...rest,
      fromYear: year || null,
      toYear: year || null,
      publishDate: rest.publishDate || null,
      page: page ?? 1,
      pageSize: pageSize ?? 50,
    });
    return response.data;
  },

  getStatistics: async (): Promise<PublicationStatistics> => {
    const response = await apiClient.get('/library/publications/statistics');
    return response.data;
  },
};

