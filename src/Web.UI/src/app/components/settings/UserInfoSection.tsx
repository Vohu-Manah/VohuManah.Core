import { useState, useEffect } from 'react';
import { Card, Stack, Title, Group, Button, TextInput } from '@mantine/core';
import { useForm } from '@mantine/form';
import { userService } from '@/app/services/library/userService';
import { appMessage } from '@/app/messages/appMessages';
import { handleApiError } from '@/app/helpers/errorHandler';
import { isNotEmpty } from '@mantine/form';

export default function UserInfoSection() {
  const [loading, setLoading] = useState(false);
  const [loadingData, setLoadingData] = useState(true);
  const [isEditing, setIsEditing] = useState(false);

  const form = useForm({
    initialValues: {
      userName: '',
      name: '',
      lastName: '',
      email: '',
      phoneNumber: '',
    },
    validate: {
      name: isNotEmpty('نام الزامی است'),
      lastName: isNotEmpty('نام خانوادگی الزامی است'),
      email: (value) => {
        if (value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
          return 'ایمیل معتبر نیست';
        }
        return null;
      },
    },
  });

  useEffect(() => {
    loadUserProfile();
  }, []);

  const loadUserProfile = async () => {
    try {
      setLoadingData(true);
      const profile = await userService.getCurrentProfile();
      form.setValues({
        userName: profile.userName,
        name: profile.name,
        lastName: profile.lastName,
        email: profile.email || '',
        phoneNumber: profile.phoneNumber || '',
      });
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری مشخصات کاربری');
    } finally {
      setLoadingData(false);
    }
  };

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleCancel = () => {
    setIsEditing(false);
    loadUserProfile();
  };

  const handleSave = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        await userService.updateProfile({
          name: form.values.name,
          lastName: form.values.lastName,
          email: form.values.email || null,
          phoneNumber: form.values.phoneNumber || null,
        });
        appMessage.success('مشخصات کاربری با موفقیت به‌روزرسانی شد');
        setIsEditing(false);
        await loadUserProfile();
      } catch (error) {
        handleApiError(error, 'خطا در به‌روزرسانی مشخصات');
      } finally {
        setLoading(false);
      }
    }
  };

  return (
    <Card withBorder>
      <Stack gap="md">
        <Title order={4}>مشخصات کاربری</Title>

        <TextInput
          label="نام کاربری"
          value={form.values.userName}
          disabled
        />

        <TextInput
          label="نام"
          withAsterisk
          disabled={!isEditing}
          {...form.getInputProps('name')}
        />

        <TextInput
          label="نام خانوادگی"
          withAsterisk
          disabled={!isEditing}
          {...form.getInputProps('lastName')}
        />

        <TextInput
          label="ایمیل"
          type="email"
          disabled={!isEditing}
          {...form.getInputProps('email')}
        />

        <TextInput
          label="شماره تلفن"
          disabled={!isEditing}
          {...form.getInputProps('phoneNumber')}
        />

        <Group justify="flex-end">
          {!isEditing ? (
            <Button onClick={handleEdit} loading={loadingData}>
              ویرایش
            </Button>
          ) : (
            <Group>
              <Button variant="default" onClick={handleCancel} disabled={loading}>
                انصراف
              </Button>
              <Button onClick={handleSave} loading={loading}>
                ثبت
              </Button>
            </Group>
          )}
        </Group>
      </Stack>
    </Card>
  );
}

