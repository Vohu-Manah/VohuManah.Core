import { AxiosError } from 'axios';
import type { ApiError, ProblemDetails } from './types';

export function toApiError(error: unknown): ApiError {
  if ((error as AxiosError)?.isAxiosError) {
    const axiosError = error as AxiosError<ProblemDetails>;
    const status = axiosError.response?.status ?? 0;
    const payload = axiosError.response?.data;
    return {
      status,
      message: payload?.errorMessage ?? payload?.title ?? axiosError.message,
      detail: payload?.detail,
      errors: payload?.errors,
      raw: payload,
    };
  }

  return {
    status: 0,
    message: (error as Error)?.message ?? 'خطای نامشخص',
    raw: error,
  };
}

