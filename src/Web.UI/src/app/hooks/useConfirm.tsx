import { useState, useCallback } from 'react';
import { Modal, Button, Stack, Text, Group } from '@mantine/core';

type ConfirmOptions = {
  title?: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  color?: 'red' | 'blue' | 'green' | 'yellow' | 'grape' | 'gray' | 'teal' | 'orange' | 'cyan' | 'pink' | 'violet' | 'indigo' | 'lime' | 'dark';
  loading?: boolean;
};

type ConfirmState = {
  options: ConfirmOptions;
  resolve: (value: boolean) => void;
};

export function useConfirm() {
  const [state, setState] = useState<ConfirmState | null>(null);

  const confirm = useCallback((options: ConfirmOptions | string): Promise<boolean> => {
    const opts: ConfirmOptions = typeof options === 'string' ? { message: options } : options;
    return new Promise<boolean>((resolve) => {
      setState({ options: opts, resolve });
    });
  }, []);

  const handleClose = useCallback((result: boolean) => {
    setState((current) => {
      current?.resolve(result);
      return null;
    });
  }, []);

  const element = (
    <Modal
      opened={state !== null}
      onClose={() => handleClose(false)}
      title={state?.options.title ?? 'تأیید'}
      centered
      dir="rtl"
      size="md"
    >
      <Stack gap="lg">
        <Text>{state?.options.message}</Text>
        <Group justify="flex-end" gap="sm">
          <Button variant="subtle" onClick={() => handleClose(false)}>
            {state?.options.cancelLabel ?? 'انصراف'}
          </Button>
          <Button
            color={state?.options.color ?? 'red'}
            loading={state?.options.loading}
            onClick={() => handleClose(true)}
          >
            {state?.options.confirmLabel ?? 'تأیید'}
          </Button>
        </Group>
      </Stack>
    </Modal>
  );

  return { confirm, element };
}
