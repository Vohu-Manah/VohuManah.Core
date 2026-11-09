import { useState, useRef } from 'react';
import { Button, Modal } from '@mantine/core';
import { FaQrcode, FaPrint } from 'react-icons/fa';
import { QRCodeSVG } from 'qrcode.react';
import { encodeQr } from '@/app/helpers/qrPayload';

type Props = {
  bookId: number;
};

export default function QrCodePrint({ bookId }: Props) {
  const printRef = useRef<HTMLDivElement>(null);
  const [opened, setOpened] = useState(false);

  const getQrCodeUrl = () => encodeQr(`books/profile?id=${bookId}`);

  const handlePrint = () => {
    if (!printRef.current) return;

    const printWindow = window.open('', '_blank');
    if (!printWindow) return;

    const content = printRef.current.innerHTML;
    printWindow.document.write(`
      <!DOCTYPE html>
      <html dir="rtl">
        <head>
          <title>بارکد کتاب</title>
          <style>
            @page {
              size: A5;
              margin: 20mm;
            }
            body {
              font-family: Arial, sans-serif;
              display: flex;
              flex-direction: column;
              align-items: center;
              justify-content: center;
              height: 100vh;
              margin: 0;
              direction: rtl;
            }
            .qr-container {
              text-align: center;
            }
            .qr-code {
              margin: 20px 0;
            }
          </style>
        </head>
        <body>
          ${content}
        </body>
      </html>
    `);
    printWindow.document.close();
    printWindow.focus();
    setTimeout(() => {
      printWindow.print();
      printWindow.close();
    }, 250);
  };

  return (
    <>
      <Button
        variant="light"
        color="grape"
        leftSection={<FaQrcode />}
        onClick={() => setOpened(true)}
        size="xs"
      >
        چاپ بارکد
      </Button>

      <Modal
        opened={opened}
        onClose={() => setOpened(false)}
        title="بارکد کتاب"
        size="sm"
      >
        <div ref={printRef} style={{ textAlign: 'center', padding: '20px' }}>
          <div className="qr-container">
            <div className="qr-code">
              <QRCodeSVG value={getQrCodeUrl()} size={200} />
            </div>
            <p style={{ marginTop: '20px', fontSize: '14px' }}>
              شناسه کتاب: {bookId}
            </p>
          </div>
        </div>
        <Button
          fullWidth
          mt="md"
          leftSection={<FaPrint />}
          onClick={handlePrint}
        >
          چاپ
        </Button>
      </Modal>
    </>
  );
}

