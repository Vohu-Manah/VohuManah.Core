import { notifications } from '@mantine/notifications';

type MessageKind = 'success' | 'error' | 'info' | 'warning';

const defaultMessages: Record<string, string> = {
  ok: 'عملیات با موفقیت انجام شد',
  unauthorized: 'دسترسی ندارید',
  invalidCredentials: 'نام کاربری یا رمز عبور اشتباه است',
};

function show(kind: MessageKind, codeOrMessage: string, description?: string) {
  const title = defaultMessages[codeOrMessage] ?? codeOrMessage;
  notifications.show({
    title,
    message: description,
    color: kind === 'success' ? 'green' : kind === 'info' ? 'blue' : kind === 'warning' ? 'yellow' : 'red',
    position: 'top-center',
    withBorder: true,
    styles: {
      root: { direction: 'rtl', textAlign: 'center' },
    },
  });
}

export const appMessage = Object.assign(
  (code: string, description?: string) => show('info', code, description),
  {
    success: (code = 'ok', description?: string) => show('success', code, description),
    error: (code = 'خطا', description?: string) => show('error', code, description),
    warning: (code: string, description?: string) => show('warning', code, description),
    info: (code: string, description?: string) => show('info', code, description),
  },
);

