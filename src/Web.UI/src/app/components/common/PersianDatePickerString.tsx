import { useState, useEffect } from 'react';
import DatePicker from 'react-multi-date-picker';
import persian from 'react-date-object/calendars/persian';
import persian_fa from 'react-date-object/locales/persian_fa';
import type { Value, DateObject } from 'react-multi-date-picker';
import { Stack, Text } from '@mantine/core';
import dayjs from 'dayjs';
import jalaliday from 'jalaliday';
import 'dayjs/locale/fa';

dayjs.extend(jalaliday);

export type PersianDatePickerStringProps = {
  value?: string | null;
  onChange?: (value: string | null) => void;
  placeholder?: string;
  label?: string;
  error?: string;
  required?: boolean;

  disabled?: boolean;
  readOnly?: boolean;
};

export function PersianDatePickerString({
  value,
  onChange,
  placeholder = 'انتخاب تاریخ',
  label,
  error,
  required = false,
  disabled = false,
  readOnly = false,
  ...props
}: PersianDatePickerStringProps) {
  const [internalValue, setInternalValue] = useState<Value>(null);

  useEffect(() => {
    if (value) {
      if (typeof value === 'string' && /^\d{4}\/\d{2}\/\d{2}$/.test(value)) {
        setInternalValue(value);
      } else {
        try {
          const date = new Date(value);
          if (!isNaN(date.getTime())) {
            const jalaliDate = dayjs(date).calendar('jalali').locale('fa');
            const dateString = jalaliDate.format('YYYY/MM/DD');
            setInternalValue(dateString);
          } else {
            setInternalValue(null);
          }
        } catch {
          setInternalValue(null);
        }
      }
    } else {
      setInternalValue(null);
    }
  }, [value]);

  const handleChange = (date: Value) => {
    setInternalValue(date);

    if (onChange) {
      if (date) {
        let jalaliDateString: string;

        if (typeof date === 'string') {
          jalaliDateString = date;
        } else if (date && typeof date === 'object' && 'year' in date) {
          const year = String((date as DateObject).year);
          const month = String((date as DateObject).month.number).padStart(2, '0');
          const day = String((date as DateObject).day).padStart(2, '0');
          jalaliDateString = `${year}/${month}/${day}`;
        } else {
          onChange(null);
          return;
        }

        onChange(jalaliDateString);
      } else {
        onChange(null);
      }
    }
  };

  return (
    <Stack gap={4}>
      {label && (
        <Text size="sm" fw={500}>
          {label}
          {required && (
            <Text component="span" c="red">
              {' '}
              *
            </Text>
          )}
        </Text>
      )}
      <DatePicker
        value={internalValue}
        onChange={handleChange}
        calendar={persian}
        locale={persian_fa}
        calendarPosition="bottom-right"
        placeholder={placeholder}
        disabled={disabled || readOnly}
        format="YYYY/MM/DD"
        containerClassName="custom-date-picker"
        inputClass="mantine-Input-input mantine-TextInput-input"
        portal
        style={{
          width: '100%',
          direction: 'rtl',
        }}
        zIndex={9999}
        {...props}
      />
      {error && (
        <Text size="xs" c="red">
          {error}
        </Text>
      )}
    </Stack>
  );
}

export default PersianDatePickerString;
