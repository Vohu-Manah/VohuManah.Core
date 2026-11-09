import { useState, useEffect, useMemo } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon, Modal } from '@mantine/core';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { FormModal } from '@/app/components/common/FormModal';
import { roleService } from '@/app/services/library/roleService';
import type { Role, CreateRoleRequest, UpdateRolePermissionsRequest } from '@/app/models/role';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { handleApiError } from '@/app/helpers/errorHandler';

type PermissionItem = {
  name: string;
};

export default function RolesPage() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [permissionsModalOpened, setPermissionsModalOpened] = useState(false);
  const [editingRole, setEditingRole] = useState<Role | null>(null);
  const [selectedRole, setSelectedRole] = useState<Role | null>(null);
  const [selectedPermissions, setSelectedPermissions] = useState<string[]>([]);
  const [selectedPermissionItems, setSelectedPermissionItems] = useState<PermissionItem[]>([]);
  const [endpoints, setEndpoints] = useState<Record<string, string[]>>({});

  const form = useForm<CreateRoleRequest>({
    initialValues: {
      name: '',
    },
    validate: {
      name: isNotEmpty('نام نقش الزامی است'),
    },
  });

  useEffect(() => {
    loadData();
    loadEndpoints();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await roleService.getAll();
      setRoles(data);
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری نقش‌ها');
    } finally {
      setLoading(false);
    }
  };

  const loadEndpoints = async () => {
    try {
      const data = await roleService.getAllEndpoints();
      setEndpoints(data);
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری لیست endpoints');
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        await roleService.create(form.values);
        appMessage.success('نقش با موفقیت ایجاد شد');
        setModalOpened(false);
        form.reset();
        loadData();
      } catch (error) {
        handleApiError(error, 'خطا در ایجاد نقش');
      } finally {
        setLoading(false);
      }
    }
  };

  const handleDelete = async () => {
    if (window.confirm('آیا از حذف این نقش اطمینان دارید؟')) {
      try {
        setLoading(true);
        appMessage.warning('قابلیت حذف نقش به زودی اضافه خواهد شد');
      } catch (error) {
        handleApiError(error, 'خطا در حذف نقش');
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEditPermissions = async (role: Role) => {
    try {
      setLoading(true);
      setSelectedRole(role);
      const permissions = await roleService.getRolePermissions(role.id);
      setSelectedPermissions(permissions);
      const selectedItems = permissionItems.filter((item) => permissions.includes(item.name));
      setSelectedPermissionItems(selectedItems);
      setPermissionsModalOpened(true);
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری مجوزها');
    } finally {
      setLoading(false);
    }
  };

  const handleSavePermissions = async () => {
    if (!selectedRole) return;

    try {
      setLoading(true);
      const data: UpdateRolePermissionsRequest = {
        roleId: selectedRole.id,
        endpointNames: selectedPermissions,
      };
      await roleService.updatePermissions(data);
      appMessage.success('مجوزها با موفقیت به‌روزرسانی شدند');
      setPermissionsModalOpened(false);
      setSelectedRole(null);
      setSelectedPermissions([]);
      setSelectedPermissionItems([]);
      loadData();
    } catch (error) {
      handleApiError(error, 'خطا در ذخیره مجوزها');
    } finally {
      setLoading(false);
    }
  };

  const columns: ProTableColumn<Role>[] = [
    { key: 'name', title: 'نام نقش' },
    {
      key: 'actions',
      title: 'عملیات',
      render: (row) => (
        <Group gap="xs">
          <ActionIcon color="blue" onClick={() => handleEditPermissions(row)}>
            <FaEdit />
          </ActionIcon>
          <ActionIcon color="red" onClick={() => handleDelete()}>
            <FaTrash />
          </ActionIcon>
        </Group>
      ),
    },
  ];

  const allEndpoints = useMemo(() => Object.values(endpoints).flat(), [endpoints]);

  const permissionItems = useMemo(() => {
    return allEndpoints.map((ep) => ({ name: ep }));
  }, [allEndpoints]);

  const permissionColumns: ProTableColumn<PermissionItem>[] = [
    { key: 'name', title: 'نام مجوز' },
  ];

  const handlePermissionSelectionChange = (selected: PermissionItem[]) => {
    setSelectedPermissionItems(selected);
    setSelectedPermissions(selected.map((p) => p.name));
  };

  return (
    <Stack p="md">
      <Title order={2}>مدیریت نقش‌ها</Title>

      <Group justify="space-between">
        <Button
          leftSection={<FaPlus />}
          onClick={() => {
            form.reset();
            setEditingRole(null);
            setModalOpened(true);
          }}
        >
          افزودن نقش جدید
        </Button>
      </Group>

      <Card withBorder>
        <ProTable
          data={roles}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['name']}
          exportFileName="roles"
          loading={loading}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingRole(null);
        }}
        title={editingRole ? 'ویرایش نقش' : 'افزودن نقش جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <SmartTextInput
          label="نام نقش"
          required
          withAsterisk
          fieldName="name"
          {...form.getInputProps('name')}
        />
      </FormModal>

      <Modal
        opened={permissionsModalOpened}
        onClose={() => {
          setPermissionsModalOpened(false);
          setSelectedRole(null);
          setSelectedPermissions([]);
          setSelectedPermissionItems([]);
        }}
        title={`مدیریت مجوزهای نقش: ${selectedRole?.name}`}
        size="xl"
      >
        <Stack gap="md">
          <ProTable
            data={permissionItems}
            columns={permissionColumns}
            searchableKeys={['name']}
            exportFileName="permissions"
            loading={false}
            selectionMode="multi"
            selectedRows={selectedPermissionItems}
            onSelectionChange={handlePermissionSelectionChange}
            getRowId={(row) => row.name}
            enableExcelExport={false}
            enablePdfExport={false}
            itemsPerPage={15}
          />
          <Group justify="flex-end">
            <Button onClick={handleSavePermissions} loading={loading}>
              ذخیره مجوزها
            </Button>
          </Group>
        </Stack>
      </Modal>
    </Stack>
  );
}
