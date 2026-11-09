import { useState, useEffect, useCallback } from 'react';
import { Button, Card, Stack, Title, Group } from '@mantine/core';
import { userService } from '@/app/services/library/userService';
import { roleService } from '@/app/services/library/roleService';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import type { LibraryUser } from '@/app/models/user';
import type { Role } from '@/app/models/role';
import { appMessage } from '@/app/messages/appMessages';
import { handleApiError } from '@/app/helpers/errorHandler';

export default function UserRolesPage() {
  const [users, setUsers] = useState<LibraryUser[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [selectedUser, setSelectedUser] = useState<LibraryUser | null>(null);
  const [selectedRoleId, setSelectedRoleId] = useState<number | null>(null);
  const [originalRoleId, setOriginalRoleId] = useState<number | null>(null);
  const [loading, setLoading] = useState(false);
  const [loadingUserRoles, setLoadingUserRoles] = useState(false);
  const [showRoles, setShowRoles] = useState(false);
  const [selectedUsers, setSelectedUsers] = useState<LibraryUser[]>([]);
  const [selectedRoles, setSelectedRoles] = useState<Role[]>([]);


  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const [usersData, rolesData] = await Promise.all([
        userService.getList(),
        roleService.getAll(),
      ]);
      setUsers(usersData);
      setRoles(rolesData);
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری داده‌ها');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {

    void loadData();
  }, [loadData]);

  const loadUserRoles = async (userId: number) => {
    if (!userId || userId === undefined || userId === null) {
      appMessage.warning('شناسه کاربر معتبر نیست');
      return;
    }
    try {
      setLoadingUserRoles(true);
      const roleIds = await roleService.getUserRoleIds(userId);
      const firstRoleId = roleIds.length > 0 ? roleIds[0] : null;
      setSelectedRoleId(firstRoleId);
      setOriginalRoleId(firstRoleId);
      if (firstRoleId) {
        const selectedRole = roles.find((r) => r.id === firstRoleId);
        setSelectedRoles(selectedRole ? [selectedRole] : []);
      } else {
        setSelectedRoles([]);
      }
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری نقش‌های کاربر');
    } finally {
      setLoadingUserRoles(false);
    }
  };

  const handleSaveRole = async () => {
    if (!selectedUser?.id) {
      appMessage.warning('لطفا کاربری را انتخاب کنید');
      return;
    }

    try {
      setLoading(true);
      
      if (originalRoleId && originalRoleId !== selectedRoleId) {
        await roleService.removeRoleFromUser(selectedUser.id, originalRoleId);
      }
      
      if (selectedRoleId && selectedRoleId !== originalRoleId) {
        await roleService.assignRoleToUser(selectedUser.id, selectedRoleId);
      }

      appMessage.success('نقش کاربر با موفقیت به‌روزرسانی شد');
      setOriginalRoleId(selectedRoleId);
    } catch (error) {
      handleApiError(error, 'خطا در ذخیره نقش');
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    setSelectedRoleId(originalRoleId);
    if (originalRoleId) {
      const originalRole = roles.find((r) => r.id === originalRoleId);
      setSelectedRoles(originalRole ? [originalRole] : []);
    } else {
      setSelectedRoles([]);
    }
  };

  const handleRoleSelectionChange = (selected: Role[]) => {
    setSelectedRoles(selected);
    if (selected.length > 0) {
      setSelectedRoleId(selected[0].id);
    } else {
      setSelectedRoleId(null);
    }
  };

  const roleColumns: ProTableColumn<Role>[] = [
    { key: 'name', title: 'نام نقش' },
  ];

  const handleBack = () => {
    setShowRoles(false);
    setSelectedUser(null);
    setSelectedRoleId(null);
    setOriginalRoleId(null);
    setSelectedUsers([]);
    setSelectedRoles([]);
  };

  const userColumns: ProTableColumn<LibraryUser>[] = [
    { key: 'id', title: 'شناسه' },
    { key: 'userName', title: 'نام کاربری' },
    { key: 'name', title: 'نام' },
    { key: 'lastName', title: 'نام خانوادگی' },
    { key: 'fullName', title: 'نام کامل' },
  ];

  const handleUserSelect = async (selectedRows: LibraryUser[]) => {
    if (selectedRows.length === 0) return;
    const user = selectedRows[0];
    if (!user.id || user.id === undefined || user.id === null) {
      appMessage.warning('کاربر انتخاب شده شناسه ندارد');
      return;
    }
    setSelectedUser(user);
    setShowRoles(true);
    await loadUserRoles(user.id);
  };


  if (showRoles && selectedUser) {
    return (
      <Stack p="md">
        <Title order={2}>مدیریت نقش کاربر</Title>

        <Card withBorder>
          <Stack gap="md">
            <Group justify="space-between">
              <div>
                <div className="fw-bold mb-4">کاربر انتخاب شده:</div>
                <div>
                  {selectedUser.fullName || `${selectedUser.name || ''} ${selectedUser.lastName || ''}`.trim()} ({selectedUser.userName}) - شناسه: {selectedUser.id || 'نامشخص'}
                </div>
              </div>
              <Button variant="outline" onClick={handleBack}>
                بازگشت
              </Button>
            </Group>

            <ProTable
              data={roles}
              columns={roleColumns}
              searchableKeys={['name']}
              exportFileName="roles"
              loading={loadingUserRoles}
              selectionMode="single"
              selectedRows={selectedRoles}
              onSelectionChange={handleRoleSelectionChange}
              getRowId={(row) => String(row.id)}
              enableExcelExport={false}
              enablePdfExport={false}
              itemsPerPage={15}
            />

            <Group justify="space-between">
              <Button variant="outline" onClick={handleCancel} disabled={selectedRoleId === originalRoleId}>
                انصراف
              </Button>
              <Button onClick={handleSaveRole} loading={loading} disabled={selectedRoleId === originalRoleId}>
                ذخیره نقش
              </Button>
            </Group>
          </Stack>
        </Card>
      </Stack>
    );
  }

  return (
    <Stack p="md">
      <Title order={2}>مدیریت نقش‌های کاربران</Title>

      <Card withBorder>
        <ProTable
          data={users}
          columns={userColumns}
          onRefresh={loadData}
          searchableKeys={['id', 'userName', 'name', 'lastName', 'fullName']}
          exportFileName="users"
          loading={loading}
          selectionMode="single"
          selectedRows={selectedUsers}
          onSelectionChange={(rows) => {
            setSelectedUsers(rows);
            if (rows.length > 0) {
              handleUserSelect(rows);
            }
          }}
          getRowId={(row) => String(row.id ?? '')}
          enableExcelExport={false}
          enablePdfExport={false}
        />
      </Card>
    </Stack>
  );
}