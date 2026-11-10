import { jsPDF } from 'jspdf';
import QRCode from 'qrcode';
import { encodeQr } from '@/app/helpers/qrPayload';

export type BarcodePdfItem = {
  id: number;
  name: string;
  author?: string;
  no?: string;
  publishYear?: string;
};


export async function buildBarcodePdf(
  items: BarcodePdfItem[],
  options?: { columns?: number; rows?: number },
): Promise<jsPDF> {
  const columns = options?.columns ?? 3;
  const rows = options?.rows ?? 8;

  const doc = new jsPDF({ unit: 'mm', format: 'a4' });
  const pageWidth = doc.internal.pageSize.getWidth();
  const pageHeight = doc.internal.pageSize.getHeight();
  const margin = 10;

  const cellWidth = (pageWidth - margin * 2) / columns;
  const cellHeight = (pageHeight - margin * 2) / rows;
  const qrSize = Math.min(cellWidth - 12, cellHeight - 26);

  for (let i = 0; i < items.length; i++) {
    const item = items[i];
    const col = i % columns;
    const row = Math.floor(i / columns) % rows;

    if (i > 0 && col === 0 && row === 0) {
      doc.addPage();
    }

    const x = margin + col * cellWidth;
    const y = margin + row * cellHeight;

    
    try {
      const dataUrl = await QRCode.toDataURL(encodeQr(`books/profile?id=${item.id}`), {
        width: 256,
        margin: 1,
        errorCorrectionLevel: 'L',
      });
      doc.addImage(dataUrl, 'PNG', x + (cellWidth - qrSize) / 2, y + 4, qrSize, qrSize);
    } catch (error) {
      console.warn('Barcode QR generation failed', error);
    }

    
    doc.setFontSize(9);
    doc.setTextColor(20, 20, 20);
    doc.text(String(item.id), x + cellWidth / 2, y + qrSize + 9, { align: 'center' });

    
    
    
    
    if (item.no) {
      doc.setFontSize(7);
      doc.setTextColor(120, 120, 120);
      doc.text(item.no, x + cellWidth / 2, y + qrSize + 12, { align: 'center' });
    }

    
    doc.setDrawColor(220, 220, 220);
    doc.setLineWidth(0.2);
    doc.roundedRect(x + 2, y + 2, cellWidth - 4, cellHeight - 4, 2, 2, 'S');
  }

  return doc;
}


export function downloadBarcodePdf(doc: jsPDF, fileName: string) {
  doc.save(fileName);
}
