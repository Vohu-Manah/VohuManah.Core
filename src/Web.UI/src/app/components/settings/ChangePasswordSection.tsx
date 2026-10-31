import { useState } from 'react';
import { Card, Stack, Title, Group, Button, PasswordInput, Text } from '@mantine/core';
import { useForm } from '@mantine/form';
import { userService } from '@/app/services/library/userService';
import { appMessage } from '@/app/messages/appMessages';
import { handleApiError } from '@/app/helpers/errorHandler';
import { isNotEmpty } from '@mantine/form';

const validatePassword = (value: string): string | null => {
  if (!value) {
    return 'رمز عبور جدید الزامی است';
  }
  if (value.length < 8) {
    return 'رمز عبور باید حداقل ۸ کاراکتر باشد';
  }
  if (value.length > 50) {
    return 'رمز عبور باید حداکثر ۵۰ کاراکتر باشد';
  }
  const hasUpper = /[A-Z]/.test(value);
  const hasLower = /[a-z]/.test(value);
  const hasDigit = /\d/.test(value);
  const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(value);

  if (!hasUpper || !hasLower || !hasDigit || !hasSpecial) {
    return 'رمز عبور باید شامل حروف بزرگ، حروف کوچک، عدد و کاراکتر خاص باشد';
  }
  return null;
};

export default function ChangePasswordSection() {
  const [loading, setLoading] = useState(false);

  const form = useForm({
    initialValues: {
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    },
    validate: {
      currentPassword: isNotEmpty('رمز عبور فعلی الزامی است'),
      newPassword: validatePassword,
      confirmPassword: (value, values) =>
        value !== values.newPassword ? 'رمز عبور جدید و تکرار آن باید یکسان باشند' : null,
    },
  });

  const handleChangePassword = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        await userService.changePassword({
          currentPassword: form.values.currentPassword,
          newPassword: form.values.newPassword,
        });
        appMessage.success('رمز عبور با موفقیت تغییر یافت');
        form.reset();
      } catch (error) {
        handleApiError(error, 'خطا در تغییر رمز عبور');
      } finally {
        setLoading(false);
      }
    }
  };

  return (
    <Card withBorder>
      <Stack gap="md">
        <Title order={4}>تغییر رمز عبور</Title>

        <PasswordInput
          label="رمز عبور فعلی"
          withAsterisk
          dir="ltr"
          styles={{
            input: {
              textAlign: 'left',
              direction: 'ltr',
            },
          }}
          {...form.getInputProps('currentPassword')}
        />

        <PasswordInput
          label="رمز عبور جدید"
          withAsterisk
          dir="ltr"
          styles={{
            input: {
              textAlign: 'left',
              direction: 'ltr',
            },
          }}
          {...form.getInputProps('newPassword')}
        />

        <PasswordInput
          label="تکرار رمز عبور جدید"
          withAsterisk
          dir="ltr"
          styles={{
            input: {
              textAlign: 'left',
              direction: 'ltr',
            },
          }}
          {...form.getInputProps('confirmPassword')}
        />

        <Text size="xs" c="dimmed">
          رمز عبور باید شامل حروف بزرگ، حروف کوچک، عدد و کاراکتر خاص باشد و حداقل ۸ کاراکتر باشد
        </Text>

        <Group justify="flex-end">
          <Button onClick={handleChangePassword} loading={loading}>
            تغییر رمز عبور
          </Button>
        </Group>
      </Stack>
    </Card>
  );
}

