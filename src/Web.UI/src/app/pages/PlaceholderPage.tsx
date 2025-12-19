import { Card, Text, Title } from '@mantine/core';

interface PlaceholderPageProps {
  title?: string;
  description?: string;
}

export default function PlaceholderPage({ title, description }: PlaceholderPageProps) {
  return (
    <Card withBorder shadow="sm" padding="lg" radius="md">
      <Title order={2} mb="md">
        {title || 'صفحه در حال توسعه'}
      </Title>
      <Text c="dimmed">
        {description || 'این صفحه به زودی اضافه خواهد شد.'}
      </Text>
    </Card>
  );
}
