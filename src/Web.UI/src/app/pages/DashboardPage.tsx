import { Card, Grid, Group, Text, Title } from '@mantine/core';
import { nowJalali } from '../helpers/date';

export default function DashboardPage() {
  return (
    <Grid>
      <Grid.Col span={{ base: 12, md: 6 }}>
        <Card withBorder>
          <Title order={4}>خوش آمدید</Title>
          <Text c="dimmed" mt="xs">
            زمان فعلی: {nowJalali('YYYY/MM/DD HH:mm')}
          </Text>
        </Card>
      </Grid.Col>
      <Grid.Col span={{ base: 12 }}>
        <Card withBorder>
          <Group justify="space-between">
            <Text size="sm" c="dimmed">
             داشبورد
            </Text>
          </Group>
        </Card>
      </Grid.Col>
    </Grid>
  );
}

