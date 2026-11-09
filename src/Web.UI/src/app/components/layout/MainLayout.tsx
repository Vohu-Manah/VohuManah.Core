import {
  AppShell,
  Burger,
  Group,
  ScrollArea,
  Text,
  Avatar,
  Stack,
  Code,
  rem,
  Switch,
  Image,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { FaSun, FaMoon } from 'react-icons/fa';
import { Outlet, useLocation } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { useAuth } from '@/app/context/AuthContext';
import { useColorScheme } from '@/app/context/ColorSchemeContext';
import { getFullImageUrl } from '@/app/helpers/imageUrl';
import { settingsService } from '@/app/services/library/settingsService';
import { visitService } from '@/app/services/library/visitService';
import { handleApiError } from '@/app/helpers/errorHandler';
import UserMenu from '@/app/components/layout/UserMenu';
import { DynamicMenu } from '@/app/components/layout/DynamicMenu';
import PageTitleUpdater from '@/app/components/common/PageTitleUpdater';
import { useThemeColors } from '@/app/hooks/useThemeColors';
import favicon from '@/assets/images/favicon.png';

function ThemeColorsApplier() {
  useThemeColors();

  return null;
}

export default function MainLayout() {
  const [opened, { toggle, close }] = useDisclosure();
  const { user, isAuthenticated } = useAuth();
  const { colorScheme, toggleColorScheme } = useColorScheme();
  const [version, setVersion] = useState<string>('v1.0.0');
  
  
  const [profileImageUrl, setProfileImageUrl] = useState<string | null>(null);
  useEffect(() => {
    if (!isAuthenticated) {
      setProfileImageUrl(null);
      return;
    }
    let cancelled = false;
    settingsService
      .getUserSettings()
      .then((settings) => {
        if (!cancelled) setProfileImageUrl(settings.profileImageUrl ?? null);
      })
      .catch(() => undefined);
    return () => {
      cancelled = true;
    };
  }, [isAuthenticated]);

  const { pathname } = useLocation();

  
  
  useEffect(() => {
    if (opened) close();
    
  }, [pathname]);

  
  useEffect(() => {
    if (pathname && pathname !== '/') {
      visitService.record(pathname);
    }
  }, [pathname]);

  const userInitials =
    user?.fullName
      ?.split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2) || 'U';
  const userEmail = user?.userName || 'user@example.com';

  useEffect(() => {
    const loadVersion = async () => {
      try {
        const appInfo = await settingsService.getAppInfo();
        if (appInfo.version) {
          setVersion(`v${appInfo.version}`);
        }
      } catch (error) {
        handleApiError(error, 'خطا در بارگذاری اطلاعات برنامه');
      }
    };

    loadVersion();
  }, []);

  return (
    <>
      <PageTitleUpdater />
      <ThemeColorsApplier />
      <AppShell
        header={{ height: 60 }}
        navbar={{
          width: 300,
          breakpoint: 'sm',
          collapsed: { mobile: !opened },
        }}
        padding="md"
      >
        <AppShell.Header>
          <Group h="100%" px="md" justify="space-between">
            <Group>
              <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" />
              <Text fw={600}>پنل مدیریتی</Text>
            </Group>
            <Group gap="md">
              <UserMenu />
              <Switch
                checked={colorScheme === 'dark'}
                onChange={(event) => {
                  if (event.currentTarget.checked !== (colorScheme === 'dark')) {
                    toggleColorScheme();
                  }
                }}
                size="md"
                thumbIcon={
                  colorScheme === 'dark' ? (
                    <FaMoon size={12} style={{ color: 'var(--mantine-color-dark-6)' }} />
                  ) : (
                    <FaSun size={12} style={{ color: 'var(--mantine-color-yellow-4)' }} />
                  )
                }
                aria-label="تغییر حالت تم"
              />
            </Group>
          </Group>
        </AppShell.Header>

        <AppShell.Navbar p="md">
          <AppShell.Section
            style={{
              paddingBottom: rem(16),
              marginBottom: rem(16),
              borderBottom: '1px solid var(--mantine-color-gray-2)',
            }}
          >
            <Group justify="space-between">
              <Group gap="xs">
                <Image src={favicon} alt="favicon" width={30} height={30} />
                <Text fw={700} size="lg">
                  وُهُومَنَهْ
                </Text>
              </Group>
              <Code fw={700}>{version}</Code>
            </Group>
          </AppShell.Section>

          {}
          <AppShell.Section grow component={ScrollArea} dir="ltr">
            <Stack gap={0} dir="rtl">
              <DynamicMenu />
            </Stack>
          </AppShell.Section>

          <AppShell.Section
            style={{
              paddingTop: rem(16),
              borderTop: '1px solid var(--mantine-color-gray-2)',
            }}
          >
            <Stack gap="sm">
              <Group gap="sm">
                <Avatar
                  src={getFullImageUrl(profileImageUrl)}
                  size="sm"
                  radius="xl"
                  color="blue"
                >
                  {!getFullImageUrl(profileImageUrl) && userInitials}
                </Avatar>
                <div style={{ flex: 1, minWidth: 0 }}>
                  <Text size="sm" fw={500} truncate>
                    {user?.fullName || 'کاربر'}
                  </Text>
                  <Text size="xs" c="dimmed" truncate>
                    {userEmail}
                  </Text>
                </div>
              </Group>
              <Group gap="xs" justify="space-between" align="center">
                <Text size="xs" c="dimmed">
                  {colorScheme === 'light' ? 'حالت روشن' : 'حالت تاریک'}
                </Text>
                <Switch
                  checked={colorScheme === 'dark'}
                  onChange={(event) => {
                    if (event.currentTarget.checked !== (colorScheme === 'dark')) {
                      toggleColorScheme();
                    }
                  }}
                  size="sm"
                  thumbIcon={
                    colorScheme === 'dark' ? (
                      <FaMoon size={10} style={{ color: 'var(--mantine-color-dark-6)' }} />
                    ) : (
                      <FaSun size={10} style={{ color: 'var(--mantine-color-yellow-4)' }} />
                    )
                  }
                  aria-label="تغییر حالت تم"
                />
              </Group>
            </Stack>
          </AppShell.Section>
        </AppShell.Navbar>

        <AppShell.Main>
          <Outlet />
        </AppShell.Main>
      </AppShell>
    </>
  );
}
