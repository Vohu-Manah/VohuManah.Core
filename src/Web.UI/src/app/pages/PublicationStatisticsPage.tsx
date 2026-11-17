import { useState } from 'react';
import { Stack, Title, Card, Select, Group, Text } from '@mantine/core';
import { ReportViewer } from '@/app/components/reports/ReportViewer';
import { FullscreenToggle } from '@/app/components/reports/FullscreenToggle';
import { REPORT_DATASOURCE_NAMES, REPORT_NAMES } from '@/app/constants/reportConstants';

export default function PublicationStatisticsPage() {
  const [filterType, setFilterType] = useState('1');
  const [fullscreen, setFullscreen] = useState(false);

  const filterOptions = [
    { value: '1', label: 'نوع' },
    { value: '2', label: 'محل انتشار' },
    { value: '3', label: 'تاریخ انتشار' },
    { value: '4', label: 'زبان' },
    { value: '5', label: 'موضوع' },
  ];

  return (
    <Stack p="md">
      <Title order={2}>آمار نشریات</Title>
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
          reportName={REPORT_NAMES.PublicationStatistic}
          datasourceName={REPORT_DATASOURCE_NAMES.DsPublicationStatistic}
          parameters={{ FilterType: filterType }}
          height="calc(100vh - 250px)"
          fullscreen={fullscreen}
          onToggleFullscreen={() => setFullscreen((v) => !v)}
        />
      </Card>
    </Stack>
  );
}
