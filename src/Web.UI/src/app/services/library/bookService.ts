import { apiClient } from '@/app/services/api/client';
import type { PagedList, ListQueryParams, PagedResult } from '@/app/models/paged';
import type { BookStatistics } from '@/app/models/statistics';
import type { Book, BookList, CreateBookRequest, UpdateBookRequest, BookSearchRequest, BookSearchResponse, BookProfile, BookBarcode } from '@/app/models/book';

export const bookService = {
  getAll: async (): Promise<Book[]> => {
    const response = await apiClient.get<Book[]>('/library/books');
    return response.data;
  },

  
  getBarcodes: async (
    afterId: number,
    take: number,
    filters?: { year?: string; search?: string },
  ): Promise<BookBarcode[]> => {
    const response = await apiClient.get<BookBarcode[]>('/library/books/barcodes', {
      params: { afterId, take, ...filters },
    });
    return response.data;
  },

  async getList(params?: ListQueryParams): Promise<PagedList<BookList>> {
    const response = await apiClient.get<PagedList<BookList>>('/library/books/list', { params });
    return response.data;
  },

  getById: async (id: number): Promise<Book> => {
    const response = await apiClient.get<Book>(`/library/books/${id}`);
    return response.data;
  },

  getProfile: async (id: number): Promise<BookProfile> => {
    const response = await apiClient.get<BookProfile>(`/library/books/${id}/profile`);
    return response.data;
  },

  create: async (data: CreateBookRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/books', data);
    return response.data;
  },

  update: async (id: number, data: UpdateBookRequest): Promise<void> => {
    await apiClient.put(`/library/books/${id}`, { ...data, id } as UpdateBookRequest);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/books/${id}`);
  },

  search: async (
    params: BookSearchRequest & { page?: number; pageSize?: number },
  ): Promise<PagedResult<BookSearchResponse>> => {
    const { publishYear, page, pageSize, ...rest } = params;
    const response = await apiClient.post<PagedResult<BookSearchResponse>>('/library/books/search', {
      ...rest,
      fromPublishYear: params.fromPublishYear ?? publishYear ?? null,
      toPublishYear: params.toPublishYear ?? publishYear ?? null,
      fromVolumeCount: params.fromVolumeCount ?? 0,
      toVolumeCount: params.toVolumeCount ?? 0,
      page: page ?? 1,
      pageSize: pageSize ?? 50,
    });
    return response.data;
  },

  getStatistics: async (): Promise<BookStatistics> => {
    const response = await apiClient.get('/library/books/statistics');
    return response.data;
  },
};

