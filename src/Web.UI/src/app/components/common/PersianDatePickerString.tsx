import { useState, useEffect } from 'react';
import DatePicker from 'react-multi-date-picker';
import persian from 'react-date-object/calendars/persian';
import persian_fa from 'react-date-object/locales/persian_fa';
import type { Value, DateObject } from 'react-multi-date-picker';
import { Stack, Text } from '@mantine/core';
import dayjs from 'dayjs';
import jalaliday from 'jalaliday';
import 'dayjs/locale/fa';

// Extend dayjs with jalaliday
dayjs.extend(jalaliday);

export interface PersianDatePickerStringProps {
  /** The selected date value as string (YYYY/MM/DD format) */
  value?: string | null;
  /** Callback when date changes */
  onChange?: (value: string | null) => void;
  /** Placeholder text */
  placeholder?: string;
  /** Label text */
  label?: string;
  /** Error message */
  error?: string;
  /** Whether the input is required */
  required?: boolean;
  /** Whether the input is disabled */
  disabled?: boolean;
  /** Whether the input is read-only */
  readOnly?: boolean;
  /** Additional props */
  [key: string]: any;
}

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

  // Convert string (YYYY/MM/DD) to Jalali date string for react-multi-date-picker
  useEffect(() => {
    if (value) {
      // If value is already in YYYY/MM/DD format, use it directly
      if (typeof value === 'string' && /^\d{4}\/\d{2}\/\d{2}$/.test(value)) {
        setInternalValue(value);
      } else {
        // Try to parse as date string and convert to Jalali
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
          // Already in YYYY/MM/DD format
          jalaliDateString = date;
        } else if (date && typeof date === 'object' && 'year' in date) {
          // DateObject - format as YYYY/MM/DD
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
          {required && <Text component="span" c="red"> *</Text>}
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
        style={{
          width: '100%',
          direction: 'rtl',
          zIndex: 9999,
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

