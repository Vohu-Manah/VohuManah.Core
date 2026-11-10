import { useState, useEffect, useCallback } from 'react';
import { Button, Card, Stack, Title, Group } from '@mantine/core';
import { FaSearch, FaFilePdf } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { TextInput, Select } from '@mantine/core';
import { bookService } from '@/app/services/library/bookService';
import { fetchAllPaged } from '@/app/services/api/client';
import { cityService } from '@/app/services/library/cityService';
import { languageService } from '@/app/services/library/languageService';
import { subjectService } from '@/app/services/library/subjectService';
import { publisherService } from '@/app/services/library/publisherService';
import type { BookSearchRequest, BookSearchResponse } from '@/app/models/book';
import type { CityName } from '@/app/models/city';
import type { LanguageName } from '@/app/models/language';
import type { SubjectName } from '@/app/models/subject';
import type { PublisherName } from '@/app/models/publisher';
import { appMessage } from '@/app/messages/appMessages';
import { usePagedSearch } from '@/app/hooks/usePagedSearch';
import { handleApiError, getErrorMessage} from '@/app/helpers/errorHandler';
import { ReportModal } from '@/app/components/reports/ReportModal';
import { YearSelect } from '@/app/components/common/YearSelect';

export default function SearchBookPage() {
  const {
    items: results,
    totalCount,
    page,
    loading,
    load: loadPaged,
    goToPage,
  } = usePagedSearch<BookSearchResponse>(
    (params) => bookService.search(params as never),
    50,
  );

  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [publishers, setPublishers] = useState<PublisherName[]>([]);
  const [reportModalOpened, setReportModalOpened] = useState(false);

  const form = useForm<BookSearchRequest>({
    initialValues: {
      name: '',
      author: '',
      publisherId: undefined,
      publishPlaceId: undefined,
      languageId: undefined,
      subjectId: undefined,
      publishYear: '',
    },
  });


  const loadAllData = useCallback(async () => {
    await loadPaged({}, true);
 }, [loadPaged]);
  const loadLookups = useCallback(async () => {
    try {
      const [citiesData, languagesData, subjectsData, publishersData] = await Promise.all([
        cityService.getNames(),
        languageService.getNames(),
        subjectService.getNames(),
        publisherService.getNames(),
      ]);
      setCities(citiesData);
      setLanguages(languagesData);
      setSubjects(subjectsData);
      setPublishers(publishersData);
    } catch (error) {
      appMessage.error('خطا در بارگذاری داده‌های کمکی', getErrorMessage(error));
    }
 }, []);

  useEffect(() => {
    
    void loadLookups();
    void loadAllData();
    
  }, []);
  const buildFilterParams = (): BookSearchRequest => {
    const params: BookSearchRequest = {};
    if (form.values.name?.trim()) params.name = form.values.name.trim();
    if (form.values.author?.trim()) params.author = form.values.author.trim();
    if (form.values.publisherId) params.publisherId = form.values.publisherId;
    if (form.values.publishPlaceId) params.publishPlaceId = form.values.publishPlaceId;
    if (form.values.languageId) params.languageId = form.values.languageId;
    if (form.values.subjectId) params.subjectId = form.values.subjectId;
    if (form.values.publishYear?.trim()) params.publishYear = form.values.publishYear.trim();
    return params;
  };

  const handleSearch = async () => {
    try {
      await loadPaged(buildFilterParams(), true);
    } catch (error) {
      handleApiError(error, 'خطا در جستجو');
    }
  };

  const columns: ProTableColumn<BookSearchResponse>[] = [
    { key: 'name', title: 'نام کتاب' },
    { key: 'author', title: 'نویسنده' },
    { key: 'publisherName', title: 'ناشر' },
    { key: 'publishPlaceName', title: 'محل چاپ' },
    { key: 'publishYear', title: 'سال چاپ' },
    { key: 'languageName', title: 'زبان' },
    { key: 'subjectTitle', title: 'موضوع' },
    { key: 'volumeCount', title: 'تعداد جلد' },
    { key: 'bookShelfRow', title: 'ردیف قفسه' },
  ];

  return (
    <Stack p="md">
      <Title order={2}>جستجوی کتاب</Title>

      <Card withBorder>
        <Stack gap="md">
          <Group grow>
            <TextInput label="نام کتاب" {...form.getInputProps('name')} />
            <TextInput label="نویسنده" {...form.getInputProps('author')} />
          </Group>
          <Group grow>
            <Select
              label="ناشر"
              data={publishers.map((p) => ({ value: String(p.id), label: p.title }))}
              clearable
              {...form.getInputProps('publisherId', { type: 'input' })}
            />
            <Select
              label="محل چاپ"
              data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
              clearable
              {...form.getInputProps('publishPlaceId', { type: 'input' })}
            />
          </Group>
          <Group grow>
            <Select
              label="زبان"
              data={languages.map((l) => ({ value: String(l.id), label: l.title }))}
              clearable
              {...form.getInputProps('languageId', { type: 'input' })}
            />
            <Select
              label="موضوع"
              data={subjects.map((s) => ({ value: String(s.id), label: s.title }))}
              clearable
              {...form.getInputProps('subjectId', { type: 'input' })}
            />
            <YearSelect
              label="سال چاپ"
              source="book"
              value={form.values.publishYear || null}
              onChange={(year) => form.setFieldValue('publishYear', year ?? '')}
            />
          </Group>
          <Group justify="flex-end">
            <Button leftSection={<FaSearch />} onClick={handleSearch} loading={loading}>
              اعمال فیلتر
            </Button>
            <Button
              variant="subtle"
              onClick={() => {
                form.reset();
                loadAllData();
              }}
            >
              نمایش همه
            </Button>
          </Group>
        </Stack>
      </Card>

      <Card withBorder>
        <Group justify="space-between" mb="md">
          <Title order={4}>نتایج ({results.length})</Title>
          <Button
            leftSection={<FaFilePdf />}
            onClick={() => setReportModalOpened(true)}
            disabled={results.length === 0 || loading}
            variant="light"
          >
            مشاهده گزارش
          </Button>
        </Group>
        <ProTable
          data={results}
          columns={columns}
          exportFileName="کتاب‌ها"
          exportLimit={50000}
          onExportAll={(onProgress) =>
            fetchAllPaged<BookSearchResponse>(
              (page, pageSize) => bookService.search({ ...buildFilterParams(), page, pageSize }),
              { pageSize: 500, onProgress },
            )
          }
          loading={loading}
          serverTotal={totalCount}
          page={page}
          pageSize={50}
          onPageChange={goToPage}
        />
      </Card>

      <ReportModal
        opened={reportModalOpened}
        onClose={() => setReportModalOpened(false)}
        reportRequest={{
          reportType: 'book',
          searchCriteria: {
            name: form.values.name || null,
            author: form.values.author || null,
            translator: null,
            editor: null,
            isbn: null,
            no: null,
            corrector: null,
            fromVolumeCount: 0,
            toVolumeCount: 0,
            languageId: form.values.languageId || 0,
            subjectId: form.values.subjectId || 0,
            publishPlaceId: form.values.publishPlaceId || 0,
            publisherId: form.values.publisherId || 0,
            fromPublishYear: null,
            toPublishYear: form.values.publishYear || null,
          },
        }}
        title="گزارش لیست کتاب‌ها"
      />
    </Stack>
  );
}