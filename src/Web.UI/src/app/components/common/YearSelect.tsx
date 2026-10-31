import { useEffect, useState } from 'react';
import { Select } from '@mantine/core';
import { bookService } from '@/app/services/library/bookService';
import { manuscriptService } from '@/app/services/library/manuscriptService';
import { publicationService } from '@/app/services/library/publicationService';

export type YearSource = 'book' | 'manuscript' | 'publication';

type YearSelectProps = {
  label?: string;
  placeholder?: string;
  value: string | null;
  onChange: (year: string | null) => void;
  source: YearSource;
  w?: number | string;
};

async function fetchYears(source: YearSource): Promise<string[]> {
  if (source === 'book') {
    const s = await bookService.getStatistics();
    return s.countByYear.map((i) => i.text).sort();
  }
  if (source === 'manuscript') {
    const s = await manuscriptService.getStatistics();
    return s.countByYear.map((i) => i.text).sort();
  }
  const s = await publicationService.getStatistics();
  
  return [...new Set(s.countByPublishDate.map((i) => i.text.slice(0, 4)))].sort();
}

function fallbackYears(): string[] {
  
  const thisYear = new Date().getFullYear() - 621;
  return Array.from({ length: 30 }, (_, k) => String(thisYear - k));
}

export function YearSelect({
  label = 'سال',
  placeholder = 'انتخاب سال',
  value,
  onChange,
  source,
  w,
}: YearSelectProps) {
  const [years, setYears] = useState<string[]>([]);

  useEffect(() => {
    let alive = true;
    fetchYears(source)
      .then((list) => {
        if (alive) setYears(list.length > 0 ? list : fallbackYears());
      })
      .catch(() => {
        if (alive) setYears(fallbackYears());
      });
    return () => {
      alive = false;
    };
  }, [source]);

  return (
    <Select
      label={label}
      placeholder={placeholder}
      data={years.map((y) => ({ value: y, label: y }))}
      value={value}
      onChange={onChange}
      searchable
      clearable
      w={w}
    />
  );
}
