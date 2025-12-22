import { apiClient } from '../api/client';
import type { Book, BookList, CreateBookRequest, UpdateBookRequest, BookSearchRequest, BookSearchResponse } from '../../models/book';

export const bookService = {
  getAll: async (): Promise<Book[]> => {
    const response = await apiClient.get<Book[]>('/library/books');
    return response.data;
  },

  getList: async (): Promise<BookList[]> => {
    const response = await apiClient.get<BookList[]>('/library/books/list');
    return response.data;
  },

  getById: async (id: number): Promise<Book> => {
    const response = await apiClient.get<Book>(`/library/books/${id}`);
    return response.data;
  },

  create: async (data: CreateBookRequest): Promise<number> => {
    const response = await apiClient.post<number>('/library/books', data);
    return response.data;
  },

  update: async (data: UpdateBookRequest): Promise<void> => {
    await apiClient.put('/library/books', data);
  },

  delete: async (id: number): Promise<void> => {
    await apiClient.delete(`/library/books/${id}`);
  },

  search: async (params: BookSearchRequest): Promise<BookSearchResponse[]> => {
    const response = await apiClient.post<BookSearchResponse[]>('/library/books/search', params);
    return response.data;
  },

  getStatistics: async (): Promise<any> => {
    const response = await apiClient.get('/library/books/statistics');
    return response.data;
  },
};

