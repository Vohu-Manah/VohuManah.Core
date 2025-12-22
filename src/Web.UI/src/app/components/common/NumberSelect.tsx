import { Select, type SelectProps } from '@mantine/core';

type NumberSelectProps = Omit<SelectProps, 'value' | 'onChange'> & {
  value?: number | null;
  onChange?: (value: number | null) => void;
};

export function NumberSelect({ value, onChange, ...props }: NumberSelectProps) {
  return (
    <Select
      {...props}
      value={value ? String(value) : null}
      onChange={(val) => onChange?.(val ? Number(val) : null)}
    />
  );
}


