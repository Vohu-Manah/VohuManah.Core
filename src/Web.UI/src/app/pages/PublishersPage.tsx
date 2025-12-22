import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { FormModal } from '../components/common/FormModal';
import { TextInput, Select } from '@mantine/core';
import { publisherService } from '../services/library/publisherService';
import { cityService } from '../services/library/cityService';
import type { Publisher, CreatePublisherRequest } from '../models/publisher';
import type { CityName } from '../models/city';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';

export default function PublishersPage() {
  const [publishers, setPublishers] = useState<Publisher[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingPublisher, setEditingPublisher] = useState<Publisher | null>(null);
  const [cities, setCities] = useState<CityName[]>([]);

  const form = useForm<CreatePublisherRequest>({
    initialValues: {
      name: '',
      managerName: '',
      telephone: '',
      address: '',
      cityId: 0,
    },
    validate: {
      name: isNotEmpty('نام ناشر الزامی است'),
    },
  });

  useEffect(() => {
    loadData();
    loadCities();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await publisherService.getAll();
      setPublishers(data);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری ناشران', error.message);
    } finally {
      setLoading(false);
    }
  };

  const loadCities = async () => {
    try {
      const data = await cityService.getNames();
      setCities(data);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری شهرها', error.message);
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingPublisher) {
          await publisherService.update({ ...form.values, id: editingPublisher.id });
          appMessage.success('ناشر با موفقیت به‌روزرسانی شد');
        } else {
          await publisherService.create(form.values);
          appMessage.success('ناشر با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingPublisher(null);
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در ذخیره ناشر', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEdit = async (id: number) => {
    try {
      setLoading(true);
      const publisher = await publisherService.getById(id);
      setEditingPublisher(publisher);
      form.setValues({
        name: publisher.name,
        managerName: publisher.managerName,
        telephone: publisher.telephone,
        address: publisher.address,
        cityId: publisher.cityId || 0,
      });
      setModalOpened(true);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری ناشر', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این ناشر مطمئن هستید؟')) {
      try {
        setLoading(true);
        await publisherService.delete(id);
        appMessage.success('ناشر با موفقیت حذف شد');
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در حذف ناشر', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<Publisher>[] = [
    { key: 'id', title: 'شناسه' },
    { key: 'name', title: 'نام ناشر' },
    { key: 'managerName', title: 'نام مدیر' },
    { key: 'telephone', title: 'تلفن' },
    { key: 'cityName', title: 'شهر' },
    {
      key: 'actions',
      title: 'عملیات',
      render: (row) => (
        <Group gap="xs">
          <ActionIcon color="blue" onClick={() => handleEdit(row.id)}>
            <FaEdit />
          </ActionIcon>
          <ActionIcon color="red" onClick={() => handleDelete(row.id)}>
            <FaTrash />
          </ActionIcon>
        </Group>
      ),
    },
  ];

  return (
    <Stack p="md">
      <Group justify="space-between">
        <Title order={2}>مدیریت ناشران</Title>
        <Button
          leftSection={<FaPlus />}
          onClick={() => {
            form.reset();
            setEditingPublisher(null);
            setModalOpened(true);
          }}
        >
          افزودن ناشر جدید
        </Button>
      </Group>

      <Card withBorder>
        <ProTable
          data={publishers}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['name', 'managerName']}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingPublisher(null);
        }}
        title={editingPublisher ? 'ویرایش ناشر' : 'افزودن ناشر جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <TextInput label="نام ناشر" required {...form.getInputProps('name')} />
        <Group grow>
          <TextInput label="نام مدیر" {...form.getInputProps('managerName')} />
          <TextInput label="تلفن" {...form.getInputProps('telephone')} />
        </Group>
        <TextInput label="آدرس" {...form.getInputProps('address')} />
        <Select
          label="شهر"
          data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
          {...form.getInputProps('cityId', { type: 'number' })}
        />
      </FormModal>
    </Stack>
  );
}


