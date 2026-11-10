import { useState, useEffect, useCallback } from 'react';
import { getFaIcon } from '@/app/helpers/faIcons';
import { Button, Card, Stack, Title, Group, ActionIcon, Badge, Switch, Text, Box } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash, FaGripVertical, FaChevronDown, FaChevronRight, FaUndo } from 'react-icons/fa';
import { FormModal } from '@/app/components/common/FormModal';
import { MenuItemEditor } from '@/app/components/menu/MenuItemEditor';
import { menuService } from '@/app/services/library/menuService';
import type { MenuItem, CreateMenuItemRequest, UpdateMenuItemRequest } from '@/app/models/menu';
import { appMessage } from '@/app/messages/appMessages';
import { handleApiError } from '@/app/helpers/errorHandler';
import { useAuth } from '@/app/context/AuthContext';
import { PERM } from '@/app/models/permissions';

type FlatMenuItem = MenuItem & { level: number };

const flattenMenuItems = (items: MenuItem[], level = 0): FlatMenuItem[] => {
  const result: FlatMenuItem[] = [];
  items.forEach((item) => {
    result.push({ ...item, level });
    if (item.children && item.children.length > 0) {
      result.push(...flattenMenuItems(item.children, level + 1));
    }
  });
  return result;
};

const rebuildHierarchy = (flatItems: FlatMenuItem[]): MenuItem[] => {
  const result: MenuItem[] = [];
  const stack: FlatMenuItem[] = [];

  flatItems.forEach((item) => {
    const menuItem: MenuItem = {
      id: item.id,
      label: item.label,
      path: item.path,
      icon: item.icon,
      parentId: item.parentId,
      displayOrder: item.displayOrder,
      isActive: item.isActive,
      isSystem: item.isSystem,
      children: [],
    };

    
    while (stack.length > 0 && stack[stack.length - 1].level >= item.level) {
      stack.pop();
    }

    if (stack.length > 0) {
      const parent = stack[stack.length - 1];
      menuItem.parentId = parent.id;
      if (!parent.children) {
        parent.children = [];
      }
      parent.children.push(menuItem);
    } else {
      result.push(menuItem);
    }

    const itemWithLevel: FlatMenuItem = { ...menuItem, level: item.level };
    stack.push(itemWithLevel);
  });

  return result;
};

type DraggedItem = {
  id: number;
  index: number;
};

export default function MenuSettingsPage() {
  const { can } = useAuth();
  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingItem, setEditingItem] = useState<MenuItem | null>(null);
  const [expandedItems, setExpandedItems] = useState<Set<number>>(new Set());
  const [draggedItem, setDraggedItem] = useState<DraggedItem | null>(null);
  const [dragOverIndex, setDragOverIndex] = useState<number | null>(null);

  const isSystemMenuOrChild = (item: MenuItem, allItems: MenuItem[]): boolean => {
    const systemMenuLabels = ['تنظیمات', 'مدیریت ادمین'];
    
    if (systemMenuLabels.includes(item.label) && !item.parentId) {
      return true;
    }
    const findParent = (id: number | undefined, items: MenuItem[]): MenuItem | null => {
      if (!id) return null;
      for (const menuItem of items) {
        if (menuItem.id === id) {
          return menuItem;
        }
        if (menuItem.children) {
          const found = findParent(id, menuItem.children);
          if (found) return found;
        }
      }
      return null;
    };

    if (item.parentId) {
      const parent = findParent(item.parentId, allItems);
      if (parent) {
        
        if (systemMenuLabels.includes(parent.label) && !parent.parentId) {
          return true;
        }
        
        if (isSystemMenuOrChild(parent, allItems)) {
          return true;
        }
      }
    }
    
    return false;
  };

  const [formData, setFormData] = useState<CreateMenuItemRequest | UpdateMenuItemRequest>({
    label: '',
    path: undefined,
    icon: undefined,
    parentId: undefined,
    displayOrder: 0,
    isActive: true,
  });

  const loadData = useCallback(async () => {
    try {
      const data = await menuService.getAll();
      setMenuItems(data);
      
      const allIds = new Set<number>();
      const collectIds = (items: MenuItem[]) => {
        items.forEach((item) => {
          allIds.add(item.id);
          if (item.children) {
            collectIds(item.children);
          }
        });
      };
      collectIds(data);
      setExpandedItems(allIds);
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری منو');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {

    void loadData();
  }, [loadData]);

  const handleSubmit = async () => {
    try {
      if (!formData.label || !formData.label.trim()) {
        appMessage.error('عنوان الزامی است');
        return;
      }

      setLoading(true);
      if (editingItem) {
        
        const updateData: Omit<UpdateMenuItemRequest, 'id'> = {
          label: formData.label.trim(),
          path: formData.path?.trim() || undefined,
          icon: formData.icon || undefined,
          parentId: formData.parentId || undefined,
          displayOrder: formData.displayOrder || 0,
          isActive: formData.isActive !== false,
        };
        await menuService.update(editingItem.id, updateData);
        appMessage.success('آیتم منو با موفقیت به‌روزرسانی شد');
      } else {
        
        const createData: CreateMenuItemRequest = {
          label: formData.label.trim(),
          path: formData.path?.trim() || undefined,
          icon: formData.icon || undefined,
          parentId: formData.parentId || undefined,
          displayOrder: formData.displayOrder || flattenMenuItems(menuItems).length,
          isActive: formData.isActive !== false,
        };
        await menuService.create(createData);
        appMessage.success('آیتم منو با موفقیت ایجاد شد');
      }
      setModalOpened(false);
      setEditingItem(null);
      setFormData({
        label: '',
        path: undefined,
        icon: undefined,
        parentId: undefined,
        displayOrder: flattenMenuItems(menuItems).length,
        isActive: true,
      });
      await loadData();
    } catch (error) {
      handleApiError(error, 'خطا در ذخیره آیتم منو');
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (item: MenuItem) => {
    if (isSystemMenuOrChild(item, menuItems)) {
      appMessage.error('نمی‌توان منوی تنظیمات و زیرمنوهایش را ویرایش کرد');
      return;
    }
    setEditingItem(item);
    setFormData({
      id: item.id,
      label: item.label,
      path: item.path,
      icon: item.icon,
      parentId: item.parentId,
      displayOrder: item.displayOrder,
      isActive: item.isActive,
    });
    setModalOpened(true);
  };

  const handleDelete = async (item: MenuItem) => {
    if (item.isSystem || isSystemMenuOrChild(item, menuItems)) {
      appMessage.error('نمی‌توان منوی تنظیمات و زیرمنوهایش را حذف کرد');
      return;
    }

    if (item.children && item.children.length > 0) {
      appMessage.error('ابتدا زیرمنوها را حذف کنید');
      return;
    }

    if (!confirm(`آیا از حذف "${item.label}" اطمینان دارید؟`)) {
      return;
    }

    try {
      setLoading(true);
      await menuService.delete(item.id);
      appMessage.success('آیتم منو با موفقیت حذف شد');
      loadData();
    } catch (error) {
      handleApiError(error, 'خطا در حذف آیتم منو');
    } finally {
      setLoading(false);
    }
  };

  const handleResetToDefault = async () => {
    if (!confirm('منوی فعلی حذف و منوی پیش‌فرض سیستم بازسازی می‌شود. ادامه می‌دهید؟')) {
      return;
    }

    try {
      setLoading(true);
      await menuService.resetToDefault();
      appMessage.success('منو به حالت پیش‌فرض بازگشت');
      await loadData();
    } catch (error) {
      handleApiError(error, 'خطا در بازگردانی منو به پیش‌فرض');
    } finally {
      setLoading(false);
    }
  };

  const handleToggleActive = async (item: MenuItem) => {
    if (item.isSystem || isSystemMenuOrChild(item, menuItems)) {
      appMessage.error('نمی‌توان وضعیت منوی تنظیمات و زیرمنوهایش را تغییر داد');
      return;
    }

    try {
      setLoading(true);
      const updateData: Omit<UpdateMenuItemRequest, 'id'> = {
        label: item.label,
        path: item.path,
        icon: item.icon,
        parentId: item.parentId,
        displayOrder: item.displayOrder,
        isActive: !item.isActive,
      };
      await menuService.update(item.id, updateData);
      appMessage.success('وضعیت آیتم منو تغییر کرد');
      loadData();
    } catch (error) {
      handleApiError(error, 'خطا در تغییر وضعیت آیتم منو');
    } finally {
      setLoading(false);
    }
  };

  const handleDragStart = (e: React.DragEvent, item: MenuItem, index: number) => {
    if (item.isSystem || isSystemMenuOrChild(item, menuItems)) {
      e.preventDefault();
      return;
    }
    setDraggedItem({ id: item.id, index });
    e.dataTransfer.effectAllowed = 'move';
  };

  const handleDragOver = (e: React.DragEvent, index: number, item: MenuItem, level: number) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
    setDragOverIndex(index);
    
    if (!draggedItem) return;
    
    
    const container = document.getElementById('menu-container');
    if (!container) return;
    
    const containerRect = container.getBoundingClientRect();
    const x = e.clientX - containerRect.left;
    const indentWidth = 30;
    
    
    
    const targetLevel = Math.round(x / indentWidth);
    const newLevel = Math.max(0, Math.min(targetLevel, level + 1));
    
    
    const targetElement = e.currentTarget as HTMLElement;
    const cardElement = targetElement.closest('.mantine-Card-root') as HTMLElement;
    const parentBox = targetElement.closest('div[style*="marginLeft"]') as HTMLElement;
    
    if (!cardElement || !parentBox) return;
    
    
    let indicator = cardElement.querySelector('.drop-indicator') as HTMLElement;
    if (!indicator) {
      indicator = document.createElement('div');
      indicator.className = 'drop-indicator';
      cardElement.style.position = 'relative';
      cardElement.appendChild(indicator);
    }
    
    
    const cardRect = cardElement.getBoundingClientRect();
    const boxRect = parentBox.getBoundingClientRect();
    const indicatorX = newLevel * indentWidth - (boxRect.left - cardRect.left);
    
    indicator.style.cssText = `
      position: absolute;
      left: ${Math.max(0, indicatorX)}px;
      top: 0;
      bottom: 0;
      width: 3px;
      background: var(--mantine-color-blue-6);
      pointer-events: none;
      z-index: 10;
      display: block;
    `;
    
    
    
    if (newLevel === level + 1 && item.children && item.children.length > 0) {
      cardElement.style.borderLeft = '3px solid var(--mantine-color-blue-6)';
      cardElement.style.backgroundColor = 'var(--mantine-color-blue-0)';
    } else {
      cardElement.style.borderLeft = '';
      cardElement.style.backgroundColor = '';
    }
  };

  const handleDrop = async (e: React.DragEvent, _targetIndex: number, targetItem: MenuItem, targetLevel: number) => {
    e.preventDefault();
    setDragOverIndex(null);
    
    
    const targetElement = e.currentTarget as HTMLElement;
    const cardElement = targetElement.closest('.mantine-Card-root') as HTMLElement;
    if (cardElement) {
      cardElement.style.borderLeft = '';
      cardElement.style.backgroundColor = '';
      const indicator = cardElement.querySelector('.drop-indicator') as HTMLElement;
      if (indicator) indicator.style.display = 'none';
    }

    if (!draggedItem) return;

    const findMenuItemById = (id: number, items: MenuItem[]): MenuItem | undefined => {
      for (const menuItem of items) {
        if (menuItem.id === id) return menuItem;
        if (menuItem.children) {
          const found = findMenuItemById(id, menuItem.children);
          if (found) return found;
        }
      }
      return undefined;
    };
    
    const draggedMenuItem = findMenuItemById(draggedItem.id, menuItems);

    if (draggedMenuItem && isSystemMenuOrChild(draggedMenuItem, menuItems)) {
      appMessage.error('نمی‌توان منوی تنظیمات و زیرمنوهایش را جابه‌جا کرد');
      setDraggedItem(null);
      return;
    }

    if (isSystemMenuOrChild(targetItem, menuItems)) {
      appMessage.error('نمی‌توان منوی تنظیمات و زیرمنوهایش را جابه‌جا کرد');
      setDraggedItem(null);
      return;
    }

    const flatItems = flattenMenuItems(menuItems);
    const sourceIndex = flatItems.findIndex((item) => item.id === draggedItem.id);
    const targetFlatIndex = flatItems.findIndex((item) => item.id === targetItem.id);

    if (sourceIndex === -1 || targetFlatIndex === -1 || sourceIndex === targetFlatIndex) {
      setDraggedItem(null);
      return;
    }

    try {
      setLoading(true);

      
      
      const container = document.getElementById('menu-container');
      if (!container) {
        setDraggedItem(null);
        return;
      }
      
      const containerRect = container.getBoundingClientRect();
      const x = e.clientX - containerRect.left;
      const indentWidth = 30;
      const targetLevelFromMouse = Math.round(x / indentWidth);
      const newLevel = Math.max(0, Math.min(targetLevelFromMouse, targetLevel + 1));

      
      const [movedItem] = flatItems.splice(sourceIndex, 1);
      let newTargetIndex = sourceIndex < targetFlatIndex ? targetFlatIndex - 1 : targetFlatIndex;
      
      
      if (newLevel === targetLevel + 1) {
        
        const targetItemLevel = flatItems[targetFlatIndex]?.level || 0;
        let lastChildIndex = targetFlatIndex;
        for (let i = targetFlatIndex + 1; i < flatItems.length; i++) {
          if (flatItems[i].level > targetItemLevel) {
            lastChildIndex = i;
          } else {
            break;
          }
        }
        newTargetIndex = lastChildIndex + 1; 
        movedItem.parentId = targetItem.id;
      } else {
        
        movedItem.parentId = undefined;
        for (let i = newTargetIndex - 1; i >= 0; i--) {
          const prevItem = flatItems[i] as FlatMenuItem;
          if (prevItem.level < newLevel) {
            movedItem.parentId = prevItem.id;
            break;
          }
        }
      }
      
      flatItems.splice(newTargetIndex, 0, movedItem);
      (movedItem as FlatMenuItem).level = newLevel;

      
      flatItems.forEach((item, idx) => {
        item.displayOrder = idx;
      });

      
      const newHierarchy = rebuildHierarchy(flatItems);

      
      const reorderData = flatItems.map((item, idx) => ({
        id: item.id,
        displayOrder: idx,
        parentId: item.parentId,
      }));

      await menuService.reorder({ items: reorderData });
      appMessage.success('ترتیب منو با موفقیت به‌روزرسانی شد');
      setMenuItems(newHierarchy);
    } catch (error) {
      handleApiError(error, 'خطا در به‌روزرسانی ترتیب منو');
      await loadData();
    } finally {
      setLoading(false);
      setDraggedItem(null);
    }
  };

  const handleDragEnd = () => {
    setDraggedItem(null);
    setDragOverIndex(null);
  };

  const toggleExpand = (itemId: number) => {
    setExpandedItems((prev) => {
      const newSet = new Set(prev);
      if (newSet.has(itemId)) {
        newSet.delete(itemId);
      } else {
        newSet.add(itemId);
      }
      return newSet;
    });
  };

  const renderMenuItem = (item: MenuItem, level = 0): React.ReactNode => {
    const hasChildren = item.children && item.children.length > 0;
    const isExpanded = expandedItems.has(item.id);
    const flatItems = flattenMenuItems(menuItems);
    const flatIndex = flatItems.findIndex((i) => i.id === item.id);

    const getIconComponent = (iconName?: string) => {
      if (!iconName) return null;
      const IconComponent = getFaIcon(iconName);
      if (!IconComponent) return null;
      return <IconComponent size={14} />;
    };

    
    return (
      <Box key={item.id} className={`menu-item-indent-${Math.min(level, 6)}`}>
        <Card
          p="sm"
          mb="xs"
          className={`menu-drag-item ${draggedItem?.id === item.id ? "is-dragging" : ""} ${dragOverIndex === flatIndex ? "drag-over" : "drag-default"} ${(item.isSystem || isSystemMenuOrChild(item, menuItems)) ? "cursor-not-allowed" : "cursor-move"}`}
          draggable={can(PERM.Menu.reorder) && !item.isSystem && !isSystemMenuOrChild(item, menuItems)}
          onDragStart={(e) => handleDragStart(e, item, flatIndex)}
          onDragOver={(e) => handleDragOver(e, flatIndex, item, level)}
          onDragLeave={(e) => {
            const targetElement = e.currentTarget as HTMLElement;
            const cardElement = targetElement.closest('.mantine-Card-root') as HTMLElement;
            if (cardElement) {
              cardElement.style.borderLeft = '';
              cardElement.style.backgroundColor = '';
              
              const indicator = cardElement.querySelector('.drop-indicator') as HTMLElement;
              if (indicator) indicator.style.display = 'none';
            }
          }}
          onDrop={(e) => handleDrop(e, flatIndex, item, level)}
          onDragEnd={handleDragEnd}
        >
          <Group justify="space-between">
            <Group gap="xs">
              {hasChildren ? (
                <ActionIcon
                  variant="subtle"
                  size="sm"
                  onClick={() => toggleExpand(item.id)}
                >
                  {isExpanded ? <FaChevronDown size={12} /> : <FaChevronRight size={12} />}
                </ActionIcon>
              ) : (
                <Box w={20} />
              )}
              {can(PERM.Menu.reorder) && (
                <FaGripVertical size={14} className={(item.isSystem || isSystemMenuOrChild(item, menuItems)) ? "cursor-not-allowed" : "cursor-grab"} />
              )}
              {getIconComponent(item.icon)}
              <Text size="sm" fw={500}>
                {item.label}
              </Text>
              {item.path && (
                <Text size="xs" c="dimmed">
                  {item.path}
                </Text>
              )}
              {item.isSystem && <Badge color="blue" size="sm">سیستمی</Badge>}
              {!item.isActive && <Badge color="red" size="sm">غیرفعال</Badge>}
            </Group>
            <Group gap="xs">
              <Switch
                checked={item.isActive}
                onChange={() => handleToggleActive(item)}
                disabled={item.isSystem || isSystemMenuOrChild(item, menuItems)}
                size="sm"
              />
              {can(PERM.Menu.update) && (
                <ActionIcon
                  variant="light"
                  color="blue"
                  onClick={() => handleEdit(item)}
                  disabled={item.isSystem || isSystemMenuOrChild(item, menuItems)}
                >
                  <FaEdit size={14} />
                </ActionIcon>
              )}
              {can(PERM.Menu.del) && (
                <ActionIcon
                  variant="light"
                  color="red"
                  onClick={() => handleDelete(item)}
                  disabled={item.isSystem || isSystemMenuOrChild(item, menuItems)}
                >
                  <FaTrash size={14} />
                </ActionIcon>
              )}
            </Group>
          </Group>
        </Card>
        {hasChildren && isExpanded && (
          <Box>
            {item.children!.map((child) => renderMenuItem(child, level + 1))}
          </Box>
        )}
      </Box>
    );
  };

  return (
    <Stack p="md">
      <Group justify="space-between">
        <Title order={2}>تنظیمات منو</Title>
        <Group gap="xs">
          {can(PERM.Menu.reset) && (
            <Button
              variant="outline"
              color="red"
              leftSection={<FaUndo size={14} />}
              onClick={handleResetToDefault}
              loading={loading}
            >
              بازگشت به منوی پیشفرض
            </Button>
          )}
          {can(PERM.Menu.create) && (
            <Button
              leftSection={<FaPlus size={14} />}
              onClick={() => {
                setEditingItem(null);
                setFormData({
                  label: '',
                  path: undefined,
                  icon: undefined,
                  parentId: undefined,
                  displayOrder: flattenMenuItems(menuItems).length,
                  isActive: true,
                });
                setModalOpened(true);
              }}
            >
              افزودن منو
            </Button>
          )}
        </Group>
      </Group>

      <Card id="menu-container">
        <Stack gap="xs">
          {menuItems.length === 0 ? (
            <Text ta="center" c="dimmed" py="xl">
              منویی وجود ندارد
            </Text>
          ) : (
            menuItems.map((item) => renderMenuItem(item, 0))
          )}
        </Stack>
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          setEditingItem(null);
        }}
        title={editingItem ? 'ویرایش منو' : 'افزودن منو'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <MenuItemEditor
          menuItem={editingItem || undefined}
          allMenuItems={menuItems}
          value={formData}
          onChange={setFormData}
        />
      </FormModal>
    </Stack>
  );
}
