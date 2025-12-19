import { Avatar, Group, Menu, Text, UnstyledButton } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export default function UserMenu() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const initials = user?.fullName?.split(' ')?.[0]?.[0] ?? 'U';

  return (
    <Menu shadow="md" position="bottom-end">
      <Menu.Target>
        <UnstyledButton>
          <Group gap="xs">
            <Avatar radius="xl" color="blue">
              {initials}
            </Avatar>
            <div>
              <Text size="sm" fw={600}>
                {user?.fullName ?? 'کاربر'}
              </Text>
              <Text size="xs" c="dimmed">
                {user?.userName ?? ''}
              </Text>
            </div>
          </Group>
        </UnstyledButton>
      </Menu.Target>
      <Menu.Dropdown>
        <Menu.Item onClick={() => navigate('/profile')}>پروفایل</Menu.Item>
        <Menu.Divider />
        <Menu.Item color="red" onClick={() => logout()}>
          خروج
        </Menu.Item>
      </Menu.Dropdown>
    </Menu>
  );
}

