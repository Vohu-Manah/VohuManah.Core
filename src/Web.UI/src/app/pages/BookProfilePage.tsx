import { useState, useEffect, useCallback } from 'react';
import {
  Stack,
  Group,
  Title,
  Text,
  Button,
  Card,
  Image,
  Avatar,
  Box,
  Divider,
  Badge,
  Modal,
  Loader,
  Center,
} from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { FaArrowRight, FaDownload, FaPlay } from 'react-icons/fa';
import { bookService } from '@/app/services/library/bookService';
import type { BookProfile } from '@/app/models/book';
import { appMessage } from '@/app/messages/appMessages';
import { getFullImageUrl } from '@/app/helpers/imageUrl';
import PdfViewer from '@/app/components/viewers/PdfViewer';
import WordViewer from '@/app/components/viewers/WordViewer';
import QrCodeWithNumber from '@/app/components/common/QrCodeWithNumber';
import { encodeQr } from '@/app/helpers/qrPayload';

type Props = {
  bookId: number;
};

export default function BookProfilePage({ bookId }: Props) {
  const navigate = useNavigate();
  const [book, setBook] = useState<BookProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [viewerOpened, setViewerOpened] = useState(false);
  const [viewerUrl, setViewerUrl] = useState<string>('');
  const [viewerType, setViewerType] = useState<'pdf' | 'word'>('pdf');

  const loadBook = useCallback(async () => {
    try {
      setLoading(true);
      const data = await bookService.getProfile(bookId);
      setBook(data);
    } catch {
      appMessage.error('خطا در بارگذاری اطلاعات کتاب');
    } finally {
      setLoading(false);
    }
  }, [bookId]);

  useEffect(() => {

    void loadBook();
  }, [loadBook]);

  const handleDownload = (url: string, fileName: string) => {
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    link.target = '_blank';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const handleViewOnline = (url: string, contentType: string) => {
    const fullUrl = getFullImageUrl(url);
    if (!fullUrl) return;

    if (contentType.includes('pdf')) {
      setViewerType('pdf');
      setViewerUrl(fullUrl);
      setViewerOpened(true);
    } else if (contentType.includes('word') || contentType.includes('document')) {
      setViewerType('word');
      setViewerUrl(fullUrl);
      setViewerOpened(true);
    } else {
      window.open(fullUrl, '_blank');
    }
  };

  const formatFileSize = (bytes: number): string => {
    const mb = bytes / (1024 * 1024);
    return `${Math.round(mb)} مگابایت`;
  };

  const getFileFormat = (contentType: string): string => {
    if (contentType.includes('pdf')) return 'فرمت PDF';
    if (contentType.includes('word') || contentType.includes('document')) return 'فرمت WORD';
    return contentType;
  };

  if (loading) {
    return (
      <Center mih="100vh">
        <Loader size="lg" />
      </Center>
    );
  }

  if (!book) {
    return (
      <Center mih="100vh">
        <Text c="red">کتاب یافت نشد</Text>
      </Center>
    );
  }

  const isImageAttachment = (a: { contentType?: string; originalFileName?: string; fileName?: string }) => {
    const contentType = a.contentType?.toLowerCase() || '';
    const originalFileName = a.originalFileName?.toLowerCase() || '';
    const fileName = a.fileName?.toLowerCase() || '';
    return contentType.includes('image') ||
           /\.(jpg|jpeg|png|gif|webp)$/i.test(originalFileName) ||
           /\.(jpg|jpeg|png|gif|webp)$/i.test(fileName);
  };

  const coverImage = [book.mainAttachment, ...book.otherAttachments]
    .find((a) => a && isImageAttachment(a));

  const bookCoverUrl = coverImage ? getFullImageUrl(coverImage.fileUrl) : undefined;

  return (
    <Stack p="md" gap="lg">
      <Group justify="space-between">
        <Button
          variant="subtle"
          leftSection={<FaArrowRight />}
          onClick={() => navigate('/books')}
        >
          بازگشت به مدیریت کتاب‌ها
        </Button>
      </Group>

      <Group align="flex-start" gap="xl">
        <Box className="flex-col-300">
          <Card withBorder p="md" radius="md">
            {bookCoverUrl ? (
              <Image src={bookCoverUrl} alt={book.name} radius="md" />
            ) : (
              <Avatar size={300} radius="md">
                {book.name.charAt(0)}
              </Avatar>
            )}
          </Card>
          <Card withBorder p="md" radius="md" mt="md">
            <QrCodeWithNumber
              value={encodeQr(`books/profile?id=${bookId}`)}
              numberLabel={`کد کتاب: ${book.no}`}
              fileName={`book-qr-${bookId}.svg`}
            />
          </Card>
        </Box>

        <Stack className="flex-1" gap="md">
          <Title order={1}>{book.name}</Title>

          <Stack gap="xs">
            <Group gap="md">
              <Text c="blue" fw={500}>
                نویسنده: {book.author}
              </Text>
            </Group>
            {book.translator && (
              <Group gap="md">
                <Text c="blue" fw={500}>
                  مترجم: {book.translator}
                </Text>
              </Group>
            )}
            {book.publisherName && (
              <Group gap="md">
                <Text c="blue" fw={500}>
                  انتشارات: {book.publisherName}
                </Text>
              </Group>
            )}
          </Stack>

          <Divider />

          <Stack gap="xs">
            <Group gap="md">
              <Text>
                <span className="fw-500">کد کتاب:</span> {book.no}
              </Text>
            </Group>
            {book.isbn && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">شابک:</span> {book.isbn}
                </Text>
              </Group>
            )}
            {book.publishYear && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">سال انتشار شمسی:</span> {book.publishYear}
                </Text>
              </Group>
            )}
            {book.publishOrder && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">شماره چاپ:</span> {book.publishOrder}
                </Text>
              </Group>
            )}
            {book.volumeCount > 0 && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">تعداد جلد:</span> {book.volumeCount}
                </Text>
              </Group>
            )}
            {book.languageName && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">زبان:</span> {book.languageName}
                </Text>
              </Group>
            )}
            {book.subjectTitle && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">موضوع:</span> {book.subjectTitle}
                </Text>
              </Group>
            )}
            {book.publishPlaceName && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">محل چاپ:</span> {book.publishPlaceName}
                </Text>
              </Group>
            )}
            {book.bookShelfRow && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">ردیف قفسه:</span> {book.bookShelfRow}
                </Text>
              </Group>
            )}
            {book.editor && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">ویراستار:</span> {book.editor}
                </Text>
              </Group>
            )}
            {book.corrector && (
              <Group gap="md">
                <Text>
                  <span className="fw-500">صحاف:</span> {book.corrector}
                </Text>
              </Group>
            )}
          </Stack>

          {book.mainAttachment && (
            <Button
              size="lg"
              fullWidth
              onClick={() => handleViewOnline(book.mainAttachment!.fileUrl, book.mainAttachment!.contentType)}
            >
              مطالعه پیوست اصلی
            </Button>
          )}
        </Stack>
      </Group>

      {book.otherAttachments.some((attachment) => attachment && !isImageAttachment(attachment)) && (
        <Stack gap="md">
          <Divider
            label={
              <Text
                fw={600}
                size="lg"
                className="attachments-title"
              >
                دیگر پیوست‌های کتاب
              </Text>
            }
            labelPosition="center"
          />

          <Stack gap="sm">
            {book.otherAttachments
              .filter((attachment) => attachment && !isImageAttachment(attachment))
              .map((attachment) => {
              const fullUrl = getFullImageUrl(attachment.fileUrl);
              return (
                <Card key={attachment.id} withBorder p="md" radius="md">
                  <Group justify="space-between" align="center">
                    <Group gap="md">
                      <Stack gap={4}>
                        <Text fw={500}>{attachment.description || attachment.originalFileName}</Text>
                        <Group gap="md">
                          <Badge variant="light" color="gray">
                            {formatFileSize(attachment.fileSize)}
                          </Badge>
                          <Badge variant="light" color="blue">
                            {getFileFormat(attachment.contentType)}
                          </Badge>
                        </Group>
                      </Stack>
                    </Group>

                    <Group gap="xs">
                      {fullUrl && (
                        <>
                          <Button
                            variant="light"
                            color="teal"
                            leftSection={<FaDownload />}
                            onClick={() => handleDownload(fullUrl, attachment.originalFileName)}
                          >
                            دانلود
                          </Button>
                          <Button
                            variant="light"
                            color="cyan"
                            leftSection={<FaPlay />}
                            onClick={() => handleViewOnline(attachment.fileUrl, attachment.contentType)}
                          >
                            مطالعه آنلاین
                          </Button>
                        </>
                      )}
                    </Group>
                  </Group>
                </Card>
              );
            })}
          </Stack>
        </Stack>
      )}

      <Modal
        opened={viewerOpened}
        onClose={() => setViewerOpened(false)}
        size="xl"
        fullScreen
        title="مطالعه فایل"
      >
        {viewerType === 'pdf' ? (
          <PdfViewer url={viewerUrl} />
        ) : (
          <WordViewer url={viewerUrl} />
        )}
      </Modal>
    </Stack>
  );
}
