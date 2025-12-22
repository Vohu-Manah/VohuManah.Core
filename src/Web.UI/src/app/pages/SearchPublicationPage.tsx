import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group } from '@mantine/core';
import { FaSearch } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { TextInput, Select } from '@mantine/core';
import { PersianDatePickerString } from '../components/common/PersianDatePickerString';
import { publicationService } from '../services/library/publicationService';
import { cityService } from '../services/library/cityService';
import { languageService } from '../services/library/languageService';
import { subjectService } from '../services/library/subjectService';
import { publicationTypeService } from '../services/library/publicationTypeService';
import type { PublicationSearchRequest, PublicationSearchResponse } from '../models/publication';
import type { CityName } from '../models/city';
import type { LanguageName } from '../models/language';
import type { SubjectName } from '../models/subject';
import type { PublicationTypeName } from '../models/publicationType';
import { appMessage } from '../messages/appMessages';
import { handleApiError } from '../helpers/errorHandler';

export default function SearchPublicationPage() {
  const [results, setResults] = useState<PublicationSearchResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [publicationTypes, setPublicationTypes] = useState<PublicationTypeName[]>([]);

  const form = useForm<PublicationSearchRequest>({
    initialValues: {
      name: '',
      typeId: undefined,
      publishPlaceId: undefined,
      languageId: undefined,
      subjectId: undefined,
      publishDate: '',
    },
  });

  useEffect(() => {
    loadLookups();
    loadAllData();
  }, []);

  const loadAllData = async () => {
    try {
      setLoading(true);
      const data = await publicationService.search({});
      setResults(data);
    } catch (error: any) {
      handleApiError(error, 'خطا در بارگذاری نشریات');
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
      handleApiError(error, 'خطا در بارگذاری داده‌های کمکی');
    }
  };

  const handleSearch = async () => {
    try {
      setLoading(true);
      const params: PublicationSearchRequest = {};
      if (form.values.name?.trim()) params.name = form.values.name.trim();
      if (form.values.typeId) params.typeId = form.values.typeId;
      if (form.values.publishPlaceId) params.publishPlaceId = form.values.publishPlaceId;
      if (form.values.languageId) params.languageId = form.values.languageId;
      if (form.values.subjectId) params.subjectId = form.values.subjectId;
      if (form.values.publishDate?.trim()) params.publishDate = form.values.publishDate.trim();

      if (Object.keys(params).length === 0) {
        await loadAllData();
      } else {
        const data = await publicationService.search(params);
        setResults(data);
      }
    } catch (error: any) {
      handleApiError(error, 'خطا در جستجو');
    } finally {
      setLoading(false);
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
              onChange={(value) => form.setFieldValue('publishPlaceId', value ? Number(value) : undefined)}
            />
          </Group>
          <Group grow>
            <Select
              label="زبان"
              data={languages.map((l) => ({ value: String(l.id), label: l.title }))}
              clearable
              value={form.values.languageId ? String(form.values.languageId) : null}
              onChange={(value) => form.setFieldValue('languageId', value ? Number(value) : undefined)}
            />
            <Select
              label="موضوع"
              data={subjects.map((s) => ({ value: String(s.id), label: s.title }))}
              clearable
              value={form.values.subjectId ? String(form.values.subjectId) : null}
              onChange={(value) => form.setFieldValue('subjectId', value ? Number(value) : undefined)}
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
            <Button variant="subtle" onClick={() => {
              form.reset();
              loadAllData();
            }}>
              نمایش همه
            </Button>
          </Group>
        </Stack>
      </Card>

      <Card withBorder>
        <Title order={4} mb="md">
          نتایج ({results.length})
        </Title>
        <ProTable data={results} columns={columns} exportFileName="نشریات" />
      </Card>
    </Stack>
  );
}
