import { useState, useEffect } from 'react';
import DatePicker from 'react-multi-date-picker';
import persian from 'react-date-object/calendars/persian';
import persian_fa from 'react-date-object/locales/persian_fa';
import type { Value, DateObject } from 'react-multi-date-picker';
import { TextInput } from '@mantine/core';
import dayjs from 'dayjs';
import jalaliday from 'jalaliday';
import 'dayjs/locale/fa';

// Extend dayjs with jalaliday
dayjs.extend(jalaliday);

export interface PersianDatePickerProps {
  /** The selected date value */
  value?: Date | null;
  /** Callback when date changes */
  onChange?: (value: Date | null) => void;
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
  /** Minimum selectable date */
  minDate?: Date;
  /** Maximum selectable date */
  maxDate?: Date;
  /** Additional props */
  [key: string]: any;
}

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

  // Convert Date to Jalali date string for react-multi-date-picker
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
        // react-multi-date-picker returns a DateObject or string
        let jalaliDate: dayjs.Dayjs;
        
        if (typeof date === 'string') {
          // Parse string format YYYY/MM/DD
          const [year, month, day] = date.split('/').map(Number);
          jalaliDate = dayjs()
            .calendar('jalali')
            .year(year)
            .month(month - 1)
            .date(day);
        } else if (date && typeof date === 'object' && 'year' in date) {
          // DateObject
          jalaliDate = dayjs()
            .calendar('jalali')
            .year((date as DateObject).year)
            .month((date as DateObject).month.number - 1)
            .date((date as DateObject).day);
        } else {
          onChange(null);
          return;
        }
        
        // Convert Jalali date to Gregorian Date
        const gregorianDate = jalaliDate.calendar('gregory').toDate();
        onChange(gregorianDate);
      } else {
        onChange(null);
      }
    }
  };

  // Convert minDate and maxDate to Jalali strings
  const minJalaliDate = minDate
    ? dayjs(minDate).calendar('jalali').locale('fa').format('YYYY/MM/DD')
    : undefined;
  const maxJalaliDate = maxDate
    ? dayjs(maxDate).calendar('jalali').locale('fa').format('YYYY/MM/DD')
    : undefined;

  return (
    <TextInput.Wrapper label={label} error={error} required={required}>
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
        style={{
          width: '100%',
          direction: 'rtl',
        }}
        {...props}
      />
    </TextInput.Wrapper>
  );
}

export default PersianDatePicker;
