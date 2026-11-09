import { useState, useEffect } from 'react';
import { useMediaQuery } from '@mantine/hooks';
import {
  Modal,
  Button,
  Stack,
  Group,
  SimpleGrid,
  TextInput,
  Select,
  NumberInput,
  Text,
  ScrollArea,
  Divider,
  Title,
} from '@mantine/core';
import { useForm } from '@mantine/form';
import { FaSearch, FaFilePdf, FaRedo } from 'react-icons/fa';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { YearSelect } from '@/app/components/common/YearSelect';
import { bookService } from '@/app/services/library/bookService';
import { fetchAllPaged } from '@/app/services/api/client';
import { cityService } from '@/app/services/library/cityService';
import { languageService } from '@/app/services/library/languageService';
import { subjectService } from '@/app/services/library/subjectService';
import { publisherService } from '@/app/services/library/publisherService';
import type { BookSearchRequest, BookSearchResponse } from '@/app/models/book';
import { appMessage } from '@/app/messages/appMessages';
import { handleApiError, getErrorMessage } from '@/app/helpers/errorHandler';
import { buildBarcodePdf, downloadBarcodePdf } from '@/app/helpers/bookBarcodePdf';

type Props = {
  opened: boolean;
  onClose: () => void;
};

const PAGE_SIZE = 50;

export default function BarcodePrintModal({ opened, onClose }: Props) {
  const isMobile = useMediaQuery('(max-width: 48em)');
  const [results, setResults] = useState<BookSearchResponse[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [exporting, setExporting] = useState(false);
  const [selected, setSelected] = useState<BookSearchResponse[]>([]);

  const [cities, setCities] = useState<{ id: number; title: string }[]>([]);
  const [languages, setLanguages] = useState<{ id: number; title: string }[]>([]);
  const [subjects, setSubjects] = useState<{ id: number; title: string }[]>([]);
  const [publishers, setPublishers] = useState<{ id: number; title: string }[]>([]);

  const form = useForm<BookSearchRequest>({
    initialValues: {
      name: '',
      author: '',
      translator: '',
      editor: '',
      corrector: '',
      isbn: '',
      no: '',
      publisherId: undefined,
      publishPlaceId: undefined,
      languageId: undefined,
      subjectId: undefined,
      fromVolumeCount: undefined,
      toVolumeCount: undefined,
      fromPublishYear: '',
      toPublishYear: '',
    },
  });


  const loadLookups = async () => {
    try {
      const [citiesData, languagesData, subjectsData, publishersData] = await Promise.all([
        cityService.getNames(),
        languageService.getNames(),
        subjectService.getNames(),
        publisherService.getNames(),
      ]);
      setCities(citiesData as never);
      setLanguages(languagesData as never);
      setSubjects(subjectsData as never);
      setPublishers(publishersData as never);
    } catch (error) {
      appMessage.error('خطا در بارگذاری داده‌های کمکی', getErrorMessage(error));
    }
  };
  const search = async (params: BookSearchRequest, reset = false, targetPageOverride?: number) => {
    try {
      setLoading(true);
      const targetPage = reset ? 1 : (targetPageOverride ?? page);
      const result = await bookService.search({ ...params, page: targetPage, pageSize: PAGE_SIZE });
      setResults(result.items);
      setTotalCount(result.totalCount);
      setPage(result.page ?? targetPage);
      if (reset) setSelected([]);
    } catch (error) {
      handleApiError(error, 'خطا در جستجو');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!opened) return;

    void loadLookups();
    void search({}, true);

  }, [opened]);


  const buildParams = (): BookSearchRequest => {
    const v = form.values;
    const params: BookSearchRequest = {};
    if (v.name?.trim()) params.name = v.name.trim();
    if (v.author?.trim()) params.author = v.author.trim();
    if (v.translator?.trim()) params.translator = v.translator.trim();
    if (v.editor?.trim()) params.editor = v.editor.trim();
    if (v.corrector?.trim()) params.corrector = v.corrector.trim();
    if (v.isbn?.trim()) params.isbn = v.isbn.trim();
    if (v.no?.trim()) params.no = v.no.trim();
    if (v.publisherId) params.publisherId = v.publisherId;
    if (v.publishPlaceId) params.publishPlaceId = v.publishPlaceId;
    if (v.languageId) params.languageId = v.languageId;
    if (v.subjectId) params.subjectId = v.subjectId;
    if (v.fromVolumeCount !== undefined) params.fromVolumeCount = v.fromVolumeCount;
    if (v.toVolumeCount !== undefined) params.toVolumeCount = v.toVolumeCount;
    if (v.fromPublishYear?.trim()) params.fromPublishYear = v.fromPublishYear.trim();
    if (v.toPublishYear?.trim()) params.toPublishYear = v.toPublishYear.trim();
    return params;
  };


  const handleSearch = () => void search(buildParams(), true);

  const handleReset = () => {
    form.reset();
    void search({}, true);
  };

  const goToPage = (target: number) => {
    setPage(target);
    void search(buildParams(), false, target);
  };

  const handleExportPdf = async () => {
    const targets = selected.length > 0 ? selected : results;
    if (targets.length === 0) {
      appMessage.warning('رکوردی برای خروجی وجود ندارد');
      return;
    }

    try {
      setExporting(true);
      const doc = await buildBarcodePdf(targets.map((r) => ({ id: r.id, name: r.name, no: r.no })));
      downloadBarcodePdf(doc, `barcodes-${Date.now()}.pdf`);
      appMessage.success(`PDF شامل ${targets.length.toLocaleString('fa-IR')} بارکد ساخته شد`);
    } catch (error) {
      handleApiError(error, 'خطا در ساخت خروجی PDF');
    } finally {
      setExporting(false);
    }
  };

  const columns: ProTableColumn<BookSearchResponse>[] = [
    { key: 'name', title: 'نام کتاب' },
    { key: 'author', title: 'نویسنده' },
    { key: 'no', title: 'شماره' },
    { key: 'publishYear', title: 'سال چاپ' },
  ];

  return (
    <Modal
      opened={opened}
      onClose={onClose}
      title="چاپ بارکد کتاب‌ها"
      size="90%"
      fullScreen={isMobile}
      classNames={{ body: 'barcode-modal-body' }}
    >
      <Stack gap="md">
        <Title order={5}>فیلتر</Title>
        <SimpleGrid cols={{ base: 1, xs: 2, md: 4 }}>
          <TextInput label="نام کتاب" {...form.getInputProps('name')} />
          <TextInput label="نویسنده" {...form.getInputProps('author')} />
          <TextInput label="مترجم" {...form.getInputProps('translator')} />
          <TextInput label="مصحح" {...form.getInputProps('corrector')} />
        </SimpleGrid>

        <SimpleGrid cols={{ base: 1, xs: 2, md: 3 }}>
          <TextInput label="ویراستار" {...form.getInputProps('editor')} />
          <TextInput label="شمارهٔ کتاب" {...form.getInputProps('no')} />
          <TextInput label="شابک (ISBN)" {...form.getInputProps('isbn')} />
        </SimpleGrid>

        <SimpleGrid cols={{ base: 1, xs: 2, md: 4 }}>
          <Select
            label="ناشر"
            data={publishers.map((p) => ({ value: String(p.id), label: p.title }))}
            clearable
            searchable
            {...form.getInputProps('publisherId', { type: 'input' })}
          />
          <Select
            label="محل چاپ"
            data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
            clearable
            searchable
            {...form.getInputProps('publishPlaceId', { type: 'input' })}
          />
          <Select
            label="زبان"
            data={languages.map((l) => ({ value: String(l.id), label: l.title }))}
            clearable
            searchable
            {...form.getInputProps('languageId', { type: 'input' })}
          />
          <Select
            label="موضوع"
            data={subjects.map((s) => ({ value: String(s.id), label: s.title }))}
            clearable
            searchable
            {...form.getInputProps('subjectId', { type: 'input' })}
          />
        </SimpleGrid>

        <SimpleGrid cols={{ base: 1, xs: 2, md: 4 }}>
          <NumberInput label="از تعداد جلد" min={0} {...form.getInputProps('fromVolumeCount')} />
          <NumberInput label="تا تعداد جلد" min={0} {...form.getInputProps('toVolumeCount')} />
          <YearSelect
            label="از سال چاپ"
            source="book"
            value={form.values.fromPublishYear || null}
            onChange={(year) => form.setValues({ ...form.values, fromPublishYear: year ?? '' })}
          />
          <YearSelect
            label="تا سال چاپ"
            source="book"
            value={form.values.toPublishYear || null}
            onChange={(year) => form.setValues({ ...form.values, toPublishYear: year ?? '' })}
          />
        </SimpleGrid>

        <Group justify="flex-end">
          <Button leftSection={<FaSearch />} onClick={handleSearch} loading={loading}>
            جستجو
          </Button>
          <Button variant="subtle" leftSection={<FaRedo />} onClick={handleReset}>
            پاک کردن فیلتر
          </Button>
        </Group>

        <Divider />

        <Group justify="space-between">
          <Group gap="xs">
            <Text fw={600}>نتایج ({totalCount.toLocaleString('fa-IR')})</Text>
            {selected.length > 0 && (
              <Text size="sm" c="dimmed">
                {selected.length.toLocaleString('fa-IR')} ردیف انتخاب شده
              </Text>
            )}
          </Group>
          <Button
            leftSection={<FaFilePdf />}
            color="red"
            variant="light"
            onClick={handleExportPdf}
            loading={exporting}
            disabled={loading || (selected.length === 0 && results.length === 0)}
          >
            خروجی PDF بارکد
          </Button>
        </Group>

        <ScrollArea>
          <ProTable
            data={results}
            columns={columns}
            loading={loading}
            serverTotal={totalCount}
            page={page}
            pageSize={PAGE_SIZE}
            onPageChange={goToPage}
            selectionMode="multi"
            selectedRows={selected}
            onSelectionChange={setSelected}
            getRowId={(row) => row.id}
            exportLimit={50000}
            onExportAll={(onProgress) =>
              fetchAllPaged<BookSearchResponse>(
                (page, pageSize) => bookService.search({ ...buildParams(), page, pageSize }),
                { pageSize: 500, onProgress },
              )
            }
            exportFileName="کتاب‌ها"
          />
        </ScrollArea>
      </Stack>
    </Modal>
  );
}