import { useState, useEffect, useCallback } from 'react';
import { Button, Card, Stack, Title, Group } from '@mantine/core';
import { FaSearch, FaFilePdf } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { TextInput, Select } from '@mantine/core';
import { manuscriptService } from '@/app/services/library/manuscriptService';
import { fetchAllPaged } from '@/app/services/api/client';
import { cityService } from '@/app/services/library/cityService';
import { languageService } from '@/app/services/library/languageService';
import { subjectService } from '@/app/services/library/subjectService';
import { gapService } from '@/app/services/library/gapService';
import type { ManuscriptSearchRequest, ManuscriptSearchResponse } from '@/app/models/manuscript';
import type { CityName } from '@/app/models/city';
import type { LanguageName } from '@/app/models/language';
import type { SubjectName } from '@/app/models/subject';
import type { GapName } from '@/app/models/gap';
import { handleApiError } from '@/app/helpers/errorHandler';
import { ReportModal } from '@/app/components/reports/ReportModal';
import { YearSelect } from '@/app/components/common/YearSelect';
import { usePagedSearch } from '@/app/hooks/usePagedSearch';

const searchManuscripts = (params: { page?: number; pageSize?: number }) =>
  manuscriptService.search(params as never);

export default function SearchManuscriptPage() {
  const {
    items: results,
    totalCount,
    page,
    loading,
    load: loadPaged,
    goToPage,
  } = usePagedSearch<ManuscriptSearchResponse>(searchManuscripts, 50);

  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [gaps, setGaps] = useState<GapName[]>([]);
  const [reportModalOpened, setReportModalOpened] = useState(false);

  const form = useForm<ManuscriptSearchRequest>({
    initialValues: {
      name: '',
      author: '',
      gapId: undefined,
      writingPlaceId: undefined,
      languageId: undefined,
      subjectId: undefined,
      year: '',
    },
  });


  const loadAllData = useCallback(async () => {
    await loadPaged({}, true);
 }, [loadPaged]);
  const loadLookups = useCallback(async () => {
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
      handleApiError(error, 'خطا در بارگذاری داده‌های کمکی');
    }
 }, []);

  useEffect(() => {

    void loadLookups();
    void loadAllData();
  }, [loadLookups, loadAllData]);



  const buildParams = (): ManuscriptSearchRequest => {
    const params: ManuscriptSearchRequest = {};
    if (form.values.name?.trim()) params.name = form.values.name.trim();
    if (form.values.author?.trim()) params.author = form.values.author.trim();
    if (form.values.gapId) params.gapId = form.values.gapId;
    if (form.values.writingPlaceId) params.writingPlaceId = form.values.writingPlaceId;
    if (form.values.languageId) params.languageId = form.values.languageId;
    if (form.values.subjectId) params.subjectId = form.values.subjectId;
    if (form.values.year?.trim()) params.year = form.values.year.trim();
    return params;
  };

  const handleSearch = async () => {
    try {
      await loadPaged(buildParams(), true);
    } catch (error) {
      handleApiError(error, 'خطا در جستجو');
    }
  };

  const columns: ProTableColumn<ManuscriptSearchResponse>[] = [
    { key: 'name', title: 'نام نسخه خطی' },
    { key: 'author', title: 'نویسنده' },
    { key: 'gapTitle', title: 'فاصله' },
    { key: 'writingPlaceName', title: 'محل نگارش' },
    { key: 'year', title: 'سال' },
    { key: 'languageName', title: 'زبان' },
    { key: 'subjectTitle', title: 'موضوع' },
    { key: 'pageCount', title: 'تعداد صفحات' },
    { key: 'size', title: 'اندازه' },
  ];

  return (
    <Stack p="md">
      <Title order={2}>جستجوی نسخه خطی</Title>

      <Card withBorder>
        <Stack gap="md">
          <Group grow>
            <TextInput label="نام نسخه خطی" {...form.getInputProps('name')} />
            <TextInput label="نویسنده" {...form.getInputProps('author')} />
          </Group>
          <Group grow>
            <Select
              label="فاصله"
              data={gaps.map((g) => ({ value: String(g.id), label: g.title }))}
              clearable
              value={form.values.gapId ? String(form.values.gapId) : null}
              onChange={(value) => form.setFieldValue('gapId', value ? Number(value) : undefined)}
            />
            <Select
              label="محل نگارش"
              data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
              clearable
              value={form.values.writingPlaceId ? String(form.values.writingPlaceId) : null}
              onChange={(value) =>
                form.setFieldValue('writingPlaceId', value ? Number(value) : undefined)
              }
            />
          </Group>
          <Group grow>
            <Select
              label="زبان"
              data={languages.map((l) => ({ value: String(l.id), label: l.title }))}
              clearable
              value={form.values.languageId ? String(form.values.languageId) : null}
              onChange={(value) =>
                form.setFieldValue('languageId', value ? Number(value) : undefined)
              }
            />
            <Select
              label="موضوع"
              data={subjects.map((s) => ({ value: String(s.id), label: s.title }))}
              clearable
              value={form.values.subjectId ? String(form.values.subjectId) : null}
              onChange={(value) =>
                form.setFieldValue('subjectId', value ? Number(value) : undefined)
              }
            />
            <YearSelect
              label="سال"
              source="manuscript"
              value={form.values.year || null}
              onChange={(year) => form.setFieldValue('year', year ?? '')}
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
          exportFileName="نسخ_خطی"
          exportLimit={50000}
          onExportAll={(onProgress) =>
            fetchAllPaged<ManuscriptSearchResponse>(
              (page, pageSize) => manuscriptService.search({ ...buildParams(), page, pageSize }),
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
          reportType: 'manuscript',
          searchCriteria: {
            name: form.values.name || null,
            author: form.values.author || null,
            writingPlaceId: form.values.writingPlaceId || 0,
            fromYear: null,
            toYear: form.values.year || null,
            fromPageCount: 0,
            toPageCount: 0,
            no: null,
            languageId: form.values.languageId || 0,
            subjectId: form.values.subjectId || 0,
            gapId: form.values.gapId || 0,
            size: null,
          },
        }}
        title="گزارش لیست نسخ خطی"
      />
    </Stack>
  );
}