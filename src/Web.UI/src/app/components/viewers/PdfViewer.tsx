import { useRef } from 'react';
import { Box } from '@mantine/core';

type Props = {
  url: string;
};

export default function PdfViewer({ url }: Props) {
  const iframeRef = useRef<HTMLIFrameElement>(null);

  return (
    <Box style={{ width: '100%', height: '80vh' }}>
      <iframe
        ref={iframeRef}
        src={url}
        style={{
          width: '100%',
          height: '100%',
          border: 'none',
        }}
        title="PDF Viewer"
      />
    </Box>
  );
}

