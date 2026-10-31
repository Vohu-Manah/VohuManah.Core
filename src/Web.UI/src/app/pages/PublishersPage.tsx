import { useState, useEffect } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { FormModal } from '@/app/components/common/FormModal';
import { Select } from '@mantine/core';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { publisherService } from '@/app/services/library/publisherService';
import { cityService } from '@/app/services/library/cityService';
import type { Publisher, CreatePublisherRequest } from '@/app/models/publisher';
import type { CityName } from '@/app/models/city';
import { appMessage } from '@/app/messages/appMessages';
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
      website: '',
      email: '',
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
    } catch (error) {
      appMessage.error('خطا در بارگذاری ناشران', getErrorMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const loadCities = async () => {
    try {
      const data = await cityService.getNames();
      setCities(data);
    } catch (error) {
      appMessage.error('خطا در بارگذاری شهرها', getErrorMessage(error));
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingPublisher) {
          await publisherService.update(editingPublisher.id, { ...form.values });
          appMessage.success('ناشر با موفقیت به‌روزرسانی شد');
        } else {
          await publisherService.create(form.values);
          appMessage.success('ناشر با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingPublisher(null);
        loadData();
      } catch (error) {
        appMessage.error('خطا در ذخیره ناشر', getErrorMessage(error));
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
        website: publisher.website || '',
        email: publisher.email || '',
        cityId: publisher.cityId || 0,
      });
      setModalOpened(true);
    } catch (error) {
      appMessage.error('خطا در بارگذاری ناشر', getErrorMessage(error));
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
      } catch (error) {
        appMessage.error('خطا در حذف ناشر', getErrorMessage(error));
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
          exportFileName="publishers"
          loading={loading}
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
        <SmartTextInput
          label="نام ناشر"
          required
          withAsterisk
          fieldName="name"
          {...form.getInputProps('name')}
        />
        <Group grow>
          <SmartTextInput
            label="نام مدیر"
            fieldName="managerName"
            {...form.getInputProps('managerName')}
          />
          <SmartTextInput label="تلفن" fieldName="telephone" {...form.getInputProps('telephone')} />
        </Group>
        <SmartTextInput label="آدرس" fieldName="address" {...form.getInputProps('address')} />
        <Group grow>
          <SmartTextInput label="وبسایت" fieldName="website" {...form.getInputProps('website')} />
          <SmartTextInput label="ایمیل" fieldName="email" type="email" {...form.getInputProps('email')} />
        </Group>
        <Select
          label="شهر"
          data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
          value={form.values.cityId ? String(form.values.cityId) : null}
          onChange={(value) => form.setFieldValue('cityId', value ? Number(value) : 0)}
          error={form.errors.cityId}
        />
      </FormModal>
    </Stack>
  );
}
