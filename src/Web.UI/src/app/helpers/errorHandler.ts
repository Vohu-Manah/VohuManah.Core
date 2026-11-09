import { appMessage } from '@/app/messages/appMessages';
import type { ApiError } from '@/app/services/api/types';

const STATUS_MESSAGES: Record<number, string> = {
  400: 'درخواست نامعتبر است',
  401: 'احراز هویت شما ناموفق بود',
  403: 'شما مجاز به دسترسی به این بخش نیستید',
  404: 'مورد درخواستی یافت نشد',
  405: 'روش درخواست مجاز نیست',
  409: 'تعارض در داده‌ها رخ داد',
  415: 'نوع داده ارسال‌شده پشتیبانی نمی‌شود',
  422: 'داده‌های ارسال‌شده نامعتبر هستند',
  500: 'خطای سرور رخ داد',
  502: 'درگاه نامعتبر است',
  503: 'سرویس در دسترس نیست',
  504: 'مهلت پاسخ‌گویی سرویس به پایان رسید',
};

const AXIOS_GENERIC = /^Request failed with status code \d+$/;

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

export function getStatusMessage(status: number | undefined): string | null {
  return typeof status === 'number' && STATUS_MESSAGES[status] ? STATUS_MESSAGES[status] : null;
}

export function getErrorMessage(error: unknown, fallback = 'خطای نامشخص'): string {
  if (error && typeof error === 'object') {
    const err = error as { message?: unknown; detail?: unknown; title?: unknown; status?: unknown };

    
    if (typeof err.detail === 'string' && err.detail.trim()) {
      return err.detail;
    }

    
    const message = typeof err.message === 'string' ? err.message.trim() : '';
    if (message && !AXIOS_GENERIC.test(message)) {
      return message;
    }

    
    const status = typeof err.status === 'number' ? err.status : undefined;
    const statusMessage = getStatusMessage(status);
    if (statusMessage) {
      return statusMessage;
    }

    
    if (typeof err.title === 'string' && err.title.trim()) {
      return err.title;
    }
    if (message) {
      return message;
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

  const specificMessage = apiError.detail?.trim() || apiError.message?.trim();

  if (
    specificMessage &&
    specificMessage !== 'یک یا چند خطای اعتبارسنجی رخ داده است' &&
    !AXIOS_GENERIC.test(specificMessage)
  ) {
    const fullMessage =
      apiError.title && apiError.title.trim()
        ? `${apiError.title}\n${specificMessage}`
        : specificMessage;
    appMessage.error(defaultMessage, fullMessage);
    return;
  }

  const statusMessage = getStatusMessage(apiError.status);
  if (statusMessage) {
    appMessage.error(defaultMessage, statusMessage);
    return;
  }

  appMessage.error(defaultMessage);
}
