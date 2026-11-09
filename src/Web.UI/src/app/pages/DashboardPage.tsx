import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, Grid, Group, Text, Title, Button, Badge, Stack } from '@mantine/core';
import { FaGithub, FaHeart, FaStar, FaQrcode } from 'react-icons/fa';
import { nowJalali, formatJalali } from '@/app/helpers/date';
import ChartViewer from '@/app/components/reports/ChartViewer';
import type { ChartViewerDatum } from '@/app/components/reports/ChartViewer';
import AnalogClock from '@/app/components/dashboard/AnalogClock';
import CountUp from '@/app/components/effects/CountUp';
import ShinyText from '@/app/components/effects/ShinyText';
import StarBorder from '@/app/components/common/StarBorder';
import { visitService } from '@/app/services/library/visitService';
import { bookService } from '@/app/services/library/bookService';

const GITHUB_URL = 'https://github.com/Vohu-Manah/VohuManah.Core';

export default function DashboardPage() {
  const navigate = useNavigate();
  const [visits, setVisits] = useState<ChartViewerDatum[]>([]);
  const [bookStats, setBookStats] = useState<ChartViewerDatum[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;

    const load = async () => {
      try {
        const [visitData, stats] = await Promise.allSettled([
          visitService.getStats(30),
          bookService.getStatistics(),
        ]);

        if (!mounted) return;

        if (visitData.status === 'fulfilled') {
          setVisits(
            visitData.value.items.map((item) => ({
              label: formatJalali(item.date, 'MM/DD'),
              value: item.count,
            })),
          );
        }

        if (stats.status === 'fulfilled') {
          setBookStats(
            stats.value.countBySubject.map((item) => ({
              label: item.text,
              value: item.value,
            })),
          );
        }
      } finally {
        if (mounted) {
          setLoading(false);
        }
      }
    };

    load();

    return () => {
      mounted = false;
    };
  }, []);

  const totalVisits = visits.reduce((sum, item) => sum + item.value, 0);
  const totalBooks = bookStats.reduce((sum, item) => sum + item.value, 0);

  return (
    <Stack p="md" gap="md">
      <Group justify="space-between" align="center">
        <ShinyText text="داشبورد" />
        <Badge variant="light" color="gray" size="lg">
          {nowJalali('YYYY/MM/DD')}
        </Badge>
      </Group>

      {}
      <Grid gutter="md" align="stretch">
        <Grid.Col span={{ base: 12, md: 6 }}>
          <StarBorder
            as="div"
            className="star-border--card"
            theme="light"
            color="#4c6ef5"
            trailColor="#9775fa"
            thickness={1}
            radius={12}
            stars={2}
            duration={6}
            glow={0.35}
            sparkle={false}
            hover="lap"
            clickPulse={false}
          >
            <Card withBorder h="100%" className="card-enter">
              <Stack gap="xs">
                <Title order={4}>خوش آمدید</Title>
                <Text c="dimmed" size="sm">
                  نمای کلی کتابخانه — بازدیدها، آمار کتاب‌ها و دسترسی سریع به گزارش‌ها.
                </Text>

                <Group gap="xs" mt="xs">
                  <Button onClick={() => navigate('/books/search')} variant="light" size="xs">
                    جستجوی کتاب
                  </Button>
                  <Button onClick={() => navigate('/books/statistics')} variant="light" size="xs">
                    آمار کتاب‌ها
                  </Button>
                  <Button onClick={() => navigate('/reports')} variant="light" size="xs">
                    گزارش‌ها
                  </Button>
                </Group>

                <Group gap="xl" mt="md">
                  <div>
                    <Text size="xl" fw={700}>
                      <CountUp value={totalVisits} />
                    </Text>
                    <Text size="xs" c="dimmed">
                      بازدید ۳۰ روز اخیر
                    </Text>
                  </div>
                  <div>
                    <Text size="xl" fw={700}>
                      <CountUp value={totalBooks} />
                    </Text>
                    <Text size="xs" c="dimmed">
                      کتاب (بر اساس موضوع)
                    </Text>
                  </div>
                </Group>
              </Stack>
            </Card>
          </StarBorder>
        </Grid.Col>

        <Grid.Col span={{ base: 12, md: 3 }}>
          <Card
            withBorder
            h="100%"
            className="card-enter"
            style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}
          >
            <AnalogClock size={140} label={nowJalali('HH:mm')} />
          </Card>
        </Grid.Col>

        <Grid.Col span={{ base: 12, md: 3 }}>
          <StarBorder
            as="div"
            className="star-border--card"
            theme="light"
            color="#4c6ef5"
            trailColor="#9775fa"
            thickness={1}
            radius={12}
            stars={2}
            duration={6}
            glow={0.35}
            sparkle={false}
            hover="lap"
            clickPulse={false}
          >
            <Card withBorder h="100%" className="card-enter">
              <Stack gap="xs">
                <Group gap="xs">
                  <FaGithub size={20} />
                  <Title order={5}>حمایت از پروژه</Title>
                </Group>
                <Text size="sm" c="dimmed">
                  اگر این پروژه برایتان مفید بود، با یک ستاره یا گزارش مشکل از آن حمایت کنید.
                </Text>
                <Group gap="xs" mt="xs">
                  <Button
                    component="a"
                    href={GITHUB_URL}
                    target="_blank"
                    rel="noopener noreferrer"
                    leftSection={<FaStar size={14} />}
                    size="xs"
                    variant="light"
                    style={{ display: 'inline-flex' }}
                  >
                    ستاره دادن
                  </Button>
                  <Button
                    component="a"
                    href={`${GITHUB_URL}/issues`}
                    target="_blank"
                    rel="noopener noreferrer"
                    leftSection={<FaHeart size={14} />}
                    size="xs"
                    variant="subtle"
                    style={{ display: 'inline-flex' }}
                  >
                    گزارش مشکل
                  </Button>
                </Group>
              </Stack>
            </Card>
          </StarBorder>
        </Grid.Col>
      </Grid>

      {}
      <Card withBorder radius="md" p="md" mb="md" className="card-enter">
        <Group justify="space-between" wrap="nowrap">
          <Stack gap={2}>
            <Title order={5}>اسکن بارکد کتاب</Title>
            <Text size="sm" c="dimmed">
              با دوربین یا تصویر، بارکد/QR کتاب را بخوانید و مستقیم به پروفایل آن بروید.
            </Text>
          </Stack>
          <Button leftSection={<FaQrcode />} onClick={() => navigate('/scan')}>
            اسکن
          </Button>
        </Group>
      </Card>

      {}
      <Grid gutter="md" align="stretch">
        <Grid.Col span={{ base: 12, md: 6 }}>
          <ChartViewer
            title="میزان بازدید (۳۰ روز اخیر)"
            type="bar"
            color="blue.6"
            data={visits}
            loading={loading}
            emptyMessage="هنوز بازدیدی ثبت نشده است"
            height={300}
          />
        </Grid.Col>

        <Grid.Col span={{ base: 12, md: 6 }}>
          <ChartViewer
            title="آمار کتاب‌ها بر اساس موضوع"
            type="bar"
            color="green.6"
            data={bookStats}
            loading={loading}
            emptyMessage="کتابی برای نمایش آمار وجود ندارد"
            height={300}
          />
        </Grid.Col>
      </Grid>
    </Stack>
  );
}
