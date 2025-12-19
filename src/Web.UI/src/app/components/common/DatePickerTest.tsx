import { useState } from 'react';
import { Box, Card, Group, Text, Stack, Button, Divider } from '@mantine/core';
import { PersianDatePicker } from './PersianDatePicker';
import { formatJalali, nowJalali } from '../../helpers/date';

export function DatePickerTest() {
  const [selectedDate, setSelectedDate] = useState<Date | null>(null);
  const [minDate] = useState<Date>(new Date(2020, 0, 1)); // January 1, 2020
  const [maxDate] = useState<Date>(new Date(2030, 11, 31)); // December 31, 2030

  const handleDateChange = (date: Date | null) => {
    setSelectedDate(date);
  };

  const clearDate = () => {
    setSelectedDate(null);
  };

  const setToday = () => {
    setSelectedDate(new Date());
  };

  return (
    <Card shadow="sm" padding="lg" radius="md" withBorder>
      <Card.Section withBorder inheritPadding py="xs">
        <Text size="lg" fw={500}>
          تست DatePicker شمسی
        </Text>
      </Card.Section>

      <Stack gap="md" mt="md">
        {/* Basic DatePicker */}
        <Box>
          <Text size="sm" fw={500} mb="xs">
            DatePicker پایه:
          </Text>
          <PersianDatePicker
            label="تاریخ تولد"
            placeholder="تاریخ تولد خود را انتخاب کنید"
            value={selectedDate}
            onChange={handleDateChange}
          />
        </Box>

        <Divider />

        {/* DatePicker with constraints */}
        <Box>
          <Text size="sm" fw={500} mb="xs">
            DatePicker با محدودیت تاریخ:
          </Text>
          <PersianDatePicker
            label="تاریخ رویداد"
            placeholder="تاریخی بین ۱۴۰۰ تا ۱۴۱۰ انتخاب کنید"
            value={selectedDate}
            onChange={handleDateChange}
            minDate={minDate}
            maxDate={maxDate}
            error={selectedDate && (selectedDate < minDate || selectedDate > maxDate)
              ? 'تاریخ انتخاب شده خارج از محدوده مجاز است'
              : undefined}
          />
        </Box>

        <Divider />

        {/* DatePicker with error */}
        <Box>
          <Text size="sm" fw={500} mb="xs">
            DatePicker با خطا:
          </Text>
          <PersianDatePicker
            label="تاریخ انقضا"
            placeholder="تاریخ انقضا را انتخاب کنید"
            value={selectedDate}
            onChange={handleDateChange}
            error="این فیلد اجباری است"
            required
          />
        </Box>

        <Divider />

        {/* DatePicker disabled */}
        <Box>
          <Text size="sm" fw={500} mb="xs">
            DatePicker غیرفعال:
          </Text>
          <PersianDatePicker
            label="تاریخ ثبت"
            placeholder="این فیلد غیرفعال است"
            value={new Date()}
            disabled
          />
        </Box>

        <Divider />

        {/* Action buttons */}
        <Group>
          <Button variant="light" onClick={setToday}>
            تنظیم تاریخ امروز
          </Button>
          <Button variant="outline" onClick={clearDate}>
            پاک کردن تاریخ
          </Button>
        </Group>

        {/* Date information display */}
        {selectedDate && (
          <Box>
            <Text size="sm" fw={500} mb="xs">
              اطلاعات تاریخ انتخاب شده:
            </Text>
            <Stack gap="xs">
              <Text size="sm">
                <strong>میلادی:</strong> {selectedDate.toLocaleDateString('fa-IR')}
              </Text>
              <Text size="sm">
                <strong>شمسی:</strong> {formatJalali(selectedDate, 'YYYY/MM/DD')}
              </Text>
              <Text size="sm">
                <strong>روز هفته:</strong> {formatJalali(selectedDate, 'dddd')}
              </Text>
              <Text size="sm">
                <strong>ماه:</strong> {formatJalali(selectedDate, 'MMMM')}
              </Text>
            </Stack>
          </Box>
        )}

        {/* Current date info */}
        <Box>
          <Text size="sm" fw={500} mb="xs">
            تاریخ فعلی سیستم:
          </Text>
          <Text size="sm">
            {nowJalali('YYYY/MM/DD HH:mm:ss dddd')}
          </Text>
        </Box>
      </Stack>
    </Card>
  );
}

export default DatePickerTest;
