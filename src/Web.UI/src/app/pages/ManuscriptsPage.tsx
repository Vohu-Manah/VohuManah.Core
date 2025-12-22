import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { FormModal } from '../components/common/FormModal';
import { TextInput, Select, NumberInput } from '@mantine/core';
import { manuscriptService } from '../services/library/manuscriptService';
import { cityService } from '../services/library/cityService';
import { languageService } from '../services/library/languageService';
import { subjectService } from '../services/library/subjectService';
import { gapService } from '../services/library/gapService';
import type { Manuscript, CreateManuscriptRequest } from '../models/manuscript';
import type { CityName } from '../models/city';
import type { LanguageName } from '../models/language';
import type { SubjectName } from '../models/subject';
import type { GapName } from '../models/gap';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';

export default function ManuscriptsPage() {
  const [manuscripts, setManuscripts] = useState<Manuscript[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingManuscript, setEditingManuscript] = useState<Manuscript | null>(null);
  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [gaps, setGaps] = useState<GapName[]>([]);

  const form = useForm<CreateManuscriptRequest>({
    initialValues: {
      name: '',
      author: '',
      writingPlaceId: 0,
      year: '',
      pageCount: 0,
      size: '',
      gapId: 0,
      versionNo: '',
      languageId: 0,
      subjectId: 0,
    },
    validate: {
      name: isNotEmpty('نام نسخه خطی الزامی است'),
    },
  });

  useEffect(() => {
    loadData();
    loadLookups();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await manuscriptService.getList();
      setManuscripts(data);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری نسخ خطی', error.message);
    } finally {
      setLoading(false);
    }
  };

  const loadLookups = async () => {
    try {
      const [citiesData, languagesData, subjectsData, gapsData] = await Promise.all([
        cityService.getNames(),
        languageService.getNames(),
        subjectService.getNames(),
        gapService.getNames(),
      ]);
      setCities(citiesData);
      setLanguages(languagesData);
      setSubjects(subjectsData);
      setGaps(gapsData);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری داده‌های کمکی', error.message);
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingManuscript) {
          await manuscriptService.update({ ...form.values, id: editingManuscript.id });
          appMessage.success('نسخه خطی با موفقیت به‌روزرسانی شد');
        } else {
          await manuscriptService.create(form.values);
          appMessage.success('نسخه خطی با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingManuscript(null);
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در ذخیره نسخه خطی', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEdit = async (id: number) => {
    try {
      setLoading(true);
      const manuscript = await manuscriptService.getById(id);
      setEditingManuscript(manuscript);
      form.setValues({
        name: manuscript.name,
        author: manuscript.author,
        writingPlaceId: manuscript.writingPlaceId,
        year: manuscript.year,
        pageCount: manuscript.pageCount,
        size: manuscript.size,
        gapId: manuscript.gapId,
        versionNo: manuscript.versionNo,
        languageId: manuscript.languageId,
        subjectId: manuscript.subjectId,
      });
      setModalOpened(true);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری نسخه خطی', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این نسخه خطی مطمئن هستید؟')) {
      try {
        setLoading(true);
        await manuscriptService.delete(id);
        appMessage.success('نسخه خطی با موفقیت حذف شد');
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در حذف نسخه خطی', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<Manuscript>[] = [
    { key: 'name', title: 'نام نسخه خطی' },
    { key: 'author', title: 'نویسنده' },
    { key: 'gapTitle', title: 'فاصله' },
    { key: 'writingPlaceName', title: 'محل نگارش' },
    { key: 'year', title: 'سال' },
    { key: 'pageCount', title: 'تعداد صفحات' },
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
        <Title order={2}>مدیریت نسخ خطی</Title>
        <Button
          leftSection={<FaPlus />}
          onClick={() => {
            form.reset();
            setEditingManuscript(null);
            setModalOpened(true);
          }}
        >
          افزودن نسخه خطی جدید
        </Button>
      </Group>

      <Card withBorder>
        <ProTable
          data={manuscripts}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['name', 'author', 'gapTitle']}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingManuscript(null);
        }}
        title={editingManuscript ? 'ویرایش نسخه خطی' : 'افزودن نسخه خطی جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <TextInput label="نام نسخه خطی" required {...form.getInputProps('name')} />
        <TextInput label="نویسنده" {...form.getInputProps('author')} />
        <Group grow>
          <Select
            label="فاصله"
            data={gaps.map((g) => ({ value: String(g.id), label: g.title }))}
            {...form.getInputProps('gapId', { type: 'number' })}
          />
          <Select
            label="محل نگارش"
            data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
            {...form.getInputProps('writingPlaceId', { type: 'number' })}
          />
        </Group>
        <Group grow>
          <TextInput label="سال" {...form.getInputProps('year')} />
          <NumberInput label="تعداد صفحات" min={0} {...form.getInputProps('pageCount')} />
          <TextInput label="اندازه" {...form.getInputProps('size')} />
        </Group>
        <TextInput label="شماره نسخه" {...form.getInputProps('versionNo')} />
        <Group grow>
          <Select
            label="زبان"
            data={languages.map((l) => ({ value: String(l.id), label: l.title }))}
            {...form.getInputProps('languageId', { type: 'number' })}
          />
          <Select
            label="موضوع"
            data={subjects.map((s) => ({ value: String(s.id), label: s.title }))}
            {...form.getInputProps('subjectId', { type: 'number' })}
          />
        </Group>
      </FormModal>
    </Stack>
  );
}


