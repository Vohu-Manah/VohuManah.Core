import { useState, useMemo } from 'react';
import { getFaIcon } from '@/app/helpers/faIcons';
import { Modal, TextInput, ScrollArea, Group, ActionIcon, Stack, Text } from '@mantine/core';
import { FaSearch } from 'react-icons/fa';
import * as FaIcons from 'react-icons/fa';


const allIcons = Object.keys(FaIcons).filter(
  (key) => key !== 'default' && typeof FaIcons[key as keyof typeof FaIcons] === 'function'
) as Array<keyof typeof FaIcons>;

type IconPickerModalProps = {
  opened: boolean;
  onClose: () => void;
  onSelect: (iconName: string) => void;
  currentIcon?: string;
};

export function IconPickerModal({ opened, onClose, onSelect, currentIcon }: IconPickerModalProps) {
  const [searchQuery, setSearchQuery] = useState('');

  const filteredIcons = useMemo(() => {
    if (!searchQuery.trim()) {
      return allIcons;
    }
    const query = searchQuery.toLowerCase();
    return allIcons.filter((iconName) => iconName.toLowerCase().includes(query));
  }, [searchQuery]);

  const handleSelect = (iconName: string) => {
    onSelect(iconName);
    onClose();
  };

  const getIconComponent = (iconName: string) => {
    const IconComponent = getFaIcon(iconName);
    if (!IconComponent) return null;
    return <IconComponent size={20} />;
  };

  return (
    <Modal opened={opened} onClose={onClose} title="انتخاب آیکن" size="lg" centered dir="rtl">
      <Stack gap="md">
        <TextInput
          placeholder="جستجوی آیکن..."
          leftSection={<FaSearch size={14} />}
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.currentTarget.value)}
        />
        <ScrollArea h={400}>
          <Group gap="xs">
            {filteredIcons.map((iconName) => {
              const IconComponent = getIconComponent(iconName);
              if (!IconComponent) return null;

              const isSelected = currentIcon === iconName;

              return (
                <ActionIcon
                  key={iconName}
                  variant={isSelected ? 'filled' : 'light'}
                  color={isSelected ? 'blue' : 'gray'}
                  size="lg"
                  onClick={() => handleSelect(iconName)}
                  title={iconName}
                  style={{
                    border: isSelected ? '2px solid var(--mantine-color-blue-6)' : '1px solid transparent',
                  }}
                >
                  {IconComponent}
                </ActionIcon>
              );
            })}
          </Group>
          {filteredIcons.length === 0 && (
            <Text ta="center" c="dimmed" py="xl">
              آیکنی یافت نشد
            </Text>
          )}
        </ScrollArea>
      </Stack>
    </Modal>
  );
}

