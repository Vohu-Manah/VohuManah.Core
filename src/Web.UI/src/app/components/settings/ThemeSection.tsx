import { useState, useEffect } from 'react';
import { Card, Stack, Title, Group, ColorPicker, Button, Text, TextInput } from '@mantine/core';
import { settingsService } from '@/app/services/library/settingsService';
import { appMessage } from '@/app/messages/appMessages';
import { handleApiError } from '@/app/helpers/errorHandler';
import { useUserSettings } from '@/app/context/UserSettingsContext';
import { applyThemeColors as applyTheme, DEFAULT_THEME } from '@/app/helpers/themeColors';
import type { UpdateUserSettingsRequest } from '@/app/models/settings';

export default function ThemeSection() {
  const { userSettings, reload } = useUserSettings();
  const [loading, setLoading] = useState(false);
  const [primaryColor, setPrimaryColor] = useState('#228be6');
  const [secondaryColor, setSecondaryColor] = useState('#868e96');
  const [accentColor, setAccentColor] = useState('#fd7e14');

  useEffect(() => {
    if (userSettings) {
      if (userSettings.themePrimaryColor) {
        setPrimaryColor(userSettings.themePrimaryColor);
      }
      if (userSettings.themeSecondaryColor) {
        setSecondaryColor(userSettings.themeSecondaryColor);
      }
      if (userSettings.themeAccentColor) {
        setAccentColor(userSettings.themeAccentColor);
      }
    }
  }, [userSettings]);

  const handleSaveTheme = async () => {
    try {
      setLoading(true);
      const updateData: UpdateUserSettingsRequest = {
        themePrimaryColor: primaryColor,
        themeSecondaryColor: secondaryColor,
        themeAccentColor: accentColor,
      };

      await settingsService.updateUserSettings(updateData);
      appMessage.success('تنظیمات تم با موفقیت به‌روزرسانی شد');
      await reload();
      
      applyTheme({ primary: primaryColor, secondary: secondaryColor, accent: accentColor });
    } catch (error) {
      handleApiError(error, 'خطا در ذخیره تنظیمات تم');
    } finally {
      setLoading(false);
    }
  };

  const handleResetTheme = async () => {
    try {
      setLoading(true);
      setPrimaryColor(DEFAULT_THEME.primary);
      setSecondaryColor(DEFAULT_THEME.secondary);
      setAccentColor(DEFAULT_THEME.accent);

      const updateData: UpdateUserSettingsRequest = {
        themePrimaryColor: DEFAULT_THEME.primary,
        themeSecondaryColor: DEFAULT_THEME.secondary,
        themeAccentColor: DEFAULT_THEME.accent,
      };

      await settingsService.updateUserSettings(updateData);
      
      
      await reload();
      applyTheme(DEFAULT_THEME);
      appMessage.success('رنگبندی تم به حالت پیشفرض برگشت');
    } catch (error) {
      handleApiError(error, 'خطا در بازگردانی تم به پیشفرض');
    } finally {
      setLoading(false);
    }
  };



  useEffect(() => {
    if (userSettings) {
      applyTheme({
        primary: userSettings.themePrimaryColor || primaryColor,
        secondary: userSettings.themeSecondaryColor || secondaryColor,
        accent: userSettings.themeAccentColor || accentColor,
      });
    }
  }, [userSettings]);

  return (
    <Card withBorder>
      <Stack gap="md">
        <Title order={4}>تنظیمات تم</Title>

        <Stack gap="xs">
          <Text size="sm" fw={500}>
            رنگ ورودی‌ها و دکمه‌ها
          </Text>
          <Group gap="md" align="flex-start">
            <ColorPicker
              value={primaryColor}
              onChange={setPrimaryColor}
              format="hex"
            />
            <TextInput
              label="کد هگز"
              value={primaryColor}
              onChange={(e) => {
                const value = e.currentTarget.value;
                if (/^#[0-9A-Fa-f]{0,6}$/.test(value)) {
                  setPrimaryColor(value);
                }
              }}
              placeholder="#228be6"
              style={{ flex: 1 }}
            />
          </Group>
        </Stack>

        <Stack gap="xs">
          <Text size="sm" fw={500}>
            رنگ متن‌ها
          </Text>
          <Group gap="md" align="flex-start">
            <ColorPicker
              value={secondaryColor}
              onChange={setSecondaryColor}
              format="hex"
            />
            <TextInput
              label="کد هگز"
              value={secondaryColor}
              onChange={(e) => {
                const value = e.currentTarget.value;
                if (/^#[0-9A-Fa-f]{0,6}$/.test(value)) {
                  setSecondaryColor(value);
                }
              }}
              placeholder="#868e96"
              style={{ flex: 1 }}
            />
          </Group>
        </Stack>

        <Stack gap="xs">
          <Text size="sm" fw={500}>
            رنگ آیکن‌ها
          </Text>
          <Group gap="md" align="flex-start">
            <ColorPicker
              value={accentColor}
              onChange={setAccentColor}
              format="hex"
            />
            <TextInput
              label="کد هگز"
              value={accentColor}
              onChange={(e) => {
                const value = e.currentTarget.value;
                if (/^#[0-9A-Fa-f]{0,6}$/.test(value)) {
                  setAccentColor(value);
                }
              }}
              placeholder="#fd7e14"
              style={{ flex: 1 }}
            />
          </Group>
        </Stack>

        <Group justify="space-between">
          <Button variant="outline" color="gray" onClick={handleResetTheme} loading={loading}>
            برگشت به رنگهای پیشفرض
          </Button>
          <Button onClick={handleSaveTheme} loading={loading}>
            ذخیره تنظیمات تم
          </Button>
        </Group>
      </Stack>
    </Card>
  );
}

