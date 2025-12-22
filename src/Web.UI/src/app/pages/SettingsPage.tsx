import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, TextInput, ColorInput, FileButton, Avatar, Text } from '@mantine/core';
import { useForm } from '@mantine/form';
import { settingsService } from '../services/library/settingsService';
import type { Setting, UpdateSettingRequest } from '../models/settings';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { handleApiError } from '../helpers/errorHandler';
import { useAuth } from '../context/AuthContext';

export default function SettingsPage() {
  const { user } = useAuth();
  const [settings, setSettings] = useState<Setting[]>([]);
  const [loading, setLoading] = useState(false);
  const [mainTitle, setMainTitle] = useState('');
  const [primaryColor, setPrimaryColor] = useState('#228be6');
  const [logoFile, setLogoFile] = useState<File | null>(null);
  const [logoPreview, setLogoPreview] = useState<string>('');
  const [profileImageFile, setProfileImageFile] = useState<File | null>(null);
  const [profileImagePreview, setProfileImagePreview] = useState<string>('');

  const form = useForm<{ mainTitle: string }>({
    initialValues: {
      mainTitle: '',
    },
    validate: {
      mainTitle: isNotEmpty('عنوان اصلی الزامی است'),
    },
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [settingsData, title] = await Promise.all([
        settingsService.getAll(),
        settingsService.getMainTitle(),
      ]);
      setSettings(settingsData);
      setMainTitle(title);
      form.setValues({ mainTitle: title });
      
      // Load color setting
      const colorSetting = settingsData.find((s) => s.name === 'PrimaryColor');
      if (colorSetting) {
        setPrimaryColor(colorSetting.value);
      }
    } catch (error: any) {
      handleApiError(error, 'خطا در بارگذاری تنظیمات');
    } finally {
      setLoading(false);
    }
  };

  const handleSaveMainTitle = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        const setting = settings.find((s) => s.name === 'ApplicationMainTitle');
        if (setting) {
          await settingsService.update({
            id: setting.id,
            name: setting.name,
            value: form.values.mainTitle,
          });
          appMessage.success('عنوان اصلی با موفقیت به‌روزرسانی شد');
          setMainTitle(form.values.mainTitle);
        }
      } catch (error: any) {
        handleApiError(error, 'خطا در ذخیره تنظیمات');
      } finally {
        setLoading(false);
      }
    }
  };

  const handleSaveColor = async () => {
    try {
      setLoading(true);
      let colorSetting = settings.find((s) => s.name === 'PrimaryColor');
      if (colorSetting) {
        await settingsService.update({
          id: colorSetting.id,
          name: colorSetting.name,
          value: primaryColor,
        });
      } else {
        // Create new setting if doesn't exist
        // Note: You may need to add a create method to settingsService
        appMessage.warning('امکان ایجاد تنظیمات جدید از طریق UI وجود ندارد');
        return;
      }
      appMessage.success('رنگ اصلی با موفقیت به‌روزرسانی شد');
    } catch (error: any) {
      handleApiError(error, 'خطا در ذخیره رنگ');
    } finally {
      setLoading(false);
    }
  };

  const handleLogoChange = (file: File | null) => {
    if (file) {
      setLogoFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setLogoPreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleProfileImageChange = (file: File | null) => {
    if (file) {
      setProfileImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setProfileImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSaveLogo = async () => {
    if (!logoFile) {
      appMessage.warning('لطفا فایل لوگو را انتخاب کنید');
      return;
    }
    // TODO: Implement logo upload API
    appMessage.info('قابلیت آپلود لوگو به زودی اضافه خواهد شد');
  };

  const handleSaveProfileImage = async () => {
    if (!profileImageFile) {
      appMessage.warning('لطفا عکس پروفایل را انتخاب کنید');
      return;
    }
    // TODO: Implement profile image upload API
    appMessage.info('قابلیت آپلود عکس پروفایل به زودی اضافه خواهد شد');
  };

  return (
    <Stack p="md">
      <Title order={2}>تنظیمات</Title>

      <Card withBorder>
        <Stack gap="md">
          <Title order={4}>تنظیمات عمومی</Title>
          <TextInput
            label="عنوان اصلی برنامه"
            withAsterisk
            {...form.getInputProps('mainTitle')}
          />
          <Group justify="flex-end">
            <Button onClick={handleSaveMainTitle} loading={loading}>
              ذخیره عنوان
            </Button>
          </Group>
        </Stack>
      </Card>

      <Card withBorder>
        <Stack gap="md">
          <Title order={4}>رنگ اصلی برنامه</Title>
          <ColorInput
            label="رنگ اصلی"
            value={primaryColor}
            onChange={setPrimaryColor}
            format="hex"
          />
          <Group justify="flex-end">
            <Button onClick={handleSaveColor} loading={loading}>
              ذخیره رنگ
            </Button>
          </Group>
        </Stack>
      </Card>

      <Card withBorder>
        <Stack gap="md">
          <Title order={4}>لوگو برنامه</Title>
          {logoPreview && (
            <Avatar src={logoPreview} size={120} mx="auto" />
          )}
          <Group justify="center">
            <FileButton onChange={handleLogoChange} accept="image/*">
              {(props) => <Button {...props}>انتخاب لوگو</Button>}
            </FileButton>
            {logoFile && (
              <Button onClick={handleSaveLogo} loading={loading}>
                ذخیره لوگو
              </Button>
            )}
          </Group>
        </Stack>
      </Card>

      <Card withBorder>
        <Stack gap="md">
          <Title order={4}>عکس پروفایل</Title>
          {profileImagePreview && (
            <Avatar src={profileImagePreview} size={120} mx="auto" />
          )}
          <Group justify="center">
            <FileButton onChange={handleProfileImageChange} accept="image/*">
              {(props) => <Button {...props}>انتخاب عکس</Button>}
            </FileButton>
            {profileImageFile && (
              <Button onClick={handleSaveProfileImage} loading={loading}>
                ذخیره عکس
              </Button>
            )}
          </Group>
        </Stack>
      </Card>
    </Stack>
  );
}
