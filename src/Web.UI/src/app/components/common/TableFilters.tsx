import { Group } from '@mantine/core';
import { YearSelect, type YearSource } from '@/app/components/common/YearSelect';

export type TableFilterValues = {
  year: string;
  fromDate: string | null;
  toDate: string | null;
};

type TableFiltersProps = {
  value: TableFilterValues;
  onChange: (next: TableFilterValues) => void;
  yearSource: YearSource;
};


export function TableFilters({ value, onChange, yearSource }: TableFiltersProps) {
  return (
    <Group gap="xs" align="flex-end">
      <YearSelect
        w={130}
        source={yearSource}
        value={value.year || null}
        onChange={(year) => onChange({ ...value, year: year ?? '' })}
      />
    </Group>
  );
}
