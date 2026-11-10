import { useRef } from 'react';
import { Box } from '@mantine/core';

type Props = {
  url: string;
};

export default function PdfViewer({ url }: Props) {
  const iframeRef = useRef<HTMLIFrameElement>(null);

  return (
    <Box className="pdf-shell">
      <iframe
        ref={iframeRef}
        src={url}
        className="iframe-full"
        title="PDF Viewer"
      />
    </Box>
  );
}

