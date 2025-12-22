import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { FormModal } from '../components/common/FormModal';
import { TextInput, PasswordInput } from '@mantine/core';
import { userService } from '../services/library/userService';
import type { LibraryUser, CreateUserRequest } from '../models/user';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { handleApiError } from '../helpers/errorHandler';

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
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری کاربران', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    // در حالت ویرایش، password رو validate نکن اگر خالی بود
    if (editingUser && !form.values.password) {
      form.clearFieldError('password');
    }
    
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingUser) {
          // در حالت ویرایش، password رو همیشه بفرست (backend باید handle کنه)
          await userService.update({
            userName: form.values.userName,
            password: form.values.password || '', // Backend باید handle کنه
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
      } catch (error: any) {
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
        password: '', // Don't load password
        name: user.name,
        lastName: user.lastName,
      });
      setModalOpened(true);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری کاربر', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (userName: string) => {
    if (window.confirm('آیا از حذف این کاربر مطمئن هستید؟')) {
      try {
        setLoading(true);
        await userService.delete(userName);
        appMessage.success('کاربر با موفقیت حذف شد');
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در حذف کاربر', error.message);
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
          <ActionIcon color="red" onClick={() => handleDelete(row.userName)}>
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
        <TextInput
          label="نام کاربری"
          required
          readOnly={!!editingUser}
          withAsterisk
          {...form.getInputProps('userName')}
        />
        <PasswordInput
          label="رمز عبور"
          required={!editingUser}
          withAsterisk={!editingUser}
          {...form.getInputProps('password')}
        />
        <Group grow>
          <TextInput label="نام" required withAsterisk {...form.getInputProps('name')} />
          <TextInput label="نام خانوادگی" required withAsterisk {...form.getInputProps('lastName')} />
        </Group>
      </FormModal>
    </Stack>
  );
}

