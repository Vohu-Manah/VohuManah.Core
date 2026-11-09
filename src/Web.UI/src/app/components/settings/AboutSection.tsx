import { useState, useEffect } from 'react';
import { Card, Stack, Title, Text, Group, Divider, Badge } from '@mantine/core';
import { settingsService } from '@/app/services/library/settingsService';
import { handleApiError } from '@/app/helpers/errorHandler';
import type { AppInfo, SystemInfo } from '@/app/models/settings';

export default function AboutSection() {
  const [, setLoading] = useState(false);
  const [appInfo, setAppInfo] = useState<AppInfo | null>(null);
  const [systemInfo, setSystemInfo] = useState<SystemInfo | null>(null);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [appData, systemData] = await Promise.all([
        settingsService.getAppInfo(),
        settingsService.getSystemInfo(),
      ]);
      setAppInfo(appData);
      setSystemInfo(systemData);
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری اطلاعات');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card withBorder>
      <Stack gap="md">
        <Title order={4}>درباره برنامه</Title>

        {appInfo && (
          <Stack gap="xs">
            <Group justify="space-between">
              <Text fw={600}>نام برنامه:</Text>
              <Text>{appInfo.appName}</Text>
            </Group>
            <Group justify="space-between">
              <Text fw={600}>ورژن:</Text>
              <Badge>{appInfo.version}</Badge>
            </Group>
            <Group justify="space-between">
              <Text fw={600}>نویسنده:</Text>
              <Text>{appInfo.author}</Text>
            </Group>
            {appInfo.description && (
              <Group justify="space-between" align="flex-start">
                <Text fw={600}>توضیحات:</Text>
                <Text style={{ flex: 1, textAlign: 'right' }}>{appInfo.description}</Text>
              </Group>
            )}
            {appInfo.lastUpdateDate && (
              <Group justify="space-between">
                <Text fw={600}>آخرین بروزرسانی:</Text>
                <Text>{new Date(appInfo.lastUpdateDate).toLocaleDateString('fa-IR')}</Text>
              </Group>
            )}
            {appInfo.copyright && (
              <Group justify="space-between">
                <Text fw={600}>کپی‌رایت:</Text>
                <Text size="sm" c="dimmed">{appInfo.copyright}</Text>
              </Group>
            )}
            {appInfo.license && (
              <Group justify="space-between">
                <Text fw={600}>مجوز:</Text>
                <Text>{appInfo.license}</Text>
              </Group>
            )}
            {appInfo.website && (
              <Group justify="space-between">
                <Text fw={600}>وب‌سایت:</Text>
                <Text c="blue">{appInfo.website}</Text>
              </Group>
            )}
            {appInfo.supportEmail && (
              <Group justify="space-between">
                <Text fw={600}>ایمیل پشتیبانی:</Text>
                <Text c="blue">{appInfo.supportEmail}</Text>
              </Group>
            )}
          </Stack>
        )}

        {systemInfo && (
          <>
            <Divider my="md" />
            <Title order={5}>مشخصات سیستم</Title>
            <Stack gap="xs">
              <Group justify="space-between">
                <Text fw={600}>سیستم عامل:</Text>
                <Text>{systemInfo.operatingSystem}</Text>
              </Group>
              <Group justify="space-between">
                <Text fw={600}>ورژن فریمورک:</Text>
                <Text>{systemInfo.frameworkVersion}</Text>
              </Group>
              <Group justify="space-between">
                <Text fw={600}>نام سرور:</Text>
                <Text>{systemInfo.machineName}</Text>
              </Group>
              <Group justify="space-between">
                <Text fw={600}>تعداد پردازنده:</Text>
                <Text>{systemInfo.processorCount}</Text>
              </Group>
              <Group justify="space-between">
                <Text fw={600}>حافظه کل:</Text>
                <Text>{systemInfo.totalMemory}</Text>
              </Group>
              <Group justify="space-between">
                <Text fw={600}>حافظه در دسترس:</Text>
                <Text>{systemInfo.availableMemory}</Text>
              </Group>
              <Group justify="space-between">
                <Text fw={600}>زمان سرور:</Text>
                <Text>{new Date(systemInfo.serverTime).toLocaleString('fa-IR')}</Text>
              </Group>
              <Group justify="space-between">
                <Text fw={600}>منطقه زمانی:</Text>
                <Text>{systemInfo.timeZone}</Text>
              </Group>
            </Stack>
          </>
        )}
      </Stack>
    </Card>
  );
}


