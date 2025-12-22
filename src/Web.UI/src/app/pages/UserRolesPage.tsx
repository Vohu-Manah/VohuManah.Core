import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, Select, MultiSelect } from '@mantine/core';
import { userService } from '../services/library/userService';
import { roleService } from '../services/library/roleService';
import type { LibraryUser } from '../models/user';
import type { Role } from '../models/role';
import { appMessage } from '../messages/appMessages';
import { handleApiError } from '../helpers/errorHandler';

export default function UserRolesPage() {
  const [users, setUsers] = useState<LibraryUser[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
  const [selectedRoleIds, setSelectedRoleIds] = useState<number[]>([]);
  const [userRoles, setUserRoles] = useState<number[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadData();
  }, []);

  useEffect(() => {
    if (selectedUserId) {
      loadUserRoles();
    }
  }, [selectedUserId]);

  const loadData = async () => {
    try {
      setLoading(true);
      const [usersData, rolesData] = await Promise.all([
        userService.getList(),
        roleService.getAll(),
      ]);
      setUsers(usersData);
      setRoles(rolesData);
    } catch (error: any) {
      handleApiError(error, 'خطا در بارگذاری داده‌ها');
    } finally {
      setLoading(false);
    }
  };

  const loadUserRoles = async () => {
    if (!selectedUserId) return;
    
    try {
      setLoading(true);
      const roleIds = await roleService.getUserRoleIds(selectedUserId);
      setUserRoles(roleIds);
      setSelectedRoleIds(roleIds);
    } catch (error: any) {
      handleApiError(error, 'خطا در بارگذاری نقش‌های کاربر');
    } finally {
      setLoading(false);
    }
  };

  const handleSaveRoles = async () => {
    if (!selectedUserId) {
      appMessage.warning('لطفا کاربری را انتخاب کنید');
      return;
    }

    try {
      setLoading(true);
      
      // Get current roles for user
      const currentRoleIds = userRoles;
      
      // Find roles to add
      const rolesToAdd = selectedRoleIds.filter(id => !currentRoleIds.includes(id));
      
      // Find roles to remove
      const rolesToRemove = currentRoleIds.filter(id => !selectedRoleIds.includes(id));
      
      // Add new roles
      for (const roleId of rolesToAdd) {
        await roleService.assignRoleToUser(selectedUserId, roleId);
      }
      
      // Remove old roles
      for (const roleId of rolesToRemove) {
        await roleService.removeRoleFromUser(selectedUserId, roleId);
      }
      
      appMessage.success('نقش‌های کاربر با موفقیت به‌روزرسانی شدند');
      setUserRoles(selectedRoleIds);
      loadUserRoles();
    } catch (error: any) {
      handleApiError(error, 'خطا در ذخیره نقش‌ها');
    } finally {
      setLoading(false);
    }
  };

  const userOptions = users
    .filter(u => u && u.id != null && u.id !== undefined && u.name != null && u.lastName != null && u.userName != null)
    .map(u => ({
      value: String(u.id),
      label: `${u.name || ''} ${u.lastName || ''} (${u.userName || ''})`.trim() || `User ${u.id}`,
    }))
    .filter((option, index, self) => 
      index === self.findIndex(o => o.value === option.value && o.value !== 'undefined')
    );

  const roleOptions = roles
    .filter(r => r && r.id != null && r.id !== undefined && r.name != null)
    .map(r => ({
      value: String(r.id),
      label: r.name || `Role ${r.id}`,
    }))
    .filter((option, index, self) => 
      index === self.findIndex(o => o.value === option.value && o.value !== 'undefined')
    );

  return (
    <Stack p="md">
      <Title order={2}>مدیریت نقش‌های کاربران</Title>

      <Card withBorder>
        <Stack gap="md">
          <Select
            label="کاربر"
            placeholder="کاربر را انتخاب کنید"
            data={userOptions}
            value={selectedUserId ? String(selectedUserId) : null}
            onChange={(value) => {
              setSelectedUserId(value ? Number(value) : null);
              setSelectedRoleIds([]);
            }}
            searchable
            clearable
          />

          {selectedUserId && (
            <>
              <MultiSelect
                label="نقش‌ها"
                placeholder="نقش‌ها را انتخاب کنید"
                data={roleOptions}
                value={selectedRoleIds.map(String)}
                onChange={(values) => {
                  if (values && Array.isArray(values)) {
                    const validIds = values
                      .map(v => {
                        const num = Number(v);
                        return !isNaN(num) && num > 0 ? num : null;
                      })
                      .filter((id): id is number => id !== null);
                    setSelectedRoleIds(validIds);
                  } else {
                    setSelectedRoleIds([]);
                  }
                }}
                searchable
                clearable
              />

              <Group justify="flex-end">
                <Button onClick={handleSaveRoles} loading={loading}>
                  ذخیره نقش‌ها
                </Button>
              </Group>
            </>
          )}
        </Stack>
      </Card>
    </Stack>
  );
}

