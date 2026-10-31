import { Card, Stack, Title, Text, Group, Badge, LoadingOverlay } from '@mantine/core';
import { BarChart, AreaChart } from '@mantine/charts';
import type { BarChartSeries, AreaChartSeries } from '@mantine/charts';
import CountUp from '@/app/components/effects/CountUp';

export type ChartViewerDatum = {
  label: string;
  value: number;
};

type ChartViewerProps = {
  title: string;
  data: ChartViewerDatum[];
  type?: 'bar' | 'area';
  color?: string;
  loading?: boolean;
  emptyMessage?: string;
  
  xLabel?: string;
  
  yLabel?: string;
  height?: number;
};


export default function ChartViewer({
  title,
  data,
  type = 'bar',
  color = 'blue.6',
  loading = false,
  emptyMessage = 'داده‌ای برای نمایش وجود ندارد',
  xLabel = 'دسته',
  yLabel = 'تعداد',
  height = 280,
}: ChartViewerProps) {
  const total = data.reduce((sum, item) => sum + (Number(item.value) || 0), 0);

  const series: (BarChartSeries | AreaChartSeries)[] = [
    { name: 'value', label: yLabel, color },
  ];

  
  
  
  
  const valueFormatter = (value: number) => new Intl.NumberFormat('en-US').format(value);

  const TOOLTIP_PROPS = {
    labelStyle: { color: '#212529', fontWeight: 600, fontSize: 11 },
    style: {
      border: '1px solid #e1e1e1',
      borderRadius: 8,
      boxShadow: '0 2px 6px rgba(0,0,0,0.08)',
      padding: '4px 7px',
      maxWidth: 220,
    },
  };

  
  const tickStyle = {
    tick: { fill: '#868e96', fontSize: 11 },
    axisLine: { stroke: '#ced4da' },
    tickLine: { stroke: '#ced4da' },
  };
  
  const xAxisProps = {
    ...tickStyle,
    interval: 'preserveStartEnd' as const,
    angle: -30,
    textAnchor: 'end' as const,
    height: 70,
    label: { value: xLabel, position: 'bottom' as const, offset: 14, style: { fill: '#495057', fontSize: 12 } },
  };
  const yAxisProps = {
    ...tickStyle,
    width: 48,
    label: { value: yLabel, position: 'insideLeft' as const, angle: -90, offset: 6, style: { fill: '#495057', fontSize: 12 } },
  };

  return (
    <Card withBorder style={{ position: 'relative' }}>
      <LoadingOverlay visible={loading} />

      <Stack gap="sm">
        <Group justify="space-between">
          <Title order={5}>{title}</Title>
          {data.length > 0 && (
            <Badge variant="light" color="gray">
              مجموع: <CountUp value={total} />
            </Badge>
          )}
        </Group>

        {data.length === 0 ? (
          <Text c="dimmed" ta="center" py="xl" size="sm">
            {emptyMessage}
          </Text>
        ) : type === 'area' ? (
          <AreaChart
            h={height}
            data={data}
            dataKey="label"
            series={series as AreaChartSeries[]}
            curveType="monotone"
            withDots={false}
            withGradient
            tooltipProps={TOOLTIP_PROPS}
            valueFormatter={valueFormatter}
            xAxisProps={xAxisProps}
            yAxisProps={yAxisProps}
          />
        ) : (
          <BarChart
            cursorFill="gray.2"
            h={height}
            data={data}
            dataKey="label"
            series={series as BarChartSeries[]}
            tooltipProps={TOOLTIP_PROPS}
            valueFormatter={valueFormatter}
            xAxisProps={xAxisProps}
            yAxisProps={yAxisProps}
          />
        )}
      </Stack>
    </Card>
  );
}
