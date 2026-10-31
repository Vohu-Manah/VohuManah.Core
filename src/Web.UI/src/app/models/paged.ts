
export type PagedList<T> = {
  items: T[];
  totalCount: number;
  page?: number;
  pageSize?: number;
};


export type ListQueryParams = {
  page?: number;
  pageSize?: number;
  year?: string;
  sortField?: string;
  sortOrder?: 'asc' | 'desc';
  [key: string]: unknown;
};


export type PagedResult<T> = PagedList<T>;
