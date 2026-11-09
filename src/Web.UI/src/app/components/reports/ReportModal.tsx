import { useState, useRef, useEffect, useCallback } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import {
  Modal,
  Box,
  Button,
  Group,
  LoadingOverlay,
  Stack,
  Text,
  Tooltip,
  Alert,
} from '@mantine/core';
import { FaExpand, FaCompress } from 'react-icons/fa';
import { ActionIcon } from '@mantine/core';
import { reportService, type SearchListReportRequest } from '@/app/services/reports/reportService';
import { appMessage } from '@/app/messages/appMessages';

interface ReportModalProps {
  opened: boolean;
  onClose: () => void;
  reportRequest: SearchListReportRequest;
  title?: string;
}

const MAX_EXPORT_ROWS = 50000;

export function ReportModal({ opened, onClose, reportRequest, title = 'گزارش' }: ReportModalProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [reportHtml, setReportHtml] = useState<string | null>(null);
  const [fullscreen, setFullscreen] = useState(false);
  const iframeRef = useRef<HTMLIFrameElement>(null);

  const bufferRef = useRef<Map<number, string>>(new Map());
  const pendingRef = useRef<
    Map<number, Promise<{ html: string; totalPages: number; totalCount: number }>>
  >(new Map());

  const fetchPage = useCallback(
    async (targetPage: number) => {
      const cached = bufferRef.current.get(targetPage);
      if (cached) {
        return { html: cached, totalPages: 0, totalCount: 0 };
      }

      const pending = pendingRef.current.get(targetPage);
      if (pending) {
        return pending;
      }

      const promise = reportService
        .getSearchListReportHtml({
          ...reportRequest,
          searchCriteria: { ...reportRequest.searchCriteria, page: targetPage },
        })
        .then((html) => {
          bufferRef.current.set(targetPage, html);
          pendingRef.current.delete(targetPage);
          return { html, totalPages: 0, totalCount: 0 };
        });

      pendingRef.current.set(targetPage, promise);
      return promise;
    },
    [reportRequest],
  );

  const loadReport = useCallback(
    async (targetPage = 1) => {
      if (!opened) return;

      setLoading(true);
      setError(null);

      try {
        const { html } = await fetchPage(targetPage);
        setReportHtml(html);

        void fetchPage(targetPage + 1);
      } catch (err) {
        const errorMessage = getErrorMessage(err, 'خطا در بارگذاری گزارش');
        setError(errorMessage);
        appMessage.error('خطا در بارگذاری گزارش', errorMessage);
      } finally {
        setLoading(false);
      }
    },
    [opened, fetchPage],
  );

  useEffect(() => {
    if (opened) {
      bufferRef.current.clear();
      pendingRef.current.clear();
      loadReport(1);
    } else {
      setReportHtml(null);
      setError(null);
      bufferRef.current.clear();
      pendingRef.current.clear();
    }
  }, [opened, loadReport]);

  const handleExport = useCallback(
    async (format: 'pdf' | 'excel') => {
      setLoading(true);
      setError(null);
      appMessage.info('در حال پردازش', 'در حال رندر گزارش، لطفاً صبر کنید...');
      try {
        await reportService.exportSearchListReport({
          ...reportRequest,
          format,
          searchCriteria: { ...reportRequest.searchCriteria, page: 1, pageSize: MAX_EXPORT_ROWS },
        });
      } catch (err) {
        const errorMessage = getErrorMessage(err, 'خطا در خروجی‌گرفتن گزارش');
        setError(errorMessage);
        appMessage.error('خطا در خروجی‌گرفتن گزارش', errorMessage);
      } finally {
        setLoading(false);
      }
    },
    [reportRequest],
  );

  useEffect(() => {
    const onMessage = (event: MessageEvent) => {
      switch (event.data?.type) {
        case 'report:page': {
          const target = Number(event.data.page);
          if (Number.isFinite(target) && target > 0) {
            void loadReport(target);
          }
          break;
        }
        case 'report:export':
          void handleExport(event.data.format);
          break;
        case 'report:print': {
          void handleExport('pdf');
          break;
        }
      }
    };

    window.addEventListener('message', onMessage);
    return () => window.removeEventListener('message', onMessage);
  }, [loadReport, handleExport]);

  const handleLoad = () => {
    setLoading(false);
    setError(null);
  };

  const handleError = () => {
    setLoading(false);
    const errorMessage = 'خطا در بارگذاری گزارش';
    setError(errorMessage);
    appMessage.error('خطا در بارگذاری گزارش', errorMessage);
  };

  const [reportDataUrl, setReportDataUrl] = useState<string | null>(null);

  useEffect(() => {
    if (!reportHtml) {
      setReportDataUrl(null);
      return undefined;
    }

    const blobUrl = URL.createObjectURL(
      new Blob([reportHtml], { type: 'text/html;charset=utf-8' }),
    );
    setReportDataUrl(blobUrl);

    return () => URL.revokeObjectURL(blobUrl);
  }, [reportHtml]);

  return (
    <Modal
      opened={opened}
      onClose={onClose}
      title={title}
      size="xl"
      fullScreen={fullscreen}
      className={fullscreen ? 'report-modal-fullscreen' : undefined}
      classNames={{ body: 'report-modal-body', content: 'report-modal-content' }}
    >
      <Stack gap="md" p="md" className="h-100">
        <Group justify="space-between" wrap="wrap">
          <Stack gap={2}>
            <Text size="sm" c="dimmed">
              خروجی PDF/Excel و چاپ از نوار ابزار داخل گزارش
            </Text>
            <Text size="xs" c="dimmed">
              حداکثر خروجی هر گزارش {MAX_EXPORT_ROWS.toLocaleString('fa-IR')} رکورد است؛ برای تعداد
              کمتر فیلترها را دقیق‌تر کنید.
            </Text>
          </Stack>
          <Group gap="xs">
            <Tooltip label={fullscreen ? 'خروج از تمام‌صفحه' : 'نمایش تمام‌صفحه'}>
              <ActionIcon
                variant="light"
                onClick={() => setFullscreen((v) => !v)}
                title={fullscreen ? 'خروج از تمام‌صفحه' : 'نمایش تمام‌صفحه'}
              >
                {fullscreen ? <FaCompress size={16} /> : <FaExpand size={16} />}
              </ActionIcon>
            </Tooltip>
            <Button variant="subtle" onClick={onClose}>
              بستن
            </Button>
          </Group>
        </Group>

        <Box pos="relative" className="flex-1-flex">
          {error ? (
            <Alert
              color="red"
              title="خطا در بارگذاری گزارش"
              icon={<Text>⚠️</Text>}
              className="report-alert"
            >
              <Text size="lg" fw={500}>
                {error}
              </Text>
            </Alert>
          ) : (
            <>
              <LoadingOverlay visible={loading} overlayProps={{ radius: 'sm', blur: 2 }} />
              {reportDataUrl && (
                <Box
                  component="iframe"
                  ref={iframeRef}
                  src={reportDataUrl}
                  onLoad={handleLoad}
                  onError={handleError}
                  className="iframe-panel"
                  title={title}
                />
              )}
            </>
          )}
        </Box>
      </Stack>
    </Modal>
  );
}
