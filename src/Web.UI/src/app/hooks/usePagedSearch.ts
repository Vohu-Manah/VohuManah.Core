import { useCallback, useEffect, useRef, useState } from 'react';

import type { PagedResult } from '@/app/models/paged';

type SearchFn<T> = (params: { page?: number; pageSize?: number }) => Promise<PagedResult<T>>;


export function usePagedSearch<T>(searchFn: SearchFn<T>, pageSize = 50) {
  const [items, setItems] = useState<T[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  
  const bufferRef = useRef<Map<number, T[]>>(new Map());
  const pendingRef = useRef<Map<number, Promise<PagedResult<T>>>>(new Map());
  const lastParamsRef = useRef<Record<string, unknown>>({});

  const fetchPage = useCallback(
    async (targetPage: number): Promise<PagedResult<T>> => {
      const cached = bufferRef.current.get(targetPage);
      if (cached) {
        return { items: cached, totalCount, page: targetPage, pageSize };
      }

      const pending = pendingRef.current.get(targetPage);
      if (pending) {
        return pending;
      }

      const promise = searchFn({ ...lastParamsRef.current, page: targetPage, pageSize }).then((result) => {
        bufferRef.current.set(targetPage, result.items);
        pendingRef.current.delete(targetPage);
        return result;
      });

      pendingRef.current.set(targetPage, promise);
      return promise;
    },
    [searchFn, pageSize, totalCount],
  );

  const load = useCallback(
    async (params: Record<string, unknown>, reset = false) => {
      lastParamsRef.current = params;
      if (reset) {
        bufferRef.current.clear();
        pendingRef.current.clear();
        setPage(1);
      }

      setLoading(true);
      setError(null);

      try {
        const targetPage = reset ? 1 : page;
        const result = await fetchPage(targetPage);
        setItems(result.items);
        setTotalCount(result.totalCount);
        setPage(result.page ?? targetPage);

        
        const nextPage = (result.page ?? targetPage) + 1;
        if ((result.page ?? targetPage) * (result.pageSize ?? pageSize) < result.totalCount) {
          void fetchPage(nextPage);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'خطا در دریافت داده');
      } finally {
        setLoading(false);
      }
    },
    [fetchPage, page],
  );

  const goToPage = useCallback(
    async (targetPage: number) => {
      setLoading(true);
      setError(null);
      try {
        const result = await fetchPage(targetPage);
        setItems(result.items);
        setPage(result.page ?? targetPage);

        const nextPage = (result.page ?? targetPage) + 1;
        if ((result.page ?? targetPage) * (result.pageSize ?? pageSize) < result.totalCount) {
          void fetchPage(nextPage);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : 'خطا در دریافت داده');
      } finally {
        setLoading(false);
      }
    },
    [fetchPage],
  );

  useEffect(() => {
    return () => {
      bufferRef.current.clear();
      pendingRef.current.clear();
    };
  }, []);

  return {
    items,
    totalCount,
    page,
    pageSize,
    totalPages: Math.max(1, Math.ceil(totalCount / pageSize)),
    loading,
    error,
    load,
    goToPage,
  };
}
