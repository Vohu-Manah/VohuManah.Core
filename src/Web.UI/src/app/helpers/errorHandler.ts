import { appMessage } from '../messages/appMessages';
import type { ApiError } from '../services/api/types';

export function handleApiError(error: unknown, defaultMessage = 'خطا در انجام عملیات') {
  const apiError = error as ApiError;
  
  // اگر errors وجود داشت، تمام خطاها رو نمایش بده
  if (apiError.errors && Object.keys(apiError.errors).length > 0) {
    const errorMessages: string[] = [];
    
    Object.entries(apiError.errors).forEach(([field, messages]) => {
      if (Array.isArray(messages)) {
        messages.forEach((msg) => {
          errorMessages.push(`${field}: ${msg}`);
        });
      } else if (messages) {
        errorMessages.push(`${field}: ${messages}`);
      }
    });
    
    if (errorMessages.length > 0) {
      appMessage.error(defaultMessage, errorMessages.join('\n'));
      return;
    }
  }
  
  // اگر message وجود داشت، نمایش بده
  if (apiError.message) {
    appMessage.error(defaultMessage, apiError.message);
    return;
  }
  
  // در غیر این صورت پیام پیش‌فرض
  appMessage.error(defaultMessage);
}


