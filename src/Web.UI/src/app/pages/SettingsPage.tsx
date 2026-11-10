import { useState } from 'react';
import { Stack, Title, Tabs } from '@mantine/core';
import { useAuth } from '@/app/context/AuthContext';
import { PERM } from '@/app/models/permissions';
import ProfileSection from '@/app/components/settings/ProfileSection';
import UserInfoSection from '@/app/components/settings/UserInfoSection';
import ChangePasswordSection from '@/app/components/settings/ChangePasswordSection';
import ThemeSection from '@/app/components/settings/ThemeSection';
import BackupSection from '@/app/components/settings/BackupSection';
import AboutSection from '@/app/components/settings/AboutSection';


export default function SettingsPage() {
  const { can } = useAuth();
  const [tab, setTab] = useState<string | null>('profile');

  const tabs = [
    { value: 'profile', label: 'پروفایل', perm: PERM.User.updateProfile, node: <ProfileSection /> },
    { value: 'user-info', label: 'اطلاعات کاربر', perm: PERM.User.updateProfile, node: <UserInfoSection /> },
    { value: 'password', label: 'تغییر رمز', perm: PERM.User.changePassword, node: <ChangePasswordSection /> },
    { value: 'theme', label: 'ظاهر', perm: PERM.Settings.updateUser, node: <ThemeSection /> },
    { value: 'backup', label: 'بکاپ', perm: PERM.Backup.getInfo, node: <BackupSection /> },
    { value: 'about', label: 'درباره', perm: PERM.Settings.appInfo, node: <AboutSection /> },
  ].filter((t) => can(t.perm));

  return (
    <Stack p="md">
      <Title order={2}>تنظیمات</Title>

      <Tabs value={tab} onChange={setTab} keepMounted={false}>
        <Tabs.List>
          {tabs.map((t) => (
            <Tabs.Tab key={t.value} value={t.value}>{t.label}</Tabs.Tab>
          ))}
        </Tabs.List>

        {tabs.map((t) => (
          <Tabs.Panel key={t.value} value={t.value} pt="md">
            {t.node}
          </Tabs.Panel>
        ))}
      </Tabs>
    </Stack>
  );
}
