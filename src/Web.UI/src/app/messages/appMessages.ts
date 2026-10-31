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
    color:
      kind === 'success'
        ? 'green'
        : kind === 'info'
          ? 'blue'
          : kind === 'warning'
            ? 'yellow'
            : 'red',
    position: 'top-center',
    withBorder: true,
    autoClose: 5000,
    styles: {
      root: {
        direction: 'rtl',
        textAlign: 'right',
        padding: '16px',
        borderRadius: '12px',
        boxShadow: '0 4px 12px rgba(0, 0, 0, 0.15)',
        maxWidth: '500px',
        minHeight: 'auto',
        height: 'auto',
      },
      body: {
        whiteSpace: 'pre-line',
        padding: '0',
        maxHeight: '400px',
        overflowY: 'auto',
        overflowX: 'hidden',
      },
      title: {
        textAlign: 'right',
        fontWeight: 600,
        fontSize: '16px',
        marginBottom: description ? '8px' : '0',
        paddingBottom: '0',
        borderBottom: 'none !important',
      },
      description: {
        textAlign: 'right',
        whiteSpace: 'pre-line',
        fontSize: '12px',
        lineHeight: '1.6',
        padding: '8px 0',
        color: '#6b7280',
        borderTop: 'none !important',
        marginTop: '0',
      },
      closeButton: {
        color: 'inherit',
        opacity: 0.7,
        '&:hover': {
          opacity: 1,
        },
      },
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
