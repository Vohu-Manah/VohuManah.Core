import { useState } from 'react';
import { Stack, Title, Card, Select } from '@mantine/core';
import { ReportViewer } from '@/app/components/reports/ReportViewer';
import { REPORT_DATASOURCE_NAMES, REPORT_NAMES } from '@/app/constants/reportConstants';

export default function ManuscriptStatisticsPage() {
  const [filterType, setFilterType] = useState('1');

  const filterOptions = [
    { value: '1', label: 'فاصله' },
    { value: '2', label: 'محل نگارش' },
    { value: '3', label: 'سال' },
    { value: '4', label: 'زبان' },
    { value: '5', label: 'موضوع' },
  ];

  return (
    <Stack p="md">
      <Title order={2}>آمار نسخ خطی</Title>
      <Card withBorder>
        <Select
          label="فیلتر بر اساس"
          placeholder="انتخاب فیلتر"
          data={filterOptions}
          value={filterType}
          onChange={(value) => setFilterType(value || '1')}
          mb="md"
        />
        <ReportViewer
          reportName={REPORT_NAMES.ManuscriptStatistic}
          datasourceName={REPORT_DATASOURCE_NAMES.DsManuscriptStatistic}
          parameters={{ FilterType: filterType }}
          height="calc(100vh - 250px)"
        />
      </Card>
    </Stack>
  );
}
