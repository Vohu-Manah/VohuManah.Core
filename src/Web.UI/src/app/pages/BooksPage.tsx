import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon, Text } from '@mantine/core';
import {FaPlus, FaEdit, FaTrash, FaUser, FaPaperclip, FaQrcode, FaCamera } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '@/app/components/common/ProTable';
import { TableFilters, type TableFilterValues } from '@/app/components/common/TableFilters';
import { FormModal } from '@/app/components/common/FormModal';
import { Select, FileButton, Avatar, Group as MantineGroup } from '@mantine/core';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import { SmartNumberInput } from '@/app/components/common/SmartNumberInput';
import { FaBook } from 'react-icons/fa';
import { bookService } from '@/app/services/library/bookService';
import { fetchAllPaged } from '@/app/services/api/client';
import { cityService } from '@/app/services/library/cityService';
import { languageService } from '@/app/services/library/languageService';
import { subjectService } from '@/app/services/library/subjectService';
import { publisherService } from '@/app/services/library/publisherService';
import { attachmentService } from '@/app/services/library/attachmentService';
import type { Book, BookList, CreateBookRequest } from '@/app/models/book';
import type { CityName } from '@/app/models/city';
import type { LanguageName } from '@/app/models/language';
import type { SubjectName } from '@/app/models/subject';
import type { PublisherName } from '@/app/models/publisher';
import { appMessage } from '@/app/messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { getFullImageUrl } from '@/app/helpers/imageUrl';
import QrCodePrint from '@/app/components/common/QrCodePrint';
import BarcodePrintModal from '@/app/components/common/BarcodePrintModal';
import BookAttachmentsModal from '@/app/components/common/BookAttachmentsModal';

export default function BooksPage() {
  const navigate = useNavigate();
  const [books, setBooks] = useState<BookList[]>([]);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [barcodeModalOpened, setBarcodeModalOpened] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [totalCount, setTotalCount] = useState(0);
  const [filters, setFilters] = useState<TableFilterValues>({ year: '', fromDate: null, toDate: null });
  const [modalOpened, setModalOpened] = useState(false);
  const [editingBook, setEditingBook] = useState<Book | null>(null);
  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [publishers, setPublishers] = useState<PublisherName[]>([]);
  const [bookImageFile, setBookImageFile] = useState<File | null>(null);
  const [bookImagePreview, setBookImagePreview] = useState<string>('');
  const [bookImages, setBookImages] = useState<Record<number, string>>({});
  const [attachmentsModalOpened, setAttachmentsModalOpened] = useState(false);
  const [selectedBookId, setSelectedBookId] = useState<number | null>(null);

  const form = useForm<CreateBookRequest>({
    initialValues: {
      name: '',
      author: '',
      translator: '',
      editor: '',
      corrector: '',
      publisherId: 0,
      publishPlaceId: 0,
      publishYear: '',
      publishOrder: '',
      isbn: '',
      no: '',
      volumeCount: 1,
      languageId: 0,
      subjectId: 0,
      bookShelfRow: '',
    },
    validate: {
      name: isNotEmpty('نام کتاب الزامی است'),
      author: isNotEmpty('نام نویسنده الزامی است'),
      no: isNotEmpty('شماره کتاب الزامی است'),
      publisherId: (value) => (value <= 0 ? 'انتخاب ناشر الزامی است' : null),
      publishPlaceId: (value) => (value <= 0 ? 'انتخاب محل چاپ الزامی است' : null),
      volumeCount: (value) => (value <= 0 ? 'تعداد جلد باید بیشتر از صفر باشد' : null),
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

  useEffect(() => {
    const loadBookImages = async () => {
      const imageMap: Record<number, string> = {};
      
      const imagePromises = books.map(async (book) => {
        try {
          const attachments = await attachmentService.getByEntity('Book', book.id);
          const isImage = (a: typeof attachments[number]) => {
            const contentType = a.contentType?.toLowerCase() || '';
            const originalFileName = a.originalFileName?.toLowerCase() || '';
            const fileName = a.fileName?.toLowerCase() || '';
            return contentType.includes('image') ||
                   /\.(jpg|jpeg|png|gif|webp)$/i.test(originalFileName) ||
                   /\.(jpg|jpeg|png|gif|webp)$/i.test(fileName);
          };
          const mainImageAttachment =
            attachments.find(a => a.isMain && isImage(a)) ??
            attachments.find(isImage);
          if (mainImageAttachment) {
            const fullUrl = getFullImageUrl(mainImageAttachment.fileUrl);
            if (fullUrl) {
              return { bookId: book.id, url: fullUrl };
            }
          }
        } catch (error) {
          console.error(`Error loading image for book ${book.id}:`, error);
        }
        return null;
      });
      
      const results = await Promise.all(imagePromises);
      results.forEach((result) => {
        if (result) {
          imageMap[result.bookId] = result.url;
        }
      });
      
      setBookImages(imageMap);
    };
    
    if (books.length > 0) {
      loadBookImages();
    }
  }, [books]);

  const loadData = async () => {
    try {
      setLoading(true);
      const result = await bookService.getList({
        page,
        pageSize: 10,
        search: searchTerm || undefined,
        year: filters.year || undefined,
        fromDate: filters.fromDate ?? undefined,
        toDate: filters.toDate ?? undefined,
      });
      setBooks(result.items);
      setTotalCount(result.totalCount);
    } catch {
      
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
    } catch {
        
      }
  };

  const handleBookImageChange = (file: File | null) => {
    if (file) {
      const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
      const fileType = file.type.toLowerCase();
      
      if (!allowedTypes.includes(fileType)) {
        appMessage.error('فرمت فایل مجاز نیست. لطفاً یک فایل تصویری (JPEG, PNG, GIF, WEBP) انتخاب کنید.');
        return;
      }

      setBookImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setBookImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async () => {
    if (!form.validate().hasErrors) {
      try {
        setLoading(true);
        if (editingBook) {
          await bookService.update(editingBook.id, { ...form.values });
          if (bookImageFile) {
            try {
              await attachmentService.upload('Book', editingBook.id, bookImageFile, 'عکس کتاب', true);
              appMessage.success('عکس کتاب با موفقیت آپلود شد');
            } catch {
        
      }
          }
          appMessage.success('کتاب با موفقیت به‌روزرسانی شد');
        } else {
          const newBookId = await bookService.create(form.values);
          if (bookImageFile) {
            try {
              
              await attachmentService.upload('Book', newBookId, bookImageFile, 'عکس کتاب', true);
              appMessage.success('عکس کتاب با موفقیت آپلود شد');
            } catch {
        
      }
          }
          appMessage.success('کتاب با موفقیت ایجاد شد');
        }
        setModalOpened(false);
        form.reset();
        setEditingBook(null);
        setBookImageFile(null);
        setBookImagePreview('');
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
      const book = await bookService.getById(id);
      setEditingBook(book);
      form.setValues({
        name: book.name,
        author: book.author,
        translator: book.translator,
        editor: book.editor,
        corrector: book.corrector,
        publisherId: book.publisherId,
        publishPlaceId: book.publishPlaceId,
        publishYear: book.publishYear,
        publishOrder: book.publishOrder,
        isbn: book.isbn,
        no: book.no,
        volumeCount: book.volumeCount,
        languageId: book.languageId,
        subjectId: book.subjectId,
        bookShelfRow: book.bookShelfRow,
      });
      
      setBookImageFile(null);
      try {
        const attachments = await attachmentService.getByEntity('Book', id);
        const isImage = (a: typeof attachments[number]) => {
          const contentType = a.contentType?.toLowerCase() || '';
          const originalFileName = a.originalFileName?.toLowerCase() || '';
          const fileName = a.fileName?.toLowerCase() || '';
          return contentType.includes('image') ||
                 /\.(jpg|jpeg|png|gif|webp)$/i.test(originalFileName) ||
                 /\.(jpg|jpeg|png|gif|webp)$/i.test(fileName);
        };
        const mainImage =
          attachments.find(a => a.isMain && isImage(a)) ??
          attachments.find(isImage);
        if (mainImage) {
          const fullUrl = getFullImageUrl(mainImage.fileUrl);
          setBookImagePreview(fullUrl || '');
        } else {
          setBookImagePreview('');
        }
      } catch {
        setBookImagePreview('');
      }
      
      setModalOpened(true);
    } catch {
      
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('آیا از حذف این کتاب مطمئن هستید؟')) {
      try {
        setLoading(true);
        await bookService.delete(id);
        appMessage.success('کتاب با موفقیت حذف شد');
        loadData();
      } catch {
        
      } finally {
        setLoading(false);
      }
    }
  };

  const columns: ProTableColumn<BookList>[] = [
    {
      key: 'image',
      title: 'عکس',
      width: 80,
      exportable: false,
      render: (row) => {
        const imageUrl = bookImages[row.id];
        return (
          <Avatar src={imageUrl} size={40} radius="sm">
            <FaBook />
          </Avatar>
        );
      },
    },
    { key: 'name', title: 'نام کتاب' },
    { key: 'author', title: 'نویسنده' },
    { key: 'publisherName', title: 'ناشر' },
    { key: 'publishYear', title: 'سال' },
    { key: 'no', title: 'شماره' },
    { key: 'volumeCount', title: 'تعداد جلد' },
    {
      key: 'qrCode',
      title: 'بارکد',
      exportable: false,
      render: (row) => <QrCodePrint bookId={row.id} />,
    },
    {
      key: 'profile',
      title: 'پروفایل',
      exportable: false,
      render: (row) => (
        <Button
          variant="light"
          color="blue"
          leftSection={<FaUser />}
          onClick={() => navigate(`/books/profile?id=${row.id}`)}
          size="xs"
        >
          مشاهده پروفایل
        </Button>
      ),
    },
    {
      key: 'attachments',
      title: 'پیوست‌ها',
      exportable: false,
      render: (row) => (
        <Button
          variant="light"
          color="orange"
          leftSection={<FaPaperclip />}
          onClick={() => {
            setSelectedBookId(row.id);
            setAttachmentsModalOpened(true);
          }}
          size="xs"
        >
          مدیریت پیوست‌ها
        </Button>
      ),
    },
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
        <Title order={2}>مدیریت کتاب‌ها</Title>
        <Group>
          <Button
          variant="light"
          color="grape"
          leftSection={<FaQrcode />}
          onClick={() => setBarcodeModalOpened(true)}
        >
          چاپ بارکد
        </Button>
          <Button
            variant="light"
            leftSection={<FaCamera />}
            onClick={() => navigate('/scan')}
          >
            اسکن
          </Button>
          <Button
            leftSection={<FaPlus />}
            onClick={() => {
              form.reset();
              setEditingBook(null);
              setBookImageFile(null);
              setBookImagePreview('');
              setModalOpened(true);
            }}
          >
            افزودن کتاب جدید
          </Button>
        </Group>
      </Group>

      <Card withBorder>
        <ProTable
          data={books}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['name', 'author', 'publisherName']}
          exportFileName="books"
          exportLimit={50000}
          onExportAll={(onProgress) =>
            fetchAllPaged<BookList>(
              (page, pageSize) =>
                bookService.getList({
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
          filters={<TableFilters value={filters} onChange={handleFilterChange} yearSource="book" />}
        />
      </Card>

      <FormModal
        opened={modalOpened}
        onClose={() => {
          setModalOpened(false);
          form.reset();
          setEditingBook(null);
          setBookImageFile(null);
          setBookImagePreview('');
        }}
        title={editingBook ? 'ویرایش کتاب' : 'افزودن کتاب جدید'}
        onSubmit={handleSubmit}
        loading={loading}
      >
        <SmartTextInput
          label="نام کتاب"
          required
          withAsterisk
          fieldName="name"
          {...form.getInputProps('name')}
        />
        <SmartTextInput
          label="نویسنده"
          required
          withAsterisk
          fieldName="author"
          {...form.getInputProps('author')}
        />
        <SmartTextInput
          label="مترجم"
          fieldName="translator"
          {...form.getInputProps('translator')}
        />
        <SmartTextInput label="ویراستار" fieldName="editor" {...form.getInputProps('editor')} />
        <SmartTextInput label="صحاف" fieldName="corrector" {...form.getInputProps('corrector')} />
        <Group grow>
          <Select
            label="ناشر"
            required
            withAsterisk
            data={publishers.map((p) => ({ value: String(p.id), label: p.title }))}
            value={form.values.publisherId ? String(form.values.publisherId) : null}
            onChange={(value) => form.setFieldValue('publisherId', value ? Number(value) : 0)}
            error={form.errors.publisherId}
          />
          <Select
            label="محل چاپ"
            required
            withAsterisk
            data={cities.map((c) => ({ value: String(c.id), label: c.title }))}
            value={form.values.publishPlaceId ? String(form.values.publishPlaceId) : null}
            onChange={(value) => form.setFieldValue('publishPlaceId', value ? Number(value) : 0)}
            error={form.errors.publishPlaceId}
          />
        </Group>
        <Group grow>
          <Select
            label="زبان"
            data={languages.map((l) => ({ value: String(l.id), label: l.title }))}
            value={form.values.languageId ? String(form.values.languageId) : null}
            onChange={(value) => form.setFieldValue('languageId', value ? Number(value) : 0)}
          />
          <Select
            label="موضوع"
            data={subjects.map((s) => ({ value: String(s.id), label: s.title }))}
            value={form.values.subjectId ? String(form.values.subjectId) : null}
            onChange={(value) => form.setFieldValue('subjectId', value ? Number(value) : 0)}
          />
        </Group>
        <Group grow>
          <SmartTextInput
            label="سال چاپ"
            fieldName="publishYear"
            {...form.getInputProps('publishYear')}
          />
          <SmartTextInput
            label="شماره چاپ"
            fieldName="publishOrder"
            {...form.getInputProps('publishOrder')}
          />
          <SmartTextInput
            label="شماره کتاب"
            required
            withAsterisk
            fieldName="no"
            {...form.getInputProps('no')}
          />
        </Group>
        <Group grow>
          <SmartTextInput label="ISBN" fieldName="isbn" {...form.getInputProps('isbn')} />
          <SmartNumberInput
            label="تعداد جلد"
            required
            withAsterisk
            min={1}
            {...form.getInputProps('volumeCount')}
          />
          <SmartTextInput
            label="ردیف قفسه"
            fieldName="bookShelfRow"
            {...form.getInputProps('bookShelfRow')}
          />
        </Group>
        <Stack gap="sm">
          <SmartTextInput
            label="عکس کتاب"
            readOnly
            value={bookImageFile?.name || 'هیچ فایلی انتخاب نشده'}
          />
          <MantineGroup>
            <FileButton
              onChange={handleBookImageChange}
              accept="image/jpeg,image/jpg,image/png,image/gif,image/webp"
            >
              {(props) => (
                <Button {...props} variant="light">
                  انتخاب عکس
                </Button>
              )}
            </FileButton>
            {bookImagePreview && (
              <Avatar src={bookImagePreview} size={100} radius="sm">
                <FaBook size={40} />
              </Avatar>
            )}
          </MantineGroup>
          <Text size="xs" c="dimmed">
            فرمت‌های مجاز: JPEG, PNG, GIF, WEBP
          </Text>
        </Stack>
      </FormModal>

      <BookAttachmentsModal
        opened={attachmentsModalOpened}
        onClose={() => {
          setAttachmentsModalOpened(false);
          setSelectedBookId(null);
        }}
        bookId={selectedBookId || 0}
      />

      <BarcodePrintModal
        opened={barcodeModalOpened}
        onClose={() => setBarcodeModalOpened(false)}
      />
    </Stack>
  );
}
