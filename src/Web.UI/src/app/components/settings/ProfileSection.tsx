import { useState, useEffect } from 'react';
import { Card, Stack, Title, Group, Avatar, FileButton, Button, TextInput } from '@mantine/core';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { useForm } from '@mantine/form';
import { useAuth } from '@/app/context/AuthContext';
import { settingsService } from '@/app/services/library/settingsService';
import { attachmentService } from '@/app/services/library/attachmentService';
import { appMessage } from '@/app/messages/appMessages';
import { handleApiError } from '@/app/helpers/errorHandler';
import { useUserSettings } from '@/app/context/UserSettingsContext';
import type { UpdateUserSettingsRequest } from '@/app/models/settings';
import { getFullImageUrl } from '@/app/helpers/imageUrl';

export default function ProfileSection() {
  const { user } = useAuth();
  const { userSettings, loading: settingsLoading, reload } = useUserSettings();
  const [loading, setLoading] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [profileImageFile, setProfileImageFile] = useState<File | null>(null);
  const [profileImagePreview, setProfileImagePreview] = useState<string>('');

  const form = useForm({
    initialValues: {
      fullName: user?.fullName || '',
    },
  });

  useEffect(() => {
    if (userSettings?.profileImageUrl) {
      const fullUrl = getFullImageUrl(userSettings.profileImageUrl);

      setProfileImagePreview(fullUrl || '');
    } else if (!profileImageFile) {
      setProfileImagePreview('');
    }
  }, [userSettings, profileImageFile]);

  const handleProfileImageChange = (file: File | null) => {
    if (file) {
      setProfileImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setProfileImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    } else {
      setProfileImageFile(null);
      if (userSettings?.profileImageUrl) {
        const fullUrl = getFullImageUrl(userSettings.profileImageUrl);
        setProfileImagePreview(fullUrl || '');
      } else {
        setProfileImagePreview('');
      }
    }
  };

  const handleSaveProfile = async () => {
    try {
      setLoading(true);
      
      if (settingsLoading || !userSettings) {
        appMessage.warning('لطفاً صبر کنید تا تنظیمات کاربر بارگذاری شود...');
        return;
      }
      
      const updateData: UpdateUserSettingsRequest = {};
      
      if (profileImageFile) {
        if (!userSettings.id) {
          appMessage.error('خطا: تنظیمات کاربر یافت نشد. لطفاً صفحه را رفرش کنید.');
          return;
        }

        setUploading(true);
        try {
          
          
          
          const attachmentId = await attachmentService.upload(
            'User',
            userSettings.userId,
            profileImageFile,
            'Profile Image',
            true,
          );
          updateData.profileImageAttachmentId = attachmentId;
        } catch (error) {
          handleApiError(error, 'خطا در آپلود عکس');
          return;
        } finally {
          setUploading(false);
        }
      }

      await settingsService.updateUserSettings(updateData);
      appMessage.success('پروفایل با موفقیت به‌روزرسانی شد');
      
      await reload();
      setProfileImageFile(null);
    } catch (error) {
      handleApiError(error, 'خطا در ذخیره پروفایل');
    } finally {
      setLoading(false);
    }
  };

  const initials = user?.fullName?.split(' ')?.[0]?.[0] ?? 'U';

  return (
    <Card withBorder>
      <Stack gap="md">
        <Title order={4}>ویرایش پروفایل</Title>
        
        <Group justify="center">
          <Avatar src={profileImagePreview} size={120} radius="xl">
            {!profileImagePreview && initials}
          </Avatar>
        </Group>

        <Group justify="center">
          <FileButton onChange={handleProfileImageChange} accept="image/*">
            {(props) => <Button {...props} disabled={uploading}>انتخاب عکس</Button>}
          </FileButton>
        </Group>

        <SmartTextInput
          label="نام کامل"
          fieldName="fullName"
          value={form.values.fullName}
          disabled
        />

        <TextInput
          label="نام کاربری"
          value={user?.userName || ''}
          disabled
        />

        {(profileImageFile || userSettings?.profileImageUrl) && (
          <Group justify="flex-end">
            <Button onClick={handleSaveProfile} loading={loading || uploading}>
              ذخیره تغییرات
            </Button>
          </Group>
        )}
      </Stack>
    </Card>
  );
}
