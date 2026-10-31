import { useState, useEffect } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { FormModal } from '@/app/components/common/FormModal';
import { Textarea, Switch } from '@mantine/core';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { gapService } from '@/app/services/library/gapService';
import type { Gap, CreateGapRequest } from '@/app/models/gap';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';

export default function GapsPage() {
  const [gaps, setGaps] = useState<Gap[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingGap, setEditingGap] = useState<Gap | null>(null);

  const form = useForm<CreateGapRequest>({
    initialValues: {
      title: '',
      note: '',
      state: true,
    },
    validate: {
      title: isNotEmpty('عنوان فاصله الزامی است'),
    },
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await gapService.getAll();
      setGaps(data);
    } catch (error) {
      appMessage.error('خطا در بارگذاری فاصله‌ها', getErrorMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingGap) {
          await gapService.update(editingGap.id, { ...form.values });
          appMessage.success('فاصله با موفقیت به‌روزرسانی شد');
        } else {
          await gapService.create(form.values);
          appMessage.success('فاصله با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingGap(null);
        loadData();
      } catch (error) {
        appMessage.error('خطا در ذخیره فاصله', getErrorMessage(error));
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEdit = async (id: number) => {
    try {
      setLoading(true);
      const gap = await gapService.getById(id);
      setEditingGap(gap);
      form.setValues({
        title: gap.title,
        note: gap.note,
        state: gap.state,
      });
      setModalOpened(true);
    } catch (error) {
      appMessage.error('خطا در بارگذاری فاصله', getErrorMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این فاصله مطمئن هستید؟')) {
      try {
        setLoading(true);
        await gapService.delete(id);
        appMessage.success('فاصله با موفقیت حذف شد');
        loadData();
      } catch (error) {
        appMessage.error('خطا در حذف فاصله', getErrorMessage(error));
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<Gap>[] = [
    { key: 'id', title: 'شناسه' },
    { key: 'title', title: 'عنوان' },
    { key: 'note', title: 'یادداشت' },
    {
      key: 'state',
      title: 'وضعیت',
      render: (row) => (row.state ? 'فعال' : 'غیرفعال'),
    },
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
        <Title order={2}>مدیریت فاصله‌ها</Title>
        <Button
          leftSection={<FaPlus />}
          onClick={() => {
            form.reset();
            setEditingGap(null);
            setModalOpened(true);
          }}
        >
          افزودن فاصله جدید
        </Button>
      </Group>

      <Card withBorder>
        <ProTable
          data={gaps}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['title', 'note']}
          exportFileName="gaps"
          loading={loading}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingGap(null);
        }}
        title={editingGap ? 'ویرایش فاصله' : 'افزودن فاصله جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <SmartTextInput label="عنوان" required withAsterisk fieldName="title" {...form.getInputProps('title')} />
        <Textarea label="یادداشت" dir="rtl" {...form.getInputProps('note')} />
        <Switch label="وضعیت فعال" {...form.getInputProps('state', { type: 'checkbox' })} />
      </FormModal>
    </Stack>
  );
}


