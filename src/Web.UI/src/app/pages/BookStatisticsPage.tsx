import { useState } from 'react';
import { Stack, Title, Card, Select, Group, Text } from '@mantine/core';
import { ReportViewer } from '@/app/components/reports/ReportViewer';
import { FullscreenToggle } from '@/app/components/reports/FullscreenToggle';
import { REPORT_DATASOURCE_NAMES, REPORT_NAMES } from '@/app/constants/reportConstants';

export default function BookStatisticsPage() {
  const [filterType, setFilterType] = useState<string>('1');
  const [fullscreen, setFullscreen] = useState(false);

  const filterOptions = [
    { value: '1', label: 'ناشر' },
    { value: '2', label: 'محل انتشار' },
    { value: '3', label: 'سال انتشار' },
    { value: '4', label: 'زبان' },
    { value: '5', label: 'موضوع' },
  ];

  return (
    <Stack p="md">
      <Title order={2}>آمار کتاب‌ها</Title>
      <Card withBorder>
        <Group justify="space-between" wrap="nowrap">
          <Text size="sm" fw={500}>فیلتر بر اساس</Text>
          <FullscreenToggle fullscreen={fullscreen} onToggle={() => setFullscreen((v) => !v)} />
        </Group>
        <Select
          placeholder="انتخاب فیلتر"
          data={filterOptions}
          value={filterType}
          onChange={(value) => setFilterType(value || '1')}
          className={fullscreen ? 'report-filter-hidden' : undefined}
          mb="md"
        />
        <ReportViewer
          reportName={REPORT_NAMES.BookStatistic}
          datasourceName={REPORT_DATASOURCE_NAMES.DsBookStatistic}
          parameters={{ FilterType: filterType }}
          height="calc(100vh - 250px)"
          fullscreen={fullscreen}
          onToggleFullscreen={() => setFullscreen((v) => !v)}
        />
      </Card>
    </Stack>
  );
}
