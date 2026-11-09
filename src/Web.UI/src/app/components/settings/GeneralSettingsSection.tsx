import { useState, useEffect, useCallback } from 'react';
import { Card, Stack, Title, Group, Button, ColorInput, FileButton, Avatar } from '@mantine/core';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { useForm } from '@mantine/form';
import { settingsService } from '@/app/services/library/settingsService';
import type { Setting } from '@/app/models/settings';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { handleApiError } from '@/app/helpers/errorHandler';

export default function GeneralSettingsSection() {
  const [settings, setSettings] = useState<Setting[]>([]);
  const [loading, setLoading] = useState(false);
  const [primaryColor, setPrimaryColor] = useState('#228be6');
  const [logoFile, setLogoFile] = useState<File | null>(null);
  const [logoPreview, setLogoPreview] = useState<string>('');

  const form = useForm<{ mainTitle: string }>({
    initialValues: {
      mainTitle: '',
    },
    validate: {
      mainTitle: isNotEmpty('عنوان اصلی الزامی است'),
    },
  });

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const [settingsData, title] = await Promise.all([
        settingsService.getAll(),
        settingsService.getMainTitle(),
      ]);
      setSettings(settingsData);
      form.setValues({ mainTitle: title });
      const colorSetting = settingsData.find((s) => s.name === 'PrimaryColor');
      if (colorSetting) {
        setPrimaryColor(colorSetting.value);
      }
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری تنظیمات');
    } finally {
      setLoading(false);
    }
  }, [form]);

  useEffect(() => {

    void loadData();
  }, [loadData]);

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
        }
      } catch (error) {
        handleApiError(error, 'خطا در ذخیره تنظیمات');
      } finally {
        setLoading(false);
      }
    }
  };

  const handleSaveColor = async () => {
    try {
      setLoading(true);
      const colorSetting = settings.find((s) => s.name === 'PrimaryColor');
      if (colorSetting) {
        await settingsService.update({
          id: colorSetting.id,
          name: colorSetting.name,
          value: primaryColor,
        });
      } else {
        appMessage.warning('امکان ایجاد تنظیمات جدید از طریق UI وجود ندارد');
        return;
      }
      appMessage.success('رنگ اصلی با موفقیت به‌روزرسانی شد');
    } catch (error) {
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

  const handleSaveLogo = async () => {
    if (!logoFile) {
      appMessage.warning('لطفا فایل لوگو را انتخاب کنید');
      return;
    }
    appMessage.info('قابلیت آپلود لوگو به زودی اضافه خواهد شد');
  };

  return (
    <>
      <Card withBorder>
        <Stack gap="md">
          <Title order={4}>تنظیمات عمومی</Title>
          <SmartTextInput
            label="عنوان اصلی برنامه"
            withAsterisk
            fieldName="mainTitle"
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
          {logoPreview && <Avatar src={logoPreview} size={120} mx="auto" />}
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
    </>
  );
}

