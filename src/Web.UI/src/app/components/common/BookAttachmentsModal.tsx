import { useState, useEffect } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import {
  Modal,
  Stack,
  Button,
  Group,
  Text,
  FileButton,
  Progress,
  Card,
  Badge,
  ActionIcon,
  TextInput,
  Switch,
  Loader,
  Center,
} from '@mantine/core';
import { FaUpload, FaTrash, FaEdit, FaDownload, FaPlay, FaCheck, FaTimes } from 'react-icons/fa';
import { attachmentService } from '@/app/services/library/attachmentService';
import type { AttachmentResponse } from '@/app/models/attachment';
import { appMessage } from '@/app/messages/appMessages';
import { getFullImageUrl } from '@/app/helpers/imageUrl';
import PdfViewer from '@/app/components/viewers/PdfViewer';
import WordViewer from '@/app/components/viewers/WordViewer';

type Props = {
  opened: boolean;
  onClose: () => void;
  bookId: number;
};

export default function BookAttachmentsModal({ opened, onClose, bookId }: Props) {
  const [attachments, setAttachments] = useState<AttachmentResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [uploadFile, setUploadFile] = useState<File | null>(null);
  const [uploadDescription, setUploadDescription] = useState('');
  const [editingId, setEditingId] = useState<number | null>(null);
  const [editDescription, setEditDescription] = useState('');
  const [viewerOpened, setViewerOpened] = useState(false);
  const [viewerUrl, setViewerUrl] = useState<string>('');
  const [viewerType, setViewerType] = useState<'pdf' | 'word'>('pdf');


  const loadAttachments = async () => {
    try {
      setLoading(true);
      const data = await attachmentService.getByEntity('Book', bookId);
      setAttachments(data);
    } catch {
      appMessage.error('خطا در بارگذاری پیوست‌ها');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (opened) {

      loadAttachments();
    }

  }, [opened, bookId]);


  const handleUpload = async () => {
    if (!uploadFile) {
      appMessage.error('لطفاً فایلی انتخاب کنید');
      return;
    }

    const fileExtension = uploadFile.name.toLowerCase().split('.').pop();
    const allowedExtensions = ['pdf', 'doc', 'docx', 'xls', 'xlsx'];

    if (!allowedExtensions.includes(fileExtension || '')) {
      appMessage.error('فقط فایل‌های PDF، اکسل و Word مجاز هستند');
      return;
    }

    try {
      setUploading(true);
      setUploadProgress(0);

      const progressInterval = setInterval(() => {
        setUploadProgress((prev) => {
          if (prev >= 90) {
            clearInterval(progressInterval);
            return 90;
          }
          return prev + 10;
        });
      }, 200);

      await attachmentService.upload('Book', bookId, uploadFile, uploadDescription || undefined);
      
      clearInterval(progressInterval);
      setUploadProgress(100);
      
      appMessage.success('پیوست با موفقیت آپلود شد');
      setUploadFile(null);
      setUploadDescription('');
      await loadAttachments();
      
      setTimeout(() => {
        setUploadProgress(0);
        setUploading(false);
      }, 500);
    } catch (error) {
      appMessage.error(getErrorMessage(error, 'خطا در آپلود فایل'));
      setUploadProgress(0);
    } finally {
      setUploading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('آیا از حذف این پیوست مطمئن هستید؟')) {
      return;
    }

    try {
      await attachmentService.delete(id);
      appMessage.success('پیوست با موفقیت حذف شد');
      await loadAttachments();
    } catch {
      appMessage.error('خطا در حذف پیوست');
    }
  };

  const handleStartEdit = (attachment: AttachmentResponse) => {
    setEditingId(attachment.id);
    setEditDescription(attachment.description || '');
  };

  const handleSaveEdit = async (id: number) => {
    try {
      await attachmentService.update(id, editDescription);
      appMessage.success('پیوست با موفقیت به‌روزرسانی شد');
      setEditingId(null);
      setEditDescription('');
      await loadAttachments();
    } catch {
      appMessage.error('خطا در به‌روزرسانی پیوست');
    }
  };

  const handleCancelEdit = () => {
    setEditingId(null);
    setEditDescription('');
  };

  const handleSetMain = async (id: number, isMain: boolean) => {
    try {
      await attachmentService.update(id, undefined, isMain);
      appMessage.success(isMain ? 'پیوست به عنوان اصلی تنظیم شد' : 'پیوست اصلی حذف شد');
      await loadAttachments();
    } catch {
      appMessage.error('خطا در تنظیم پیوست اصلی');
    }
  };

  const handleDownload = (url: string, fileName: string) => {
    const fullUrl = getFullImageUrl(url);
    if (!fullUrl) return;

    const link = document.createElement('a');
    link.href = fullUrl;
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
    if (contentType.includes('pdf')) return 'PDF';
    if (contentType.includes('word') || contentType.includes('document')) return 'WORD';
    if (contentType.includes('excel') || contentType.includes('spreadsheet')) return 'اکسل';
    return contentType;
  };

  const documentAttachments = attachments.filter(
    (a) => a.contentType.includes('pdf') || a.contentType.includes('word') || a.contentType.includes('document') || a.contentType.includes('excel') || a.contentType.includes('spreadsheet'),
  );

  return (
    <>
      <Modal
        opened={opened}
        onClose={onClose}
        title="مدیریت پیوست‌های کتاب"
        size="xl"
      >
        <Stack gap="md">
          <Card withBorder p="md">
            <Stack gap="sm">
              <Text fw={500}>افزودن پیوست جدید</Text>
              <FileButton
                onChange={setUploadFile}
                accept=".pdf,.doc,.docx,.xls,.xlsx,application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
              >
                {(props) => (
                  <Button {...props} leftSection={<FaUpload />} variant="light">
                    {uploadFile ? uploadFile.name : 'انتخاب فایل (PDF یا Word)'}
                  </Button>
                )}
              </FileButton>
              <TextInput
                label="توضیحات"
                placeholder="توضیحات پیوست"
                value={uploadDescription}
                onChange={(e) => setUploadDescription(e.currentTarget.value)}
              />
              {uploading && (
                <Stack gap="xs">
                  <Text size="sm">در حال آپلود...</Text>
                  <Progress value={uploadProgress} animated />
                </Stack>
              )}
              <Button
                onClick={handleUpload}
                disabled={!uploadFile || uploading}
                leftSection={<FaUpload />}
                fullWidth
              >
                آپلود
              </Button>
            </Stack>
          </Card>

          {loading ? (
            <Center p="xl">
              <Loader />
            </Center>
          ) : documentAttachments.length === 0 ? (
            <Center p="xl">
              <Text c="dimmed">هیچ پیوستی یافت نشد</Text>
            </Center>
          ) : (
            <Stack gap="sm">
              {documentAttachments.map((attachment) => {
                const fullUrl = getFullImageUrl(attachment.fileUrl);
                const isEditing = editingId === attachment.id;

                return (
                  <Card key={attachment.id} withBorder p="md">
                    <Stack gap="sm">
                      <Group justify="space-between" align="flex-start">
                        <Stack gap="xs" className="flex-1">
                          {isEditing ? (
                            <TextInput
                              value={editDescription}
                              onChange={(e) => setEditDescription(e.currentTarget.value)}
                              placeholder="توضیحات"
                            />
                          ) : (
                            <Text fw={500}>
                              {attachment.description || attachment.originalFileName}
                            </Text>
                          )}
                          <Group gap="xs">
                            <Badge variant="light" color="gray">
                              {formatFileSize(attachment.fileSize)}
                            </Badge>
                            <Badge variant="light" color="blue">
                              {getFileFormat(attachment.contentType)}
                            </Badge>
                            {attachment.isMain && (
                              <Badge color="grape">پیوست اصلی</Badge>
                            )}
                          </Group>
                        </Stack>

                        <Group gap="xs">
                          {isEditing ? (
                            <>
                              <ActionIcon
                                color="green"
                                onClick={() => handleSaveEdit(attachment.id)}
                              >
                                <FaCheck />
                              </ActionIcon>
                              <ActionIcon color="gray" onClick={handleCancelEdit}>
                                <FaTimes />
                              </ActionIcon>
                            </>
                          ) : (
                            <>
                              <Switch
                                label="اصلی"
                                checked={attachment.isMain}
                                onChange={(e) => handleSetMain(attachment.id, e.currentTarget.checked)}
                                size="sm"
                              />
                              <ActionIcon
                                color="blue"
                                onClick={() => handleStartEdit(attachment)}
                              >
                                <FaEdit />
                              </ActionIcon>
                              {fullUrl && (
                                <>
                                  <ActionIcon
                                    color="teal"
                                    onClick={() => handleDownload(fullUrl, attachment.originalFileName)}
                                  >
                                    <FaDownload />
                                  </ActionIcon>
                                  <ActionIcon
                                    color="cyan"
                                    onClick={() => handleViewOnline(attachment.fileUrl, attachment.contentType)}
                                  >
                                    <FaPlay />
                                  </ActionIcon>
                                </>
                              )}
                              <ActionIcon
                                color="red"
                                onClick={() => handleDelete(attachment.id)}
                              >
                                <FaTrash />
                              </ActionIcon>
                            </>
                          )}
                        </Group>
                      </Group>
                    </Stack>
                  </Card>
                );
              })}
            </Stack>
          )}
        </Stack>
      </Modal>

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
    </>
  );
}