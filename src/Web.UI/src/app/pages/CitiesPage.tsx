import { useState, useEffect, useCallback } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { FormModal } from '@/app/components/common/FormModal';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { cityService } from '@/app/services/library/cityService';
import type { City, CreateCityRequest } from '@/app/models/city';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { useAuth } from '@/app/context/AuthContext';
import { PERM } from '@/app/models/permissions';

export default function CitiesPage() {
  const { can } = useAuth();
  const [cities, setCities] = useState<City[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingCity, setEditingCity] = useState<City | null>(null);

  const form = useForm<CreateCityRequest>({
    initialValues: {
      name: '',
    },
    validate: {
      name: isNotEmpty('نام شهر الزامی است'),
    },
  });

  const loadData = useCallback(async () => {
    try {
      const data = await cityService.getAll();
      setCities(data);
    } catch (error) {
      appMessage.error('خطا در بارگذاری شهرها', getErrorMessage(error));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {

    void loadData();
  }, [loadData]);

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingCity) {
          await cityService.update(editingCity.id, { ...form.values });
          appMessage.success('شهر با موفقیت به‌روزرسانی شد');
        } else {
          await cityService.create(form.values);
          appMessage.success('شهر با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingCity(null);
        loadData();
      } catch (error) {
        appMessage.error('خطا در ذخیره شهر', getErrorMessage(error));
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEdit = async (id: number) => {
    try {
      setLoading(true);
      const city = await cityService.getById(id);
      setEditingCity(city);
      form.setValues({ name: city.name });
      setModalOpened(true);
    } catch (error) {
      appMessage.error('خطا در بارگذاری شهر', getErrorMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این شهر مطمئن هستید؟')) {
      try {
        setLoading(true);
        await cityService.delete(id);
        appMessage.success('شهر با موفقیت حذف شد');
        loadData();
      } catch (error) {
        appMessage.error('خطا در حذف شهر', getErrorMessage(error));
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<City>[] = [
    { key: 'id', title: 'شناسه' },
    { key: 'name', title: 'نام شهر' },
    {
      key: 'actions',
      title: 'عملیات',
      render: (row) => (
        <Group gap="xs">
          {can(PERM.City.update) && (
            <ActionIcon color="blue" onClick={() => handleEdit(row.id)}>
              <FaEdit />
            </ActionIcon>
          )}
          {can(PERM.City.del) && (
            <ActionIcon color="red" onClick={() => handleDelete(row.id)}>
              <FaTrash />
            </ActionIcon>
          )}
        </Group>
      ),
    },
  ];

  return (
    <Stack p="md">
      <Group justify="space-between">
        <Title order={2}>مدیریت شهرها</Title>
        {can(PERM.City.create) && (
          <Button
            leftSection={<FaPlus />}
            onClick={() => {
              form.reset();
              setEditingCity(null);
              setModalOpened(true);
            }}
          >
            افزودن شهر جدید
          </Button>
        )}
      </Group>

      <Card withBorder>
        <ProTable
          data={cities}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['name']}
          exportFileName="cities"
          loading={loading}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingCity(null);
        }}
        title={editingCity ? 'ویرایش شهر' : 'افزودن شهر جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <SmartTextInput label="نام شهر" required withAsterisk fieldName="name" {...form.getInputProps('name')} />
      </FormModal>
    </Stack>
  );
}

