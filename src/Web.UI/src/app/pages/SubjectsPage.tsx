import { useState, useEffect, useCallback } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { FormModal } from '@/app/components/common/FormModal';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { subjectService } from '@/app/services/library/subjectService';
import type { Subject, CreateSubjectRequest } from '@/app/models/subject';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { useAuth } from '@/app/context/AuthContext';
import { PERM } from '@/app/models/permissions';

export default function SubjectsPage() {
  const { can } = useAuth();
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingSubject, setEditingSubject] = useState<Subject | null>(null);

  const form = useForm<CreateSubjectRequest>({
    initialValues: {
      title: '',
    },
    validate: {
      title: isNotEmpty('عنوان موضوع الزامی است'),
    },
  });

  const loadData = useCallback(async () => {
    try {
      const data = await subjectService.getAll();
      setSubjects(data);
    } catch (error) {
      appMessage.error('خطا در بارگذاری موضوعات', getErrorMessage(error));
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
        if (editingSubject) {
          await subjectService.update(editingSubject.id, { ...form.values });
          appMessage.success('موضوع با موفقیت به‌روزرسانی شد');
        } else {
          await subjectService.create(form.values);
          appMessage.success('موضوع با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingSubject(null);
        loadData();
      } catch (error) {
        appMessage.error('خطا در ذخیره موضوع', getErrorMessage(error));
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEdit = async (id: number) => {
    try {
      setLoading(true);
      const subject = await subjectService.getById(id);
      setEditingSubject(subject);
      form.setValues({ title: subject.title });
      setModalOpened(true);
    } catch (error) {
      appMessage.error('خطا در بارگذاری موضوع', getErrorMessage(error));
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این موضوع مطمئن هستید؟')) {
      try {
        setLoading(true);
        await subjectService.delete(id);
        appMessage.success('موضوع با موفقیت حذف شد');
        loadData();
      } catch (error) {
        appMessage.error('خطا در حذف موضوع', getErrorMessage(error));
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<Subject>[] = [
    { key: 'id', title: 'شناسه' },
    { key: 'title', title: 'عنوان موضوع' },
    {
      key: 'actions',
      title: 'عملیات',
      render: (row) => (
        <Group gap="xs">
          {can(PERM.Subject.update) && (
            <ActionIcon color="blue" onClick={() => handleEdit(row.id)}>
              <FaEdit />
            </ActionIcon>
          )}
          {can(PERM.Subject.del) && (
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
        <Title order={2}>مدیریت موضوعات</Title>
        {can(PERM.Subject.create) && (
          <Button
            leftSection={<FaPlus />}
            onClick={() => {
              form.reset();
              setEditingSubject(null);
              setModalOpened(true);
            }}
          >
            افزودن موضوع جدید
          </Button>
        )}
      </Group>

      <Card withBorder>
        <ProTable
          data={subjects}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['title']}
          exportFileName="subjects"
          loading={loading}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingSubject(null);
        }}
        title={editingSubject ? 'ویرایش موضوع' : 'افزودن موضوع جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <SmartTextInput label="عنوان موضوع" required withAsterisk fieldName="title" {...form.getInputProps('title')} />
      </FormModal>
    </Stack>
  );
}

