import { Stack, Title, Card, Group, Button, SimpleGrid } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { FaSearch, FaChartBar, FaBook, FaScroll, FaNewspaper } from 'react-icons/fa';

export default function ReportsPage() {
  const navigate = useNavigate();

  const reportCategories = [
    {
      title: 'گزارش‌های جستجو',
      icon: <FaSearch size={24} />,
      reports: [
        { path: '/books/search', label: 'جستجوی کتاب', icon: <FaBook size={18} /> },
        { path: '/manuscripts/search', label: 'جستجوی نسخه خطی', icon: <FaScroll size={18} /> },
        { path: '/publications/search', label: 'جستجوی نشریه', icon: <FaNewspaper size={18} /> },
      ],
    },
    {
      title: 'گزارش‌های آماری',
      icon: <FaChartBar size={24} />,
      reports: [
        { path: '/books/statistics', label: 'گزارشات آماری کتاب', icon: <FaBook size={18} /> },
        { path: '/manuscripts/statistics', label: 'گزارشات آماری نسخ خطی', icon: <FaScroll size={18} /> },
        { path: '/publications/statistics', label: 'گزارشات آماری نشریه', icon: <FaNewspaper size={18} /> },
      ],
    },
  ];

  return (
    <Stack p="md">
      <Title order={2}>گزارش‌ها</Title>

      <SimpleGrid cols={{ base: 1, md: 2 }} spacing="lg">
        {reportCategories.map((category, categoryIndex) => (
          <Card key={categoryIndex} withBorder shadow="sm" padding="lg" radius="md">
            <Group mb="md">
              {category.icon}
              <Title order={4}>{category.title}</Title>
            </Group>
            <Stack gap="xs">
              {category.reports.map((report, reportIndex) => (
                <Button
                  key={reportIndex}
                  variant="light"
                  fullWidth
                  leftSection={report.icon}
                  justify="flex-start"
                  onClick={() => navigate(report.path)}
                >
                  {report.label}
                </Button>
              ))}
            </Stack>
          </Card>
        ))}
      </SimpleGrid>
    </Stack>
  );
}


