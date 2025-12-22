import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { FormModal } from '../components/common/FormModal';
import { TextInput } from '@mantine/core';
import { publicationTypeService } from '../services/library/publicationTypeService';
import type { PublicationType, CreatePublicationTypeRequest } from '../models/publicationType';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';

export default function PublicationTypesPage() {
  const [publicationTypes, setPublicationTypes] = useState<PublicationType[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingType, setEditingType] = useState<PublicationType | null>(null);

  const form = useForm<CreatePublicationTypeRequest>({
    initialValues: {
      title: '',
    },
    validate: {
      title: isNotEmpty('عنوان نوع نشریه الزامی است'),
    },
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await publicationTypeService.getAll();
      setPublicationTypes(data);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری انواع نشریات', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingType) {
          await publicationTypeService.update({ ...form.values, id: editingType.id });
          appMessage.success('نوع نشریه با موفقیت به‌روزرسانی شد');
        } else {
          await publicationTypeService.create(form.values);
          appMessage.success('نوع نشریه با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingType(null);
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در ذخیره نوع نشریه', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEdit = async (id: number) => {
    try {
      setLoading(true);
      const type = await publicationTypeService.getById(id);
      setEditingType(type);
      form.setValues({ title: type.title });
      setModalOpened(true);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری نوع نشریه', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این نوع نشریه مطمئن هستید؟')) {
      try {
        setLoading(true);
        await publicationTypeService.delete(id);
        appMessage.success('نوع نشریه با موفقیت حذف شد');
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در حذف نوع نشریه', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<PublicationType>[] = [
    { key: 'id', title: 'شناسه' },
    { key: 'title', title: 'عنوان' },
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
        <Title order={2}>مدیریت انواع نشریات</Title>
        <Button
          leftSection={<FaPlus />}
          onClick={() => {
            form.reset();
            setEditingType(null);
            setModalOpened(true);
          }}
        >
          افزودن نوع جدید
        </Button>
      </Group>

      <Card withBorder>
        <ProTable
          data={publicationTypes}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['title']}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingType(null);
        }}
        title={editingType ? 'ویرایش نوع نشریه' : 'افزودن نوع نشریه جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <TextInput label="عنوان" required {...form.getInputProps('title')} />
      </FormModal>
    </Stack>
  );
}


