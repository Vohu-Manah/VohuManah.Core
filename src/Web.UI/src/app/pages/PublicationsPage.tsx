import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { FormModal } from '../components/common/FormModal';
import { TextInput, Select } from '@mantine/core';
import { PersianDatePickerString } from '../components/common/PersianDatePickerString';
import { publicationService } from '../services/library/publicationService';
import { cityService } from '../services/library/cityService';
import { languageService } from '../services/library/languageService';
import { subjectService } from '../services/library/subjectService';
import { publicationTypeService } from '../services/library/publicationTypeService';
import type { Publication, CreatePublicationRequest } from '../models/publication';
import type { CityName } from '../models/city';
import type { LanguageName } from '../models/language';
import type { SubjectName } from '../models/subject';
import type { PublicationTypeName } from '../models/publicationType';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';

export default function PublicationsPage() {
  const [publications, setPublications] = useState<Publication[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingPublication, setEditingPublication] = useState<Publication | null>(null);
  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [publicationTypes, setPublicationTypes] = useState<PublicationTypeName[]>([]);

  const form = useForm<CreatePublicationRequest>({
    initialValues: {
      name: '',
      typeId: 0,
      concessionaire: '',
      responsibleDirector: '',
      editor: '',
      year: '',
      journalNo: '',
      publishDate: '',
      publishPlaceId: 0,
      no: '',
      period: '',
      languageId: 0,
      subjectId: 0,
    },
    validate: {
      name: isNotEmpty('نام نشریه الزامی است'),
    },
  });

  useEffect(() => {
    loadData();
    loadLookups();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await publicationService.getList();
      setPublications(data);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری نشریات', error.message);
    } finally {
      setLoading(false);
    }
  };

  const loadLookups = async () => {
    try {
      const [citiesData, languagesData, subjectsData, typesData] = await Promise.all([
        cityService.getNames(),
        languageService.getNames(),
        subjectService.getNames(),
        publicationTypeService.getNames(),
      ]);
      setCities(citiesData);
      setLanguages(languagesData);
      setSubjects(subjectsData);
      setPublicationTypes(typesData);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری داده‌های کمکی', error.message);
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingPublication) {
          await publicationService.update({ ...form.values, id: editingPublication.id });
          appMessage.success('نشریه با موفقیت به‌روزرسانی شد');
        } else {
          await publicationService.create(form.values);
          appMessage.success('نشریه با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingPublication(null);
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در ذخیره نشریه', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const handleEdit = async (id: number) => {
    try {
      setLoading(true);
      const publication = await publicationService.getById(id);
      setEditingPublication(publication);
      form.setValues({
        name: publication.name,
        typeId: publication.typeId,
        concessionaire: publication.concessionaire,
        responsibleDirector: publication.responsibleDirector,
        editor: publication.editor,
        year: publication.year,
        journalNo: publication.journalNo,
        publishDate: publication.publishDate,
        publishPlaceId: publication.publishPlaceId,
        no: publication.no,
        period: publication.period,
        languageId: publication.languageId,
        subjectId: publication.subjectId,
      });
      setModalOpened(true);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری نشریه', error.message);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این نشریه مطمئن هستید؟')) {
      try {
        setLoading(true);
        await publicationService.delete(id);
        appMessage.success('نشریه با موفقیت حذف شد');
        loadData();
      } catch (error: any) {
        appMessage.error('خطا در حذف نشریه', error.message);
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<Publication>[] = [
    { key: 'name', title: 'نام نشریه' },
    { key: 'typeName', title: 'نوع' },
    { key: 'publishPlaceName', title: 'محل چاپ' },
    { key: 'publishDate', title: 'تاریخ انتشار' },
    { key: 'journalNo', title: 'شماره مجله' },
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
        <Title order={2}>مدیریت نشریات</Title>
        <Button
          leftSection={<FaPlus />}
          onClick={() => {
            form.reset();
            setEditingPublication(null);
            setModalOpened(true);
          }}
        >
          افزودن نشریه جدید
        </Button>
      </Group>

      <Card withBorder>
        <ProTable
          data={publications}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['name', 'typeName', 'publishPlaceName']}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingPublication(null);
        }}
        title={editingPublication ? 'ویرایش نشریه' : 'افزودن نشریه جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <TextInput label="نام نشریه" required {...form.getInputProps('name')} />
        <Group grow>
          <Select
            label="نوع نشریه"
            data={publicationTypes.map((t) => ({ value: String(t.id), label: t.title }))}
            {...form.getInputProps('typeId', { type: 'number' })}
          />
          <Select
            label="محل چاپ"
            data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
            {...form.getInputProps('publishPlaceId', { type: 'number' })}
          />
        </Group>
        <Group grow>
          <TextInput label="امتیازدار" {...form.getInputProps('concessionaire')} />
          <TextInput label="مدیر مسئول" {...form.getInputProps('responsibleDirector')} />
        </Group>
        <TextInput label="ویراستار" {...form.getInputProps('editor')} />
        <Group grow>
          <TextInput label="سال" {...form.getInputProps('year')} />
          <TextInput label="شماره مجله" {...form.getInputProps('journalNo')} />
          <PersianDatePickerString
            label="تاریخ انتشار"
            placeholder="تاریخ انتشار را انتخاب کنید"
            value={form.values.publishDate || null}
            onChange={(date) => form.setFieldValue('publishDate', date || '')}
            error={form.errors.publishDate}
          />
        </Group>
        <Group grow>
          <TextInput label="شماره" {...form.getInputProps('no')} />
          <TextInput label="دوره" {...form.getInputProps('period')} />
        </Group>
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

