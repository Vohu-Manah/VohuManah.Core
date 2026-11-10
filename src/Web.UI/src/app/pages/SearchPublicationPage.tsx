import { useState, useEffect, useCallback } from 'react';
import { Button, Card, Stack, Title, Group } from '@mantine/core';
import { FaSearch, FaFilePdf } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { TextInput, Select } from '@mantine/core';
import { PersianDatePickerString } from '@/app/components/common/PersianDatePickerString';
import { publicationService } from '@/app/services/library/publicationService';
import { fetchAllPaged } from '@/app/services/api/client';
import { cityService } from '@/app/services/library/cityService';
import { languageService } from '@/app/services/library/languageService';
import { subjectService } from '@/app/services/library/subjectService';
import { publicationTypeService } from '@/app/services/library/publicationTypeService';
import type { PublicationSearchRequest, PublicationSearchResponse } from '@/app/models/publication';
import type { CityName } from '@/app/models/city';
import type { LanguageName } from '@/app/models/language';
import type { SubjectName } from '@/app/models/subject';
import type { PublicationTypeName } from '@/app/models/publicationType';
import { handleApiError } from '@/app/helpers/errorHandler';
import { ReportModal } from '@/app/components/reports/ReportModal';
import { YearSelect } from '@/app/components/common/YearSelect';
import { usePagedSearch } from '@/app/hooks/usePagedSearch';

const searchPublications = (params: { page?: number; pageSize?: number }) =>
  publicationService.search(params as never);

export default function SearchPublicationPage() {
  const {
    items: results,
    totalCount,
    page,
    loading,
    load: loadPaged,
    goToPage,
  } = usePagedSearch<PublicationSearchResponse>(searchPublications, 50);

  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [publicationTypes, setPublicationTypes] = useState<PublicationTypeName[]>([]);
  const [reportModalOpened, setReportModalOpened] = useState(false);

  const form = useForm<PublicationSearchRequest>({
    initialValues: {
      name: '',
      typeId: undefined,
      publishPlaceId: undefined,
      languageId: undefined,
      subjectId: undefined,
      year: '',
      publishDate: '',
    },
  });


  const loadAllData = useCallback(async () => {
    await loadPaged({}, true);
 }, [loadPaged]);
  const loadLookups = useCallback(async () => {
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
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری داده‌های کمکی');
    }
 }, []);

  useEffect(() => {

    void loadLookups();
    void loadAllData();
  }, [loadLookups, loadAllData]);



  const buildParams = (): PublicationSearchRequest => {
    const params: PublicationSearchRequest = {};
    if (form.values.name?.trim()) params.name = form.values.name.trim();
    if (form.values.typeId) params.typeId = form.values.typeId;
    if (form.values.publishPlaceId) params.publishPlaceId = form.values.publishPlaceId;
    if (form.values.languageId) params.languageId = form.values.languageId;
    if (form.values.subjectId) params.subjectId = form.values.subjectId;
    if (form.values.year?.trim()) params.year = form.values.year.trim();
    if (form.values.publishDate?.trim()) params.publishDate = form.values.publishDate.trim();
    return params;
  };

  const handleSearch = async () => {
    try {
      await loadPaged(buildParams(), true);
    } catch (error) {
      handleApiError(error, 'خطا در جستجو');
    }
  };

  const columns: ProTableColumn<PublicationSearchResponse>[] = [
    { key: 'name', title: 'نام نشریه' },
    { key: 'typeName', title: 'نوع' },
    { key: 'publishPlaceName', title: 'محل چاپ' },
    { key: 'publishDate', title: 'تاریخ انتشار' },
    { key: 'languageName', title: 'زبان' },
    { key: 'subjectTitle', title: 'موضوع' },
    { key: 'journalNo', title: 'شماره مجله' },
    { key: 'no', title: 'شماره' },
  ];

  return (
    <Stack p="md">
      <Title order={2}>جستجوی نشریه</Title>

      <Card withBorder>
        <Stack gap="md">
          <TextInput label="نام نشریه" {...form.getInputProps('name')} />
          <Group grow>
            <Select
              label="نوع نشریه"
              data={publicationTypes.map((t) => ({ value: String(t.id), label: t.title }))}
              clearable
              value={form.values.typeId ? String(form.values.typeId) : null}
              onChange={(value) => form.setFieldValue('typeId', value ? Number(value) : undefined)}
            />
            <Select
              label="محل چاپ"
              data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
              clearable
              value={form.values.publishPlaceId ? String(form.values.publishPlaceId) : null}
              onChange={(value) =>
                form.setFieldValue('publishPlaceId', value ? Number(value) : undefined)
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
              source="publication"
              value={form.values.year || null}
              onChange={(year) => form.setFieldValue('year', year ?? '')}
            />
            <PersianDatePickerString
              label="تاریخ انتشار"
              placeholder="تاریخ انتشار را انتخاب کنید"
              value={form.values.publishDate || null}
              onChange={(date) => form.setFieldValue('publishDate', date || '')}
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
          exportFileName="نشریات"
          exportLimit={50000}
          onExportAll={(onProgress) =>
            fetchAllPaged<PublicationSearchResponse>(
              (page, pageSize) => publicationService.search({ ...buildParams(), page, pageSize }),
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
          reportType: 'publication',
          searchCriteria: {
            name: form.values.name || null,
            publicationTypeId: form.values.typeId || 0,
            concessionaire: null,
            responsibleDirector: null,
            editor: null,
            fromYear: form.values.year || null,
            toYear: form.values.year || null,
            no: null,
            publishDate: form.values.publishDate || null,
            publishPlaceId: form.values.publishPlaceId || 0,
            journalNo: null,
            languageId: form.values.languageId || 0,
            subjectId: form.values.subjectId || 0,
            fromPeriod: null,
            toPeriod: null,
          },
        }}
        title="گزارش لیست نشریات"
      />
    </Stack>
  );
}