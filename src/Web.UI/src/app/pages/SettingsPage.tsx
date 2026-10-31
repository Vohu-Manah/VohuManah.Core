import { useState } from 'react';
import { Stack, Title, Tabs } from '@mantine/core';
import ProfileSection from '@/app/components/settings/ProfileSection';
import UserInfoSection from '@/app/components/settings/UserInfoSection';
import ChangePasswordSection from '@/app/components/settings/ChangePasswordSection';
import ThemeSection from '@/app/components/settings/ThemeSection';
import BackupSection from '@/app/components/settings/BackupSection';
import AboutSection from '@/app/components/settings/AboutSection';


export default function SettingsPage() {
  const [tab, setTab] = useState<string | null>('profile');

  return (
    <Stack p="md">
      <Title order={2}>تنظیمات</Title>

      <Tabs value={tab} onChange={setTab} keepMounted={false}>
        <Tabs.List>
          <Tabs.Tab value="profile">پروفایل</Tabs.Tab>
          <Tabs.Tab value="user-info">اطلاعات کاربر</Tabs.Tab>
          <Tabs.Tab value="password">تغییر رمز</Tabs.Tab>
          <Tabs.Tab value="theme">ظاهر</Tabs.Tab>
          <Tabs.Tab value="backup">بکاپ</Tabs.Tab>
          <Tabs.Tab value="about">درباره</Tabs.Tab>
        </Tabs.List>

        <Tabs.Panel value="profile" pt="md">
          <ProfileSection />
        </Tabs.Panel>
        <Tabs.Panel value="user-info" pt="md">
          <UserInfoSection />
        </Tabs.Panel>
        <Tabs.Panel value="password" pt="md">
          <ChangePasswordSection />
        </Tabs.Panel>
        <Tabs.Panel value="theme" pt="md">
          <ThemeSection />
        </Tabs.Panel>
        <Tabs.Panel value="backup" pt="md">
          <BackupSection />
        </Tabs.Panel>
        <Tabs.Panel value="about" pt="md">
          <AboutSection />
        </Tabs.Panel>
      </Tabs>
    </Stack>
  );
}
