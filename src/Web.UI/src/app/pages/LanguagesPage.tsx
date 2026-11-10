import { useState, useEffect, useCallback } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { FormModal } from '@/app/components/common/FormModal';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { languageService } from '@/app/services/library/languageService';
import type { Language, CreateLanguageRequest } from '@/app/models/language';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { useAuth } from '@/app/context/AuthContext';
import { PERM } from '@/app/models/permissions';

export default function LanguagesPage() {
  const { can } = useAuth();
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

  const loadData = useCallback(async () => {
    try {
      const data = await languageService.getAll();
      setLanguages(data);
    } catch (error) {
      appMessage.error('خطا در بارگذاری زبان‌ها', getErrorMessage(error));
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
        if (editingLanguage) {
          await languageService.update(editingLanguage.id, { ...form.values });
          appMessage.success('زبان با موفقیت به‌روزرسانی شد');
        } else {
          await languageService.create(form.values);
          appMessage.success('زبان با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingLanguage(null);
        loadData();
      } catch (error) {
        appMessage.error('خطا در ذخیره زبان', getErrorMessage(error));
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
    } catch (error) {
      appMessage.error('خطا در بارگذاری زبان', getErrorMessage(error));
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
      } catch (error) {
        appMessage.error('خطا در حذف زبان', getErrorMessage(error));
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
          {can(PERM.Language.update) && (
            <ActionIcon color="blue" onClick={() => handleEdit(row.id)}>
              <FaEdit />
            </ActionIcon>
          )}
          {can(PERM.Language.del) && (
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
        <Title order={2}>مدیریت زبان‌ها</Title>
        {can(PERM.Language.create) && (
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
        )}
      </Group>

      <Card withBorder>
        <ProTable
          data={languages}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['name', 'abbreviation']}
          exportFileName="languages"
          loading={loading}
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
        <SmartTextInput label="نام زبان" required withAsterisk fieldName="name" {...form.getInputProps('name')} />
        <SmartTextInput label="مخفف" required withAsterisk fieldName="abbreviation" {...form.getInputProps('abbreviation')} />
      </FormModal>
    </Stack>
  );
}

