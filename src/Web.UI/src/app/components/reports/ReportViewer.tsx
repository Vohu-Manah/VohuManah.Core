import { useState, useRef, useEffect, useCallback } from 'react';
import { Box, LoadingOverlay, Stack, Text, Alert, ActionIcon } from '@mantine/core';
import { FaExpand, FaCompress } from 'react-icons/fa';
import { reportService, type ReportExportRequest } from '@/app/services/reports/reportService';
import { appMessage } from '@/app/messages/appMessages';
import { getTokens } from '@/app/services/api/tokenStore';

interface ReportViewerProps {
  reportName: string;
  datasourceName: string;
  parameters?: Record<string, string>;
  height?: string;
  showToolbar?: boolean;
}

export function ReportViewer({
  reportName,
  datasourceName,
  parameters = {},
  height = '800px',
}: ReportViewerProps) {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [fullscreen, setFullscreen] = useState(false);
  const iframeRef = useRef<HTMLIFrameElement>(null);
  const boxRef = useRef<HTMLDivElement>(null);

  
  useEffect(() => {
    if (!fullscreen) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === 'Escape') setFullscreen(false);
    };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [fullscreen]);

  const handleExport = useCallback(
    async (format: 'pdf' | 'excel') => {
      const request: ReportExportRequest = { reportName, datasourceName, format, parameters };
      try {
        await reportService.exportReport(request);
      } catch (err) {
        const errorMessage =
          err instanceof Error ? err.message : 'خطا در خروجی‌گرفتن گزارش';
        appMessage.error('خطا در خروجی‌گرفتن گزارش', errorMessage);
      }
    },
    [reportName, datasourceName, parameters],
  );

  useEffect(() => {
    const el = boxRef.current;
    if (el && !fullscreen) el.style.height = height;
  }, [height, fullscreen]);

  useEffect(() => {
    const onMessage = (event: MessageEvent) => {
      switch (event.data?.type) {
        case 'report:export':
          void handleExport(event.data.format === 'excel' ? 'excel' : 'pdf');
          break;
        case 'report:print':
          void handleExport('pdf');
          break;
      }
    };
    window.addEventListener('message', onMessage);
    return () => window.removeEventListener('message', onMessage);
  }, [handleExport]);

  const buildReportUrl = useCallback(() => {
    return reportService.buildReportUrl({
      reportName,
      datasourceName,
      parameters,
    });
  }, [reportName, datasourceName, parameters]);

  const checkReportAvailability = useCallback(async (url: string): Promise<boolean> => {
    try {
      const tokens = getTokens();
      const headers: HeadersInit = {
        Accept: 'text/html',
      };

      if (tokens?.accessToken) {
        headers['Authorization'] = `Bearer ${tokens.accessToken}`;
      }

      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), 10000);

      try {
        const response = await fetch(url, {
          method: 'GET',
          headers,
          signal: controller.signal,
        });

        if (!response.ok) {
          let errorMessage = 'خطا در بارگذاری گزارش';

          if (response.status === 401) {
            errorMessage = 'دسترسی غیرمجاز: لطفاً دوباره وارد شوید';
          } else if (response.status === 403) {
            errorMessage = 'شما دسترسی لازم برای مشاهده این گزارش را ندارید';
          } else if (response.status === 404) {
            errorMessage = 'گزارش یافت نشد';
          } else if (response.status >= 500) {
            errorMessage = 'خطای سرور: لطفاً بعداً تلاش کنید';
          } else {
            errorMessage = `خطا: ${response.status} ${response.statusText}`;
          }

          setError(errorMessage);
          appMessage.error('خطا در بارگذاری گزارش', errorMessage);
          return false;
        }

        return true;
      } finally {
        clearTimeout(timeoutId);
      }
    } catch (err) {
      let errorMessage = 'خطا در اتصال به سرور';

      if (err instanceof Error && err.name === 'AbortError') {
        errorMessage = 'زمان اتصال به سرور به پایان رسید. لطفاً دوباره تلاش کنید';
      } else if (err instanceof TypeError && err.message.includes('fetch')) {
        errorMessage = 'خطا در اتصال به سرور. لطفاً اتصال اینترنت خود را بررسی کنید';
      }

      setError(errorMessage);
      appMessage.error('خطا در بارگذاری گزارش', errorMessage);
      return false;
    }
  }, []);

  useEffect(() => {
    const loadReport = async () => {
      setLoading(true);
      setError(null);

      const url = buildReportUrl();
      const isAvailable = await checkReportAvailability(url);

      if (isAvailable) {
        setLoading(false);
      } else {
        setLoading(false);
      }
    };

    loadReport();
  }, [buildReportUrl, checkReportAvailability]);

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

  return (
    <Stack gap="md">
      <Box
        ref={boxRef}
        pos="relative"
        className={fullscreen ? 'report-fullscreen' : undefined}
      >
        <ActionIcon
          variant="subtle"
          color="gray"
          title={fullscreen ? 'خروج از تمام‌صفحه' : 'نمایش تمام‌صفحه'}
          onClick={() => setFullscreen((v) => !v)}
          className="report-fullscreen-btn"
        >
          {fullscreen ? <FaCompress size={16} /> : <FaExpand size={16} />}
        </ActionIcon>
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
            <Box
              component="iframe"
              ref={iframeRef}
              src={buildReportUrl()}
              onLoad={handleLoad}
              onError={handleError}
              className="iframe-panel"
              title={`گزارش ${reportName}`}
            />
          </>
        )}
      </Box>
    </Stack>
  );
}
