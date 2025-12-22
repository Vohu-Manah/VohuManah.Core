import { useState } from 'react';
import { Button, Card, Stack, Title, Group } from '@mantine/core';
import { FaSearch } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { TextInput, Select } from '@mantine/core';
import { bookService } from '../services/library/bookService';
import { cityService } from '../services/library/cityService';
import { languageService } from '../services/library/languageService';
import { subjectService } from '../services/library/subjectService';
import { publisherService } from '../services/library/publisherService';
import type { BookSearchRequest, BookSearchResponse } from '../models/book';
import type { CityName } from '../models/city';
import type { LanguageName } from '../models/language';
import type { SubjectName } from '../models/subject';
import type { PublisherName } from '../models/publisher';
import { appMessage } from '../messages/appMessages';
import { useEffect } from 'react';
import { handleApiError } from '../helpers/errorHandler';

export default function SearchBookPage() {
  const [results, setResults] = useState<BookSearchResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [publishers, setPublishers] = useState<PublisherName[]>([]);

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

  useEffect(() => {
    loadLookups();
    loadAllData(); // Load all data by default
  }, []);

  const loadAllData = async () => {
    try {
      setLoading(true);
      const data = await bookService.search({});
      setResults(data);
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری کتاب‌ها', error.message);
    } finally {
      setLoading(false);
    }
  };

  const loadLookups = async () => {
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
    } catch (error: any) {
      appMessage.error('خطا در بارگذاری داده‌های کمکی', error.message);
    }
  };

  const handleSearch = async () => {
    try {
      setLoading(true);
      const params: BookSearchRequest = {};
      if (form.values.name?.trim()) params.name = form.values.name.trim();
      if (form.values.author?.trim()) params.author = form.values.author.trim();
      if (form.values.publisherId) params.publisherId = form.values.publisherId;
      if (form.values.publishPlaceId) params.publishPlaceId = form.values.publishPlaceId;
      if (form.values.languageId) params.languageId = form.values.languageId;
      if (form.values.subjectId) params.subjectId = form.values.subjectId;
      if (form.values.publishYear?.trim()) params.publishYear = form.values.publishYear.trim();

      // If no filters, load all data
      if (Object.keys(params).length === 0) {
        await loadAllData();
      } else {
        const data = await bookService.search(params);
        setResults(data);
      }
    } catch (error: any) {
      handleApiError(error, 'خطا در جستجو');
    } finally {
      setLoading(false);
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
              {...form.getInputProps('publisherId', { type: 'number' })}
            />
            <Select
              label="محل چاپ"
              data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
              clearable
              {...form.getInputProps('publishPlaceId', { type: 'number' })}
            />
          </Group>
          <Group grow>
            <Select
              label="زبان"
              data={languages.map((l) => ({ value: String(l.id), label: l.title }))}
              clearable
              {...form.getInputProps('languageId', { type: 'number' })}
            />
            <Select
              label="موضوع"
              data={subjects.map((s) => ({ value: String(s.id), label: s.title }))}
              clearable
              {...form.getInputProps('subjectId', { type: 'number' })}
            />
            <TextInput label="سال چاپ" {...form.getInputProps('publishYear')} />
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
        <ProTable data={results} columns={columns} exportFileName="کتاب‌ها" />
      </Card>
    </Stack>
  );
}

