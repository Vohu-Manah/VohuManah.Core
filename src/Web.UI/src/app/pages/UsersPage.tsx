import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon, PasswordInput } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { FormModal } from '@/app/components/common/FormModal';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { userService } from '@/app/services/library/userService';
import type { LibraryUser, CreateUserRequest } from '@/app/models/user';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { handleApiError, getErrorMessage} from '@/app/helpers/errorHandler';

export default function UsersPage() {
  const [users, setUsers] = useState<LibraryUser[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingUser, setEditingUser] = useState<LibraryUser | null>(null);

  const form = useForm<CreateUserRequest>({
    initialValues: {
      userName: '',
      password: '',
      name: '',
      lastName: '',
    },
    validate: {
      userName: isNotEmpty('نام کاربری الزامی است'),
      password: isNotEmpty('رمز عبور الزامی است'),
      name: isNotEmpty('نام الزامی است'),
      lastName: isNotEmpty('نام خانوادگی الزامی است'),
    },
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await userService.getList();
      setUsers(data);
    } catch (error) {
      appMessage.error('خطا در بارگذاری کاربران', getErrorMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    if (editingUser && !form.values.password) {
      form.clearFieldError('password');
    }

    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingUser) {
          await userService.update(editingUser.id, {
            userName: form.values.userName,
            password: form.values.password || '',
            name: form.values.name,
            lastName: form.values.lastName,
          });
          appMessage.success('کاربر با موفقیت به‌روزرسانی شد');
        } else {
          await userService.create(form.values);
          appMessage.success('کاربر با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingUser(null);
        loadData();
      } catch (error) {
        handleApiError(error, 'خطا در ذخیره کاربر');
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEdit = async (userName: string) => {
    try {
      setLoading(true);
      const user = await userService.getByUserName(userName);
      setEditingUser(user);
      form.setValues({
        userName: user.userName,
        password: '',
        name: user.name,
        lastName: user.lastName,
      });
      setModalOpened(true);
    } catch (error) {
      appMessage.error('خطا در بارگذاری کاربر', getErrorMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number, userName: string) => {
    if (window.confirm(`آیا از حذف کاربر «${userName}» مطمئن هستید؟`)) {
      try {
        setLoading(true);
        await userService.delete(id);
        appMessage.success('کاربر با موفقیت حذف شد');
        loadData();
      } catch (error) {
        appMessage.error('خطا در حذف کاربر', getErrorMessage(error));
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<LibraryUser>[] = [
    { key: 'userName', title: 'نام کاربری' },
    { key: 'name', title: 'نام' },
    { key: 'lastName', title: 'نام خانوادگی' },
    { key: 'fullName', title: 'نام کامل' },
    {
      key: 'actions',
      title: 'عملیات',
      render: (row) => (
        <Group gap="xs">
          <ActionIcon color="blue" onClick={() => handleEdit(row.userName)}>
            <FaEdit />
          </ActionIcon>
          <ActionIcon color="red" onClick={() => handleDelete(row.id, row.userName)}>
            <FaTrash />
          </ActionIcon>
        </Group>
      ),
    },
  ];

  return (
    <Stack p="md">
      <Group justify="space-between">
        <Title order={2}>مدیریت کاربران</Title>
        <Button
          leftSection={<FaPlus />}
          onClick={() => {
            form.reset();
            setEditingUser(null);
            setModalOpened(true);
          }}
        >
          افزودن کاربر جدید
        </Button>
      </Group>

      <Card withBorder>
        <ProTable
          data={users}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['userName', 'name', 'lastName', 'fullName']}
          exportFileName="users"
          loading={loading}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingUser(null);
        }}
        title={editingUser ? 'ویرایش کاربر' : 'افزودن کاربر جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <SmartTextInput
          label="نام کاربری"
          required
          readOnly={!!editingUser}
          withAsterisk
          fieldName="userName"
          {...form.getInputProps('userName')}
        />
        <PasswordInput
          label="رمز عبور"
          required={!editingUser}
          withAsterisk={!editingUser}
          dir="ltr"
          {...form.getInputProps('password')}
        />
        <Group grow>
          <SmartTextInput
            label="نام"
            required
            withAsterisk
            fieldName="name"
            {...form.getInputProps('name')}
          />
          <SmartTextInput
            label="نام خانوادگی"
            required
            withAsterisk
            fieldName="lastName"
            {...form.getInputProps('lastName')}
          />
        </Group>
      </FormModal>
    </Stack>
  );
}
