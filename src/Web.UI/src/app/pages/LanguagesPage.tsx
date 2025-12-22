import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { FormModal } from '../components/common/FormModal';
import { TextInput } from '@mantine/core';
import { languageService } from '../services/library/languageService';
import type { Language, CreateLanguageRequest } from '../models/language';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';

export default function LanguagesPage() {
  const [languages, setLanguages] = useState<Language[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingLanguage, setEditingLanguage] = useState<Language | null>(null);

  const form = useForm<CreateLanguageRequest>({
    initialValues: {
      name: '',
      abbreviation: '',
    },
    validate: {
      name: isNotEmpty('نام زبان الزامی است'),
      abbreviation: isNotEmpty('مخفف الزامی است'),
    },
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await languageService.getAll();
      setLanguages(data);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری زبان‌ها', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingLanguage) {
          await languageService.update({ ...form.values, id: editingLanguage.id });
          appMessage.success('زبان با موفقیت به‌روزرسانی شد');
        } else {
          await languageService.create(form.values);
          appMessage.success('زبان با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingLanguage(null);
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در ذخیره زبان', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEdit = async (id: number) => {
    try {
      setLoading(true);
      const language = await languageService.getById(id);
      setEditingLanguage(language);
      form.setValues({ name: language.name, abbreviation: language.abbreviation });
      setModalOpened(true);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری زبان', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این زبان مطمئن هستید؟')) {
      try {
        setLoading(true);
        await languageService.delete(id);
        appMessage.success('زبان با موفقیت حذف شد');
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در حذف زبان', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<Language>[] = [
    { key: 'id', title: 'شناسه' },
    { key: 'name', title: 'نام زبان' },
    { key: 'abbreviation', title: 'مخفف' },
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
        <Title order={2}>مدیریت زبان‌ها</Title>
        <Button
          leftSection={<FaPlus />}
          onClick={() => {
            form.reset();
            setEditingLanguage(null);
            setModalOpened(true);
          }}
        >
          افزودن زبان جدید
        </Button>
      </Group>

      <Card withBorder>
        <ProTable
          data={languages}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['name', 'abbreviation']}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingLanguage(null);
        }}
        title={editingLanguage ? 'ویرایش زبان' : 'افزودن زبان جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <TextInput label="نام زبان" required {...form.getInputProps('name')} />
        <TextInput label="مخفف" required {...form.getInputProps('abbreviation')} />
      </FormModal>
    </Stack>
  );
}


