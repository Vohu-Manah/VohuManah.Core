import { appMessage } from '@/app/messages/appMessages';
import type { ApiError } from '@/app/services/api/types';

function extractDescription(value: unknown): string {
  if (value === null || value === undefined) {
    return '';
  }
  if (typeof value === 'string') {
    return value;
  }
  if (typeof value === 'object') {
    if ('description' in value && typeof value.description === 'string') {
      return value.description;
    }
    return '';
  }
  return String(value);
}

export function getErrorMessage(error: unknown, fallback = 'خطای نامشخص'): string {
  if (error && typeof error === 'object') {
    const { message, detail } = error as { message?: unknown; detail?: unknown };
    if (typeof message === 'string' && message.trim()) {
      return message;
    }
    if (typeof detail === 'string' && detail.trim()) {
      return detail;
    }
  }
  return fallback;
}

export function handleApiError(error: unknown, defaultMessage = 'خطا در انجام عملیات') {
  const apiError = error as ApiError;

  if (
    apiError.status === 0 ||
    apiError.message === 'خطا در بستر ارتباطی و شبکه دسترسی به سرویس ها ناموفق'
  ) {
    appMessage.error('خطا در بستر ارتباطی و شبکه دسترسی به سرویس ها ناموفق');
    return;
  }

  if (apiError.errors && Object.keys(apiError.errors).length > 0) {
    const errorMessages: string[] = [];

    Object.entries(apiError.errors).forEach(([, messages]) => {
      if (Array.isArray(messages)) {
        messages.forEach((msg) => {
          const description = extractDescription(msg);
          if (description && description.trim()) {
            errorMessages.push(description);
          }
        });
      } else if (messages) {
        const description = extractDescription(messages);
        if (description && description.trim()) {
          errorMessages.push(description);
        }
      }
    });

    if (errorMessages.length > 0) {
      errorMessages.forEach((errorMsg, index) => {
        setTimeout(() => {
          const fullMessage =
            apiError.title && apiError.title.trim() ? `${apiError.title}\n${errorMsg}` : errorMsg;
          appMessage.error(defaultMessage, fullMessage);
        }, index * 100);
      });
      return;
    }
  }

  if (
    apiError.message &&
    apiError.message.trim() &&
    apiError.message !== 'یک یا چند خطای اعتبارسنجی رخ داده است'
  ) {
    const fullMessage =
      apiError.title && apiError.title.trim()
        ? `${apiError.title}\n${apiError.message}`
        : apiError.message;
    appMessage.error(defaultMessage, fullMessage);
    return;
  }

  if (
    apiError.detail &&
    apiError.detail.trim() &&
    apiError.detail !== 'یک یا چند خطای اعتبارسنجی رخ داده است'
  ) {
    const fullDetail =
      apiError.title && apiError.title.trim()
        ? `${apiError.title}\n${apiError.detail}`
        : apiError.detail;
    appMessage.error(defaultMessage, fullDetail);
    return;
  }

  appMessage.error(defaultMessage);
}
