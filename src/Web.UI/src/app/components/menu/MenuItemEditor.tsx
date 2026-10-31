import { Stack, NumberInput, Switch, Select, Group, ActionIcon, Text } from '@mantine/core';
import { getFaIcon } from '@/app/helpers/faIcons';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { IconPickerModal } from './IconPickerModal';
import { FaPalette, FaTimes } from 'react-icons/fa';
import type { MenuItem, CreateMenuItemRequest, UpdateMenuItemRequest } from '@/app/models/menu';
import { useState } from 'react';

type MenuItemEditorProps = {
  menuItem?: MenuItem;
  allMenuItems: MenuItem[];
  value: CreateMenuItemRequest | UpdateMenuItemRequest;
  onChange: (value: CreateMenuItemRequest | UpdateMenuItemRequest) => void;
  errors?: Record<string, string>;
};


const flattenMenuItems = (items: MenuItem[], parentLabel = '', excludeId?: number): Array<{ value: string; label: string }> => {
  const result: Array<{ value: string; label: string }> = [];
  
  items.forEach((item) => {
    if (item.id !== excludeId) {
      const label = parentLabel ? `${parentLabel} > ${item.label}` : item.label;
      result.push({ value: String(item.id), label });
      
      if (item.children && item.children.length > 0) {
        result.push(...flattenMenuItems(item.children, label, excludeId));
      }
    }
  });
  
  return result;
};

export function MenuItemEditor({ menuItem, allMenuItems, value, onChange, errors }: MenuItemEditorProps) {
  const [iconPickerOpened, setIconPickerOpened] = useState(false);

  const parentOptions = [
    { value: '', label: 'بدون منوی والد' },
    ...flattenMenuItems(allMenuItems, '', menuItem?.id),
  ];

  const getIconComponent = (iconName?: string) => {
    if (!iconName) return null;
    const IconComponent = getFaIcon(iconName);
    if (!IconComponent) return null;
    return <IconComponent size={16} />;
  };

  const handleIconSelect = (iconName: string) => {
    try {
      onChange({
        ...value,
        icon: iconName,
      });
    } catch (error) {
      console.error('Error in handleIconSelect:', error);
    }
  };

  return (
    <>
      <Stack gap="md">
        <SmartTextInput
          label="عنوان"
          value={value.label}
          onChange={(e) => onChange({ ...value, label: e.currentTarget.value })}
          error={errors?.label}
          required
        />

        <SmartTextInput
          label="مسیر (Path)"
          value={value.path || ''}
          onChange={(e) => onChange({ ...value, path: e.currentTarget.value || undefined })}
          error={errors?.path}
          placeholder="مثال: /books"
        />

        <div>
          <Text size="sm" fw={500} mb={5}>
            آیکن
          </Text>
          <Group gap="xs">
            {value.icon && getIconComponent(value.icon) && (
              <ActionIcon variant="light" size="lg">
                {getIconComponent(value.icon)}
              </ActionIcon>
            )}
            <ActionIcon variant="outline" onClick={() => setIconPickerOpened(true)}>
              <FaPalette size={16} />
            </ActionIcon>
            {value.icon && (
              <ActionIcon
                variant="subtle"
                color="red"
                onClick={() => onChange({ ...value, icon: undefined })}
              >
                <FaTimes size={14} />
              </ActionIcon>
            )}
          </Group>
        </div>

        <Select
          label="منوی والد"
          data={parentOptions}
          value={value.parentId ? String(value.parentId) : ''}
          onChange={(val) => {
            try {
              onChange({ ...value, parentId: val ? Number(val) : undefined });
            } catch (error) {
              console.error('Error in parentId onChange:', error);
            }
          }}
          clearable
          searchable
        />

        <NumberInput
          label="ترتیب نمایش"
          value={value.displayOrder}
          onChange={(val) => onChange({ ...value, displayOrder: Number(val) || 0 })}
          min={0}
          error={errors?.displayOrder}
          required
        />

        <Switch
          label="فعال"
          checked={value.isActive !== false}
          onChange={(e) => onChange({ ...value, isActive: e.currentTarget.checked })}
        />
      </Stack>

      <IconPickerModal
        opened={iconPickerOpened}
        onClose={() => setIconPickerOpened(false)}
        onSelect={handleIconSelect}
        currentIcon={value.icon}
      />
    </>
  );
}

