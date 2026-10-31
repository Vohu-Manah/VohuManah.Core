import { useRef } from 'react';
import { QRCodeSVG } from 'qrcode.react';
import { Stack, Text, Button } from '@mantine/core';
import { FaDownload } from 'react-icons/fa';

type QrCodeWithNumberProps = {
  value: string;
  numberLabel?: string;
  size?: number;
  fileName?: string;
};

export default function QrCodeWithNumber({
  value,
  numberLabel,
  size = 168,
  fileName = 'barcode.svg',
}: QrCodeWithNumberProps) {
  const svgRef = useRef<SVGSVGElement>(null);

  const handleDownload = () => {
    const svg = svgRef.current;
    if (!svg) return;
    const source = new XMLSerializer().serializeToString(svg);
    const blobUrl = URL.createObjectURL(new Blob([source], { type: 'image/svg+xml' }));
    const link = document.createElement('a');
    link.href = blobUrl;
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(blobUrl);
  };

  return (
    <Stack align="center" gap="xs">
      <QRCodeSVG ref={svgRef} value={value} size={size} />
      {numberLabel ? (
        <Text fw={500} size="sm" c="dimmed" ta="center">
          {numberLabel}
        </Text>
      ) : null}
      <Button variant="light" size="xs" leftSection={<FaDownload size={12} />} onClick={handleDownload}>
        دانلود بارکد
      </Button>
    </Stack>
  );
}
