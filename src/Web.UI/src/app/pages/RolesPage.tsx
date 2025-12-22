import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon, TextInput, Modal, MultiSelect, Badge } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash, FaShieldAlt } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { FormModal } from '../components/common/FormModal';
import { roleService } from '../services/library/roleService';
import type { Role, CreateRoleRequest, UpdateRolePermissionsRequest } from '../models/role';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { handleApiError } from '../helpers/errorHandler';
import { ENDPOINT_CATALOG } from '../constants/endpointCatalog';

export default function RolesPage() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [permissionsModalOpened, setPermissionsModalOpened] = useState(false);
  const [editingRole, setEditingRole] = useState<Role | null>(null);
  const [selectedRole, setSelectedRole] = useState<Role | null>(null);
  const [selectedPermissions, setSelectedPermissions] = useState<string[]>([]);

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
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await roleService.getAll();
      setRoles(data);
    } catch (error: any) {
      handleApiError(error, 'خطا در بارگذاری نقش‌ها');
    } finally {
      setLoading(false);
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
      } catch (error: any) {
        handleApiError(error, 'خطا در ایجاد نقش');
      } finally {
        setLoading(false);
      }
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این نقش اطمینان دارید؟')) {
      try {
        setLoading(true);
        // TODO: Add delete endpoint
        appMessage.warning('قابلیت حذف نقش به زودی اضافه خواهد شد');
      } catch (error: any) {
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
      setPermissionsModalOpened(true);
    } catch (error: any) {
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
      loadData();
    } catch (error: any) {
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
          <ActionIcon color="red" onClick={() => handleDelete(row.id)}>
            <FaTrash />
          </ActionIcon>
        </Group>
      ),
    },
  ];

  // Flatten all endpoints from catalog
  const allEndpoints = Object.values(ENDPOINT_CATALOG).flat();

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
        <TextInput
          label="نام نقش"
          required
          withAsterisk
          {...form.getInputProps('name')}
        />
      </FormModal>

      <Modal
        opened={permissionsModalOpened}
        onClose={() => {
          setPermissionsModalOpened(false);
          setSelectedRole(null);
          setSelectedPermissions([]);
        }}
        title={`مدیریت مجوزهای نقش: ${selectedRole?.name}`}
        size="xl"
      >
        <Stack gap="md">
          <MultiSelect
            label="مجوزها"
            placeholder="مجوزها را انتخاب کنید"
            data={allEndpoints
              .filter(ep => ep != null && ep.trim() !== '')
              .map(ep => ({ value: ep, label: ep }))}
            value={selectedPermissions || []}
            onChange={(values) => setSelectedPermissions(values ? values.filter(v => v != null) : [])}
            searchable
            clearable
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

