import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { FormModal } from '../components/common/FormModal';
import { TextInput } from '@mantine/core';
import { subjectService } from '../services/library/subjectService';
import type { Subject, CreateSubjectRequest } from '../models/subject';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';

export default function SubjectsPage() {
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

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await subjectService.getAll();
      setSubjects(data);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری موضوعات', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingSubject) {
          await subjectService.update({ ...form.values, id: editingSubject.id });
          appMessage.success('موضوع با موفقیت به‌روزرسانی شد');
        } else {
          await subjectService.create(form.values);
          appMessage.success('موضوع با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingSubject(null);
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در ذخیره موضوع', error.message);
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
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری موضوع', error.message);
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
      } catch (error: any) {
        appMessage.error('خطا در حذف موضوع', error.message);
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
        <Title order={2}>مدیریت موضوعات</Title>
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
      </Group>

      <Card withBorder>
        <ProTable
          data={subjects}
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
          setEditingSubject(null);
        }}
        title={editingSubject ? 'ویرایش موضوع' : 'افزودن موضوع جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <TextInput label="عنوان موضوع" required {...form.getInputProps('title')} />
      </FormModal>
    </Stack>
  );
}


