import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { TableFilters, type TableFilterValues } from '@/app/components/common/TableFilters';
import { FormModal } from '@/app/components/common/FormModal';
import { Select } from '@mantine/core';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { PersianDatePickerString } from '@/app/components/common/PersianDatePickerString';
import { publicationService } from '@/app/services/library/publicationService';
import { fetchAllPaged } from '@/app/services/api/client';
import { cityService } from '@/app/services/library/cityService';
import { languageService } from '@/app/services/library/languageService';
import { subjectService } from '@/app/services/library/subjectService';
import { publicationTypeService } from '@/app/services/library/publicationTypeService';
import type { Publication, CreatePublicationRequest } from '@/app/models/publication';
import type { CityName } from '@/app/models/city';
import type { LanguageName } from '@/app/models/language';
import type { SubjectName } from '@/app/models/subject';
import type { PublicationTypeName } from '@/app/models/publicationType';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';

export default function PublicationsPage() {
  const [publications, setPublications] = useState<Publication[]>([]);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [totalCount, setTotalCount] = useState(0);
  const [filters, setFilters] = useState<TableFilterValues>({ year: '', fromDate: null, toDate: null });
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
      typeId: (value) => (value <= 0 ? 'انتخاب نوع نشریه الزامی است' : null),
      publishPlaceId: (value) => (value <= 0 ? 'انتخاب محل چاپ الزامی است' : null),
      concessionaire: isNotEmpty('امتیازدار الزامی است'),
      responsibleDirector: isNotEmpty('مدیر مسئول الزامی است'),
      editor: isNotEmpty('ویراستار الزامی است'),
      year: isNotEmpty('سال الزامی است'),
      journalNo: isNotEmpty('شماره مجله الزامی است'),
      publishDate: isNotEmpty('تاریخ انتشار الزامی است'),
      no: isNotEmpty('شماره الزامی است'),
      languageId: (value) => (value <= 0 ? 'انتخاب زبان الزامی است' : null),
      subjectId: (value) => (value <= 0 ? 'انتخاب موضوع الزامی است' : null),
    },
  });

  useEffect(() => {
    loadData();
    
  }, [page, filters, searchTerm]);

  useEffect(() => {
    loadLookups();
     
  }, []);

  const handleFilterChange = (next: TableFilterValues) => {
    setFilters(next);
    setPage(1);
  };

  const loadData = async () => {
    try {
      setLoading(true);
      const result = await publicationService.getList({
        page,
        pageSize: 10,
        search: searchTerm || undefined,
        year: filters.year || undefined,
        fromDate: filters.fromDate ?? undefined,
        toDate: filters.toDate ?? undefined,
      });
      setPublications(result.items);
      setTotalCount(result.totalCount);
    } catch {
      
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
    } catch {
        
      }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingPublication) {
          await publicationService.update(editingPublication.id, { ...form.values });
          appMessage.success('نشریه با موفقیت به‌روزرسانی شد');
        } else {
          await publicationService.create(form.values);
          appMessage.success('نشریه با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingPublication(null);
        loadData();
      } catch {
        
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
    } catch {
      
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
      } catch {
        
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
    { key: 'year', title: 'سال' },
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
          exportFileName="publications"
          exportLimit={50000}
          onExportAll={(onProgress) =>
            fetchAllPaged<Publication>(
              (page, pageSize) =>
                publicationService.getList({
                  page,
                  pageSize,
                  search: searchTerm || undefined,
                  year: filters.year || undefined,
                  fromDate: filters.fromDate ?? undefined,
                  toDate: filters.toDate ?? undefined,
                }),
              { pageSize: 200, onProgress },
            )
          }
          loading={loading}
          serverTotal={totalCount}
          page={page}
          onPageChange={setPage}
          onSearchChange={(q) => {
            setSearchTerm(q);
            setPage(1);
          }}
          filters={<TableFilters value={filters} onChange={handleFilterChange} yearSource="publication" />}
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
        <SmartTextInput label="نام نشریه" required withAsterisk {...form.getInputProps('name')} />
        <Group grow>
          <Select
            label="نوع نشریه"
            required
            withAsterisk
            data={publicationTypes.map((t) => ({ value: String(t.id), label: t.title }))}
            value={form.values.typeId ? String(form.values.typeId) : null}
            onChange={(v: string | null) => form.setFieldValue('typeId', v ? Number(v) : 0)}
          />
          <Select
            label="محل چاپ"
            required
            withAsterisk
            data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
            value={form.values.publishPlaceId ? String(form.values.publishPlaceId) : null}
            onChange={(v: string | null) => form.setFieldValue('publishPlaceId', v ? Number(v) : 0)}
          />
        </Group>
        <Group grow>
          <SmartTextInput
            label="امتیازدار"
            required
            withAsterisk
            fieldName="concessionaire"
            {...form.getInputProps('concessionaire')}
          />
          <SmartTextInput
            label="مدیر مسئول"
            required
            withAsterisk
            fieldName="responsibleDirector"
            {...form.getInputProps('responsibleDirector')}
          />
        </Group>
        <SmartTextInput
          label="ویراستار"
          required
          withAsterisk
          fieldName="editor"
          {...form.getInputProps('editor')}
        />
        <Group grow>
          <SmartTextInput
            label="سال"
            required
            withAsterisk
            fieldName="year"
            {...form.getInputProps('year')}
          />
          <SmartTextInput
            label="شماره مجله"
            required
            withAsterisk
            fieldName="journalNo"
            {...form.getInputProps('journalNo')}
          />
          <PersianDatePickerString
            label="تاریخ انتشار"
            required
            placeholder="تاریخ انتشار را انتخاب کنید"
            value={form.values.publishDate || null}
            onChange={(date) => form.setFieldValue('publishDate', date || '')}
            error={form.errors.publishDate?.toString()}
          />
        </Group>
        <Group grow>
          <SmartTextInput
            label="شماره"
            required
            withAsterisk
            fieldName="no"
            {...form.getInputProps('no')}
          />
          <SmartTextInput label="دوره" fieldName="period" {...form.getInputProps('period')} />
        </Group>
        <Group grow>
          <Select
            label="زبان"
            required
            withAsterisk
            data={languages.map((l) => ({ value: String(l.id), label: l.title }))}
            value={form.values.languageId ? String(form.values.languageId) : null}
            onChange={(v: string | null) => form.setFieldValue('languageId', v ? Number(v) : 0)}
          />
          <Select
            label="موضوع"
            required
            withAsterisk
            data={subjects.map((s) => ({ value: String(s.id), label: s.title }))}
            value={form.values.subjectId ? String(form.values.subjectId) : null}
            onChange={(v: string | null) => form.setFieldValue('subjectId', v ? Number(v) : 0)}
          />
        </Group>
      </FormModal>
    </Stack>
  );
}
