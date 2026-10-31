import { useState, useEffect } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { TableFilters, type TableFilterValues } from '@/app/components/common/TableFilters';
import { FormModal } from '@/app/components/common/FormModal';
import { Select } from '@mantine/core';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { SmartNumberInput } from '@/app/components/common/SmartNumberInput';
import { manuscriptService } from '@/app/services/library/manuscriptService';
import { fetchAllPaged } from '@/app/services/api/client';
import { cityService } from '@/app/services/library/cityService';
import { languageService } from '@/app/services/library/languageService';
import { subjectService } from '@/app/services/library/subjectService';
import { gapService } from '@/app/services/library/gapService';
import type { Manuscript, CreateManuscriptRequest } from '@/app/models/manuscript';
import type { CityName } from '@/app/models/city';
import type { LanguageName } from '@/app/models/language';
import type { SubjectName } from '@/app/models/subject';
import type { GapName } from '@/app/models/gap';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';

export default function ManuscriptsPage() {
  const [manuscripts, setManuscripts] = useState<Manuscript[]>([]);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [totalCount, setTotalCount] = useState(0);
  const [filters, setFilters] = useState<TableFilterValues>({ year: '', fromDate: null, toDate: null });
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
      const result = await manuscriptService.getList({
        page,
        pageSize: 10,
        search: searchTerm || undefined,
        year: filters.year || undefined,
        fromDate: filters.fromDate ?? undefined,
        toDate: filters.toDate ?? undefined,
      });
      setManuscripts(result.items);
      setTotalCount(result.totalCount);
    } catch (error) {
      appMessage.error('خطا در بارگذاری نسخ خطی', getErrorMessage(error));
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
    } catch (error) {
      appMessage.error('خطا در بارگذاری داده‌های کمکی', getErrorMessage(error));
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingManuscript) {
          await manuscriptService.update(editingManuscript.id, { ...form.values });
          appMessage.success('نسخه خطی با موفقیت به‌روزرسانی شد');
        } else {
          await manuscriptService.create(form.values);
          appMessage.success('نسخه خطی با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingManuscript(null);
        loadData();
      } catch (error) {
        appMessage.error('خطا در ذخیره نسخه خطی', getErrorMessage(error));
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
    } catch (error) {
      appMessage.error('خطا در بارگذاری نسخه خطی', getErrorMessage(error));
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
      } catch (error) {
        appMessage.error('خطا در حذف نسخه خطی', getErrorMessage(error));
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
          exportFileName="manuscripts"
          exportLimit={50000}
          onExportAll={(onProgress) =>
            fetchAllPaged<Manuscript>(
              (page, pageSize) =>
                manuscriptService.getList({
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
          filters={<TableFilters value={filters} onChange={handleFilterChange} yearSource="manuscript" />}
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
        <SmartTextInput
          label="نام نسخه خطی"
          required
          withAsterisk
          fieldName="name"
          {...form.getInputProps('name')}
        />
        <SmartTextInput label="نویسنده" fieldName="author" {...form.getInputProps('author')} />
        <Group grow>
          <Select
            label="فاصله"
            data={gaps.map((g) => ({ value: String(g.id), label: g.title }))}
            value={form.values.gapId ? String(form.values.gapId) : null}
            onChange={(v: string | null) => form.setFieldValue('gapId', v ? Number(v) : 0)}
          />
          <Select
            label="محل نگارش"
            data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
            value={form.values.writingPlaceId ? String(form.values.writingPlaceId) : null}
            onChange={(v: string | null) => form.setFieldValue('writingPlaceId', v ? Number(v) : 0)}
          />
        </Group>
        <Group grow>
          <SmartTextInput label="سال" fieldName="year" {...form.getInputProps('year')} />
          <SmartNumberInput label="تعداد صفحات" min={0} {...form.getInputProps('pageCount')} />
          <SmartTextInput label="اندازه" fieldName="size" {...form.getInputProps('size')} />
        </Group>
        <SmartTextInput
          label="شماره نسخه"
          fieldName="versionNo"
          {...form.getInputProps('versionNo')}
        />
        <Group grow>
          <Select
            label="زبان"
            data={languages.map((l) => ({ value: String(l.id), label: l.title }))}
            value={form.values.languageId ? String(form.values.languageId) : null}
            onChange={(v: string | null) => form.setFieldValue('languageId', v ? Number(v) : 0)}
          />
          <Select
            label="موضوع"
            data={subjects.map((s) => ({ value: String(s.id), label: s.title }))}
            value={form.values.subjectId ? String(form.values.subjectId) : null}
            onChange={(v: string | null) => form.setFieldValue('subjectId', v ? Number(v) : 0)}
          />
        </Group>
      </FormModal>
    </Stack>
  );
}
