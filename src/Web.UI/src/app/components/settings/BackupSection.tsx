import { useState, useEffect, useRef, useCallback } from 'react';
import {
  Card,
  Stack,
  Title,
  Group,
  Button,
  Text,
  Progress,
  Badge,
  Alert,
  Divider,
  FileInput,
  Modal,
  RingProgress,
  ScrollArea,
  Code,
} from '@mantine/core';
import {
  FaDatabase,
  FaDownload,
  FaUpload,
  FaHistory,
  FaExclamationTriangle,
  FaCheck,
  FaTimesCircle,
} from 'react-icons/fa';
import { getTokens } from '@/app/services/api/tokenStore';
import { backupService } from '@/app/services/library/backupService';
import type { BackupInfo } from '@/app/services/library/backupService';
import { appMessage } from '@/app/messages/appMessages';
import { handleApiError } from '@/app/helpers/errorHandler';

const formatSize = (bytes: number): string => {
  if (!bytes) return '—';
  const units = ['بایت', 'کیلوبایت', 'مگابایت', 'گیگابایت'];
  let value = bytes;
  let unit = 0;
  while (value >= 1024 && unit < units.length - 1) {
    value /= 1024;
    unit += 1;
  }
  return `${value.toFixed(unit === 0 ? 0 : 1)} ${units[unit]}`;
};

type BackupPhase = 'idle' | 'running' | 'downloading' | 'done' | 'error';

export default function BackupSection() {
  const [info, setInfo] = useState<BackupInfo | null>(null);
  const [loading, setLoading] = useState(false);
  const [restoring, setRestoring] = useState(false);
  const [uploadFile, setUploadFile] = useState<File | null>(null);
  const pollRef = useRef<number | null>(null);

  const [phase, setPhase] = useState<BackupPhase>('idle');
  const [percent, setPercent] = useState(0);
  const [stage, setStage] = useState('');
  const [phaseError, setPhaseError] = useState<string | null>(null);
  const [logLines, setLogLines] = useState<string[]>([]);
  const [cancelling, setCancelling] = useState(false);

  const loadInfo = useCallback(async () => {
    try {
      const data = await backupService.getInfo();
      setInfo(data);
      return data;
    } catch (error) {
      handleApiError(error, 'خطا در دریافت اطلاعات بکاپ');
      return null;
    }
  }, []);

  useEffect(() => {
    loadInfo();
    return () => {
      if (pollRef.current) {
        window.clearInterval(pollRef.current);
      }
    };
  }, [loadInfo]);

  const nativeDownload = (fileName: string) => {
    const url = `/library/backup/download?fileName=${encodeURIComponent(fileName)}&access_token=${encodeURIComponent(getTokens()?.accessToken ?? '')}`;
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    link.rel = 'noopener';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const handleCreateBackup = async () => {
    try {
      setLoading(true);
      setPhase('running');
      setPercent(0);
      setStage('آماده‌سازی بکاپ');
      setPhaseError(null);
      setLogLines([]);

      await backupService.create();
      appMessage.success('بکاپ‌گیری آغاز شد');

      pollRef.current = window.setInterval(async () => {
        const data = await loadInfo();
        if (!data) return;

        setPercent(data.percent ?? 0);
        setStage(data.stage || 'در حال بکاپ‌گیری از دیتابیس');
        if (data.log?.length) {
          setLogLines(data.log);
        }

        if (data.isRunning) return;

        if (pollRef.current) {
          window.clearInterval(pollRef.current);
          pollRef.current = null;
        }

        if (data.error) {
          setPhase('error');
          setPhaseError(data.error);
          return;
        }

        setPhase('downloading');
        setStage('دانلود خودکار فایل بکاپ');
        setPercent(100);

        try {
          nativeDownload(data.fileName || 'library-onetime.bak');
          setPhase('done');
          setStage('بکاپ با موفقیت دانلود شد');
          appMessage.success('فایل بکاپ دانلود شد و از سرور حذف گردید');
          await loadInfo();
        } catch (dlError) {
          setPhase('error');
          setPhaseError(dlError instanceof Error ? dlError.message : 'دانلود فایل بکاپ ناموفق بود');
        }
      }, 1000);
    } catch (error) {
      setPhase('error');
      setPhaseError('بکاپ‌گیری آغاز نشد');
      handleApiError(error, 'خطا در بکاپ‌گیری');
    } finally {
      setLoading(false);
    }
  };

  const closeModal = () => {
    if (pollRef.current) {
      window.clearInterval(pollRef.current);
      pollRef.current = null;
    }
    setPhase('idle');
    setPercent(0);
    setStage('');
    setPhaseError(null);
    setLogLines([]);
  };

  const handleCancel = async () => {
    try {
      setCancelling(true);
      const cancelled = await backupService.cancel();
      appMessage.info(cancelled ? 'عملیات بکاپ لغو شد' : 'عملیات بکاپ در حال اجرا نبود');
    } catch (error) {
      handleApiError(error, 'خطا در لغو بکاپ');
    } finally {
      setCancelling(false);
    }
  };

  const handleDownload = async () => {
    try {
      setLoading(true);

      nativeDownload(info?.fileName || 'library-onetime.bak');
      appMessage.success('دانلود فایل بکاپ آغاز شد');
      await loadInfo();
    } catch (error) {
      handleApiError(error, 'خطا در دانلود بکاپ');
    } finally {
      setLoading(false);
    }
  };

  const handleRestoreDaily = async () => {
    if (
      !confirm('دیتابیس به آخرین بکاپ روزانه بازگردانی می‌شود و سیستم ریست می‌شود. ادامه می‌دهید؟')
    ) {
      return;
    }

    try {
      setRestoring(true);
      await backupService.restoreDaily();
      appMessage.success('بازگردانی انجام شد؛ لطفاً دوباره وارد شوید');
    } catch (error) {
      handleApiError(error, 'خطا در بازگردانی بکاپ روزانه');
    } finally {
      setRestoring(false);
    }
  };

  const handleRestoreUpload = async () => {
    if (!uploadFile) {
      appMessage.error('ابتدا فایل بکاپ را انتخاب کنید');
      return;
    }

    if (!confirm('دیتابیس با فایل آپلودشده بازگردانی می‌شود و سیستم ریست می‌شود. ادامه می‌دهید؟')) {
      return;
    }

    try {
      setRestoring(true);
      await backupService.restoreUpload(uploadFile);
      appMessage.success('بازگردانی انجام شد؛ لطفاً دوباره وارد شوید');
    } catch (error) {
      handleApiError(error, 'خطا در بازگردانی فایل بکاپ');
    } finally {
      setRestoring(false);
    }
  };

  return (
    <Card withBorder>
      <Stack gap="md">
        <Group justify="space-between">
          <Title order={4}>بکاپ و بازگردانی دیتابیس</Title>
          {info?.exists ? (
            <Badge color="green" variant="light">
              آخرین بکاپ: {info.takenAt} - {info.takenTime}
            </Badge>
          ) : (
            <Badge color="gray" variant="light">
              بکاپی موجود نیست
            </Badge>
          )}
        </Group>

        <Text size="sm" c="dimmed">
          بکاپ روزانه به‌صورت خودکار هر ۲۴ ساعت یک‌بار گرفته می‌شود و فقط یک نسخه نگه داشته می‌شود.
          بکاپ لحظه‌ای (دکمهٔ پایین) جداست و اگر در یک ساعت دانلود نشود، خودکار از سرور پاک می‌شود.
        </Text>

        {info?.error && !phaseError && (
          <Alert color="red" icon={<FaExclamationTriangle />} title="خطا در بکاپ‌گیری">
            {info.error}
          </Alert>
        )}

        <Group>
          <Button
            leftSection={<FaDatabase size={14} />}
            onClick={handleCreateBackup}
            loading={loading}
          >
            بکاپ‌گیری لحظه‌ای
          </Button>

          <Button
            variant="light"
            leftSection={<FaDownload size={14} />}
            onClick={handleDownload}
            loading={loading}
            disabled={!info?.exists}
          >
            دانلود فایل بکاپ
          </Button>

          {info?.exists && (
            <Text size="sm" c="dimmed">
              حجم فایل: {formatSize(info.sizeBytes)}
            </Text>
          )}
        </Group>

        <Divider label="بازگردانی" labelPosition="center" />

        <Group align="flex-end">
          <FileInput
            label="بازگردانی از فایل بکاپ"
            placeholder="انتخاب فایل .bak"
            leftSection={<FaUpload size={14} />}
            value={uploadFile}
            onChange={setUploadFile}
            accept=".bak"
            style={{ flex: 1 }}
          />
          <Button
            color="red"
            variant="light"
            loading={restoring}
            onClick={handleRestoreUpload}
            leftSection={<FaUpload size={14} />}
          >
            بازگردانی فایل
          </Button>
          <Button
            color="orange"
            variant="light"
            loading={restoring}
            disabled={!info?.exists}
            onClick={handleRestoreDaily}
            leftSection={<FaHistory size={14} />}
          >
            بازگردانی بکاپ روزانه
          </Button>
        </Group>
      </Stack>
      <Modal
        opened={phase !== 'idle'}
        onClose={closeModal}
        title="بکاپ‌گیری از دیتابیس"
        centered
        closeOnClickOutside={phase === 'done' || phase === 'error'}
        withCloseButton={phase === 'done' || phase === 'error'}
      >
        <Stack gap="md" align="center" py="md">
          {phase === 'done' ? (
            <>
              <FaCheck size={48} color="var(--mantine-color-green-6)" />
              <Text fw={600}>{stage}</Text>
              <Button onClick={closeModal}>بستن</Button>
            </>
          ) : phase === 'error' ? (
            <>
              <FaExclamationTriangle size={48} color="var(--mantine-color-red-6)" />
              <Text fw={600} c="red">
                {phaseError}
              </Text>
              <Button onClick={closeModal}>بستن</Button>
            </>
          ) : (
            <>
              <RingProgress
                size={140}
                thickness={12}
                roundCaps
                sections={[{ value: percent, color: 'blue.6' }]}
                label={
                  <Text ta="center" fw={700} size="lg">
                    {percent}٪
                  </Text>
                }
              />
              <Text size="sm" c="dimmed" ta="center">
                {stage}
              </Text>
              <Progress value={percent} animated striped w="100%" />
              <Text size="xs" c="dimmed" ta="center">
                {phase === 'downloading'
                  ? 'در حال دانلود خودکار فایل بکاپ…'
                  : 'پس از پایان بکاپ، فایل به‌صورت خودکار دانلود می‌شود.'}
              </Text>

              {logLines.length > 0 && (
                <ScrollArea.Autosize mah={140} w="100%" type="auto">
                  <Code block style={{ fontSize: 11, whiteSpace: 'pre-wrap', textAlign: 'left' }}>
                    {logLines.join('\n')}
                  </Code>
                </ScrollArea.Autosize>
              )}

              {phase === 'running' && (
                <Button
                  color="red"
                  variant="light"
                  leftSection={<FaTimesCircle size={14} />}
                  onClick={handleCancel}
                  loading={cancelling}
                  w="100%"
                >
                  لغو عملیات
                </Button>
              )}
            </>
          )}
        </Stack>
      </Modal>
    </Card>
  );
}
