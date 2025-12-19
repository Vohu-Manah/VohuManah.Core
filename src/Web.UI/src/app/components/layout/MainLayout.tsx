import { useState } from 'react';
import {
  AppShell,
  Burger,
  Group,
  NavLink,
  ScrollArea,
  Text,
  UnstyledButton,
  Avatar,
  Stack,
  Code,
  rem,
} from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import { menuRoutes, type AppRoute } from '../../routes/menu';
import { useAuth } from '../../context/AuthContext';
import UserMenu from './UserMenu';

function NestedNavLink({ route, level = 0 }: { route: AppRoute; level?: number }) {
  const { pathname } = useLocation();
  const navigate = useNavigate();
  const [opened, setOpened] = useState(false);

  const hasChildren = route.children && route.children.length > 0;
  
  // Normalize pathname - handle hash router
  const normalizePath = (path: string) => {
    if (!path) return '';
    // Remove hash if present
    const cleanPath = path.replace(/^#/, '');
    // Ensure starts with /
    return cleanPath.startsWith('/') ? cleanPath : `/${cleanPath}`;
  };
  
  const currentPath = normalizePath(pathname);
  
  // Check if this exact route is active
  const isExactActive = route.path ? currentPath === normalizePath(route.path) : false;
  
  // Check if any child is active
  const hasActiveChild = hasChildren && route.children!.some((child) => {
    if (!child.path) return false;
    const childPath = normalizePath(child.path);
    return currentPath === childPath || currentPath.startsWith(childPath + '/');
  });
  
  // Route is active ONLY if it's exact match (not parent of active child)
  // Parent routes should not be active when only children are active
  const isActive = isExactActive;
  
  const shouldBeOpened = opened || hasActiveChild;

  const handleClick = () => {
    if (route.path) {
      // Ensure path starts with / for hash router
      const targetPath = route.path.startsWith('/') ? route.path : `/${route.path}`;
      navigate(targetPath);
    } else if (hasChildren) {
      setOpened(!opened);
    }
  };

  return (
    <>
      <NavLink
        key={route.path || route.label}
        label={route.label}
        leftSection={route.icon}
        active={isActive}
        onClick={handleClick}
        opened={shouldBeOpened}
        childrenOffset={28}
        style={{
          direction: 'rtl',
          textAlign: 'right',
        }}
      />
      {hasChildren && shouldBeOpened && (
        <div style={{ direction: 'rtl' }}>
          {route.children!.map((child) => (
            <NestedNavLink key={child.path || child.label} route={child} level={level + 1} />
          ))}
        </div>
      )}
    </>
  );
}

export default function MainLayout() {
  const [opened, { toggle }] = useDisclosure();
  const { user } = useAuth();
  const userInitials = user?.fullName?.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2) || 'U';
  const userEmail = user?.userName || 'user@example.com';

  return (
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
          <UserMenu />
        </Group>
      </AppShell.Header>

      <AppShell.Navbar p="md">
        {/* Header Section */}
        <AppShell.Section
          style={{
            paddingBottom: rem(16),
            marginBottom: rem(16),
            borderBottom: '1px solid var(--mantine-color-gray-2)',
          }}
        >
          <Group justify="space-between">
            <Text fw={700} size="lg">
            وُهُومَنَهْ
            </Text>
            <Code fw={700}>v1.0.0</Code>
          </Group>
        </AppShell.Section>

        {/* Links Section */}
        <AppShell.Section grow component={ScrollArea}>
          <Stack gap={0}>
            {menuRoutes.map((route) => (
              <NestedNavLink key={route.path || route.label} route={route} />
            ))}
          </Stack>
        </AppShell.Section>

        {/* Footer Section with User Info */}
        <AppShell.Section
          style={{
            paddingTop: rem(16),
            borderTop: '1px solid var(--mantine-color-gray-2)',
          }}
        >
          <Group gap="sm">
            <Avatar size="sm" radius="xl">
              {userInitials}
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
        </AppShell.Section>
      </AppShell.Navbar>

      <AppShell.Main>
        <Outlet />
      </AppShell.Main>
    </AppShell>
  );
}

