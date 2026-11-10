import { useState, useEffect, useCallback } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import { Box, Loader, Center, Text, Alert } from '@mantine/core';

type Props = {
  url: string;
};

export default function WordViewer({ url }: Props) {
  const [content, setContent] = useState<string>('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadWordDocument = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await fetch(url, {
        method: 'GET',
      });

      if (!response.ok) {
        throw new Error('خطا در بارگذاری فایل');
      }

      const arrayBuffer = await response.arrayBuffer();

      const mammoth = await import('mammoth');
      const result = await mammoth.default.convertToHtml({ arrayBuffer });
      setContent(result.value);
    } catch (err) {
      setError(getErrorMessage(err, 'خطا در نمایش فایل Word. لطفاً فایل را دانلود کنید.'));
      console.error('Error loading Word document:', err);
    } finally {
      setLoading(false);
    }
  }, [url]);

  useEffect(() => {

    void loadWordDocument();
  }, [loadWordDocument]);

  if (loading) {
    return (
      <Center className="h-80vh">
        <Loader size="lg" />
      </Center>
    );
  }

  if (error) {
    return (
      <Center className="h-80vh">
        <Alert color="red" title="خطا">
          {error}
          <br />
          <Text size="sm" mt="md">
            لطفاً فایل را دانلود کنید و به صورت محلی باز کنید.
          </Text>
        </Alert>
      </Center>
    );
  }

  return (
    <Box
      className="doc-viewer"
    >
      <div dangerouslySetInnerHTML={{ __html: content }} />
    </Box>
  );
}
