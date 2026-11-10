import { useState, useEffect } from 'react';
import DatePicker from './reactMultiDatePicker';
import persian from 'react-date-object/calendars/persian';
import persian_fa from 'react-date-object/locales/persian_fa';
import type { Value, DateObject } from 'react-multi-date-picker';
import { Stack, Text } from '@mantine/core';
import dayjs from 'dayjs';
import jalaliday from 'jalaliday';
import 'dayjs/locale/fa';

dayjs.extend(jalaliday);

export type PersianDatePickerProps = {
  value?: Date | null;
  onChange?: (value: Date | null) => void;
  placeholder?: string;
  label?: string;
  error?: string;
  required?: boolean;
  disabled?: boolean;
  readOnly?: boolean;
  minDate?: Date;
  maxDate?: Date;
};

export function PersianDatePicker({
  value,
  onChange,
  placeholder = 'انتخاب تاریخ',
  label,
  error,
  required = false,
  disabled = false,
  readOnly = false,
  minDate,
  maxDate,
  ...props
}: PersianDatePickerProps) {
  const [internalValue, setInternalValue] = useState<Value>(null);

  useEffect(() => {
    if (value) {
      const jalaliDate = dayjs(value).calendar('jalali').locale('fa');
      const dateString = jalaliDate.format('YYYY/MM/DD');
      setInternalValue(dateString);
    } else {
      setInternalValue(null);
    }
  }, [value]);

  const handleChange = (date: Value) => {
    setInternalValue(date);

    if (onChange) {
      if (date) {
        let jalaliDate: dayjs.Dayjs;

        if (typeof date === 'string') {
          const [year, month, day] = date.split('/').map(Number);
          jalaliDate = dayjs()
            .calendar('jalali')
            .year(year)
            .month(month - 1)
            .date(day);
        } else if (date && typeof date === 'object' && 'year' in date) {
          jalaliDate = dayjs()
            .calendar('jalali')
            .year((date as DateObject).year)
            .month((date as DateObject).month.number - 1)
            .date((date as DateObject).day);
        } else {
          onChange(null);
          return;
        }

        const gregorianDate = jalaliDate.calendar('gregory').toDate();
        onChange(gregorianDate);
      } else {
        onChange(null);
      }
    }
  };

  const minJalaliDate = minDate
    ? dayjs(minDate).calendar('jalali').locale('fa').format('YYYY/MM/DD')
    : undefined;
  const maxJalaliDate = maxDate
    ? dayjs(maxDate).calendar('jalali').locale('fa').format('YYYY/MM/DD')
    : undefined;

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
        minDate={minJalaliDate}
        maxDate={maxJalaliDate}
        format="YYYY/MM/DD"
        containerClassName="custom-date-picker"
        inputClass="mantine-Input-input mantine-TextInput-input"
        className="w-100-dir-rtl"
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

export default PersianDatePicker;
