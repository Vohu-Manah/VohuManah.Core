import dayjs from 'dayjs';
import jalaliday from 'jalaliday';

dayjs.extend(jalaliday);

export function formatJalali(date: string | number | Date, format = 'YYYY/MM/DD') {
  return dayjs(date).calendar('jalali').locale('fa').format(format);
}

export function nowJalali(format = 'YYYY/MM/DD HH:mm') {
  return formatJalali(new Date(), format);
}

