import { AxiosError } from 'axios';
import type { ApiError, ProblemDetails } from '@/app/services/api/types';

function normalizeErrors(errors: unknown): Record<string, string[]> | undefined {
  if (!errors || typeof errors !== 'object') {
    return undefined;
  }

  if (Array.isArray(errors)) {
    const normalized: Record<string, string[]> = {};
    errors.forEach((error: unknown) => {
      if (error && typeof error === 'object') {
        const { code = '', description = '' } = error as { code?: string; description?: string };
        const camelCaseField = code.charAt(0).toLowerCase() + code.slice(1);

        if (camelCaseField && description) {
          if (!normalized[camelCaseField]) {
            normalized[camelCaseField] = [];
          }
          normalized[camelCaseField].push(description);
        }
      }
    });
    return Object.keys(normalized).length > 0 ? normalized : undefined;
  }

  const normalized: Record<string, string[]> = {};
  Object.entries(errors).forEach(([key, value]) => {
    if (Array.isArray(value)) {
      normalized[key] = value
        .map((item) => {
          if (typeof item === 'string') {
            return item;
          }
          if (typeof item === 'object' && item !== null) {
            if ('description' in item && typeof item.description === 'string') {
              return item.description;
            }
            return '';
          }
          return String(item);
        })
        .filter((msg) => msg.trim() !== '');
    } else if (typeof value === 'string') {
      normalized[key] = [value];
    } else if (value !== null && value !== undefined) {
      if (typeof value === 'object' && 'description' in value) {
        normalized[key] = [String(value.description)];
      } else {
        normalized[key] = [String(value)];
      }
    }
  });
  return Object.keys(normalized).length > 0 ? normalized : undefined;
}

export function toApiError(error: unknown): ApiError {
  if ((error as AxiosError)?.isAxiosError) {
    const axiosError = error as AxiosError<ProblemDetails>;
    const status = axiosError.response?.status ?? 0;
    const payload = axiosError.response?.data;

    if (!axiosError.response) {
      return {
        status: 0,
        message: 'خطا در بستر ارتباطی و شبکه دسترسی به سرویس\u200Cها ناموفق',
        raw: error,
      };
    }

    return {
      status,
      message: payload?.detail ?? payload?.errorMessage ?? payload?.title ?? axiosError.message,
      title: payload?.title,
      detail: payload?.detail,
      errors: normalizeErrors(payload?.errors),
      raw: payload,
    };
  }

  return {
    status: 0,
    message: (error as Error)?.message ?? 'خطای نامشخص',
    raw: error,
  };
}
