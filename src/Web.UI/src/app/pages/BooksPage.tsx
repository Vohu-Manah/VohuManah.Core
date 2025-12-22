import { useState, useEffect } from 'react';
import { Button, Card, Stack, Title, Group, ActionIcon } from '@mantine/core';
import { FaPlus, FaEdit, FaTrash } from 'react-icons/fa';
import { useForm } from '@mantine/form';
import { ProTable, type ProTableColumn } from '../components/common/ProTable';
import { FormModal } from '../components/common/FormModal';
import { TextInput, Select, NumberInput, FileButton, Avatar, Group as MantineGroup } from '@mantine/core';
import { FaBook } from 'react-icons/fa';
import { bookService } from '../services/library/bookService';
import { cityService } from '../services/library/cityService';
import { languageService } from '../services/library/languageService';
import { subjectService } from '../services/library/subjectService';
import { publisherService } from '../services/library/publisherService';
import { attachmentService } from '../services/library/attachmentService';
import type { Book, BookList, CreateBookRequest } from '../models/book';
import type { CityName } from '../models/city';
import type { LanguageName } from '../models/language';
import type { SubjectName } from '../models/subject';
import type { PublisherName } from '../models/publisher';
import { appMessage } from '../messages/appMessages';
import { isNotEmpty } from '@mantine/form';
import { handleApiError } from '../helpers/errorHandler';

export default function BooksPage() {
  const [books, setBooks] = useState<BookList[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalOpened, setModalOpened] = useState(false);
  const [editingBook, setEditingBook] = useState<Book | null>(null);
  const [cities, setCities] = useState<CityName[]>([]);
  const [languages, setLanguages] = useState<LanguageName[]>([]);
  const [subjects, setSubjects] = useState<SubjectName[]>([]);
  const [publishers, setPublishers] = useState<PublisherName[]>([]);
  const [bookImageFile, setBookImageFile] = useState<File | null>(null);
  const [bookImagePreview, setBookImagePreview] = useState<string>('');
  const [bookImages, setBookImages] = useState<Record<number, string>>({});

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
    loadLookups();
  }, []);

  useEffect(() => {
    // Load images for all books
    const loadBookImages = async () => {
      const imageMap: Record<number, string> = {};
      for (const book of books) {
        try {
          const attachments = await attachmentService.getByEntity('Book', book.id);
          if (attachments.length > 0) {
            const imageUrl = attachments[0].fileUrl;
            // Convert relative URL to absolute using apiBaseUrl
            const fullUrl = imageUrl.startsWith('http') 
              ? imageUrl 
              : `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'}${imageUrl}`;
            imageMap[book.id] = fullUrl;
          }
        } catch {
          // Ignore errors
        }
      }
      setBookImages(imageMap);
    };
    if (books.length > 0) {
      loadBookImages();
    }
  }, [books]);

  const loadData = async () => {
    try {
      setLoading(true);
      const data = await bookService.getList();
      setBooks(data);
    } catch (error: any) {
      handleApiError(error, 'خطا در بارگذاری کتاب‌ها');
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
      handleApiError(error, 'خطا در بارگذاری داده‌های کمکی');
    }
  };

  const handleBookImageChange = (file: File | null) => {
    if (file) {
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
          await bookService.update({ ...form.values, id: editingBook.id });
          // Upload image if selected
          if (bookImageFile) {
            try {
              await attachmentService.upload('Book', editingBook.id, bookImageFile, 'عکس کتاب');
              appMessage.success('عکس کتاب با موفقیت آپلود شد');
            } catch (error: any) {
              handleApiError(error, 'خطا در آپلود عکس');
            }
          }
          appMessage.success('کتاب با موفقیت به‌روزرسانی شد');
        } else {
          const newBookId = await bookService.create(form.values);
          // Upload image if selected
          if (bookImageFile) {
            try {
              await attachmentService.upload('Book', newBookId, bookImageFile, 'عکس کتاب');
              appMessage.success('عکس کتاب با موفقیت آپلود شد');
            } catch (error: any) {
              handleApiError(error, 'خطا در آپلود عکس');
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
      } catch (error: any) {
        handleApiError(error, 'خطا در ذخیره کتاب');
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
      // TODO: Load book image when API is available
      setBookImagePreview('');
      setBookImageFile(null);
      setModalOpened(true);
    } catch (error: any) {
      handleApiError(error, 'خطا در بارگذاری کتاب');
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
      } catch (error: any) {
        handleApiError(error, 'خطا در حذف کتاب');
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
      render: (row) => {
        const imageUrl = bookImages[row.id];
        return (
          <Avatar
            src={imageUrl}
            size={40}
            radius="sm"
          >
            <FaBook />
          </Avatar>
        );
      },
    },
    { key: 'name', title: 'نام کتاب' },
    { key: 'author', title: 'نویسنده' },
    { key: 'publisherName', title: 'ناشر' },
    { key: 'no', title: 'شماره' },
    { key: 'volumeCount', title: 'تعداد جلد' },
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

      <Card withBorder>
        <ProTable
          data={books}
          columns={columns}
          onRefresh={loadData}
          searchableKeys={['name', 'author', 'publisherName']}
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
        <TextInput label="نام کتاب" required withAsterisk {...form.getInputProps('name')} />
        <TextInput label="نویسنده" required withAsterisk {...form.getInputProps('author')} />
        <TextInput label="مترجم" {...form.getInputProps('translator')} />
        <TextInput label="ویراستار" {...form.getInputProps('editor')} />
        <TextInput label="صحاف" {...form.getInputProps('corrector')} />
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
          <TextInput label="سال چاپ" {...form.getInputProps('publishYear')} />
          <TextInput label="شماره چاپ" {...form.getInputProps('publishOrder')} />
          <TextInput label="شماره کتاب" required withAsterisk {...form.getInputProps('no')} />
        </Group>
        <Group grow>
          <TextInput label="ISBN" {...form.getInputProps('isbn')} />
          <NumberInput label="تعداد جلد" required withAsterisk min={1} {...form.getInputProps('volumeCount')} />
          <TextInput label="ردیف قفسه" {...form.getInputProps('bookShelfRow')} />
        </Group>
        <Stack gap="sm">
          <TextInput label="عکس کتاب" readOnly value={bookImageFile?.name || 'هیچ فایلی انتخاب نشده'} />
          <MantineGroup>
            <FileButton onChange={handleBookImageChange} accept="image/*">
              {(props) => <Button {...props} variant="light">انتخاب عکس</Button>}
            </FileButton>
            {bookImagePreview && (
              <Avatar src={bookImagePreview} size={100} radius="sm">
                <FaBook size={40} />
              </Avatar>
            )}
          </MantineGroup>
        </Stack>
      </FormModal>
    </Stack>
  );
}

