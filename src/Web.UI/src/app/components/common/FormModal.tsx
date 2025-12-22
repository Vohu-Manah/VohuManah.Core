import { Modal, Button, Stack, Group } from '@mantine/core';
import type { ReactNode } from 'react';

type FormModalProps = {
  opened: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode;
  onSubmit?: () => void;
  submitLabel?: string;
  cancelLabel?: string;
  loading?: boolean;
};

export function FormModal({
  opened,
  onClose,
  title,
  children,
  onSubmit,
  submitLabel = 'ذخیره',
  cancelLabel = 'انصراف',
  loading = false,
}: FormModalProps) {
  return (
    <Modal
      opened={opened}
      onClose={onClose}
      title={title}
      size="lg"
      centered
      dir="rtl"
    >
      <form
        onSubmit={(e) => {
          e.preventDefault();
          onSubmit?.();
        }}
      >
        <Stack gap="md">
          {children}
          <Group justify="flex-end" mt="md">
            <Button variant="subtle" onClick={onClose} disabled={loading}>
              {cancelLabel}
            </Button>
            {onSubmit && (
              <Button type="submit" loading={loading}>
                {submitLabel}
              </Button>
            )}
          </Group>
        </Stack>
      </form>
    </Modal>
  );
}


