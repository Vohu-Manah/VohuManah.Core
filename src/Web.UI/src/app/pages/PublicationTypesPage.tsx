import { useState, useEffect, useCallback } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { FormModal } from '@/app/components/common/FormModal';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { publicationTypeService } from '@/app/services/library/publicationTypeService';
import type { PublicationType, CreatePublicationTypeRequest } from '@/app/models/publicationType';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { useAuth } from '@/app/context/AuthContext';
import { PERM } from '@/app/models/permissions';

export default function PublicationTypesPage() {
  const { can } = useAuth();
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

  const loadData = useCallback(async () => {
    try {
      const data = await publicationTypeService.getAll();
      setPublicationTypes(data);
    } catch (error) {
      appMessage.error('خطا در بارگذاری انواع نشریات', getErrorMessage(error));
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
        if (editingType) {
          await publicationTypeService.update(editingType.id, { ...form.values });
          appMessage.success('نوع نشریه با موفقیت به‌روزرسانی شد');
        } else {
          await publicationTypeService.create(form.values);
          appMessage.success('نوع نشریه با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingType(null);
        loadData();
      } catch (error) {
        appMessage.error('خطا در ذخیره نوع نشریه', getErrorMessage(error));
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
    } catch (error) {
      appMessage.error('خطا در بارگذاری نوع نشریه', getErrorMessage(error));
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
      } catch (error) {
        appMessage.error('خطا در حذف نوع نشریه', getErrorMessage(error));
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
          {can(PERM.PublicationType.update) && (
            <ActionIcon color="blue" onClick={() => handleEdit(row.id)}>
              <FaEdit />
            </ActionIcon>
          )}
          {can(PERM.PublicationType.del) && (
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
        <Title order={2}>مدیریت انواع نشریات</Title>
        {can(PERM.PublicationType.create) && (
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
        )}
      </Group>

      <Card withBorder>
        <ProTable
          data={publicationTypes}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['title']}
          exportFileName="publication-types"
          loading={loading}
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
        <SmartTextInput label="عنوان" required withAsterisk fieldName="title" {...form.getInputProps('title')} />
      </FormModal>
    </Stack>
  );
}

