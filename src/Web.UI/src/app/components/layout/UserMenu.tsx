import { useState } from 'react';
import { Avatar, Group, Menu, Text, UnstyledButton } from '@mantine/core';
import { FaChevronDown } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/app/context/AuthContext';
import { useUserSettings } from '@/app/context/UserSettingsContext';
import { getFullImageUrl } from '@/app/helpers/imageUrl';

export default function UserMenu() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const { userSettings } = useUserSettings();
  const initials = user?.fullName?.split(' ')?.[0]?.[0] ?? 'U';
  const profileImageUrl = getFullImageUrl(userSettings?.profileImageUrl);
  const [opened, setOpened] = useState(false);

  return (
    <Menu shadow="md" position="bottom-end" opened={opened} onChange={setOpened}>
      <Menu.Target>
        <UnstyledButton>
          <Group gap="xs" style={{ direction: 'rtl' }}>
            <Avatar src={profileImageUrl} radius="xl" color="blue">
              {!profileImageUrl && initials}
            </Avatar>
            <div style={{ textAlign: 'right', flex: 1 }}>
              <Text size="sm" fw={600} truncate>
                {user?.fullName ?? 'کاربر'}
              </Text>
              <Text size="xs" c="dimmed" truncate>
                {user?.userName ?? ''}
              </Text>
            </div>
            <FaChevronDown
              size={12}
              style={{
                opacity: 0.7,
                transform: opened ? 'rotate(180deg)' : 'rotate(0deg)',
                transition: 'transform 0.2s ease',
              }}
            />
          </Group>
        </UnstyledButton>
      </Menu.Target>
      <Menu.Dropdown style={{ direction: 'rtl' }}>
        <Menu.Item onClick={() => navigate('/settings')} style={{ textAlign: 'right' }}>
          تنظیمات
        </Menu.Item>
        <Menu.Divider />
        <Menu.Item color="red" onClick={() => logout()} style={{ textAlign: 'right' }}>
          خروج
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  );
}
