import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group } from '@mantine/core';
import { FaSearch } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { TextInput, Select } from '@mantine/core';
import { manuscriptService } from '../services/library/manuscriptService';
import { cityService } from '../services/library/cityService';
import { languageService } from '../services/library/languageService';
import { subjectService } from '../services/library/subjectService';
import { gapService } from '../services/library/gapService';
import type { ManuscriptSearchRequest, ManuscriptSearchResponse } from '../models/manuscript';
import type { CityName } from '../models/city';
import type { LanguageName } from '../models/language';
import type { SubjectName } from '../models/subject';
import type { GapName } from '../models/gap';
import { appMessage } from '../messages/appMessages';
import { handleApiError } from '../helpers/errorHandler';

export default function SearchManuscriptPage() {
  const [results, setResults] = useState<ManuscriptSearchResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [gaps, setGaps] = useState<GapName[]>([]);

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

  useEffect(() => {
    loadLookups();
    loadAllData();
  }, []);

  const loadAllData = async () => {
    try {
      setLoading(true);
      const data = await manuscriptService.search({});
      setResults(data);
    } catch (error: any) {
      handleApiError(error, 'خطا در بارگذاری نسخ خطی');
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
      handleApiError(error, 'خطا در بارگذاری داده‌های کمکی');
    }
  };

  const handleSearch = async () => {
    try {
      setLoading(true);
      const params: ManuscriptSearchRequest = {};
      if (form.values.name?.trim()) params.name = form.values.name.trim();
      if (form.values.author?.trim()) params.author = form.values.author.trim();
      if (form.values.gapId) params.gapId = form.values.gapId;
      if (form.values.writingPlaceId) params.writingPlaceId = form.values.writingPlaceId;
      if (form.values.languageId) params.languageId = form.values.languageId;
      if (form.values.subjectId) params.subjectId = form.values.subjectId;
      if (form.values.year?.trim()) params.year = form.values.year.trim();

      if (Object.keys(params).length === 0) {
        await loadAllData();
      } else {
        const data = await manuscriptService.search(params);
        setResults(data);
      }
    } catch (error: any) {
      handleApiError(error, 'خطا در جستجو');
    } finally {
      setLoading(false);
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
              onChange={(value) => form.setFieldValue('writingPlaceId', value ? Number(value) : undefined)}
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
            <TextInput label="سال" {...form.getInputProps('year')} />
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
        <ProTable data={results} columns={columns} exportFileName="نسخ_خطی" />
      </Card>
    </Stack>
  );
}
