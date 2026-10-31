import { useState, useEffect } from 'react';
import { getErrorMessage } from '@/app/helpers/errorHandler';
import {
  Anchor,
  Button,
  Checkbox,
  Container,
  Group,
  Paper,
  PasswordInput,
  Text,
  Title,
  Box,
  rem,
  Image,
} from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useForm, isNotEmpty } from '@mantine/form';
import { useAuth } from '@/app/context/AuthContext';
import { useRedirect } from '@/app/context/RedirectContext';
import { appMessage } from '@/app/messages/appMessages';
import { SmartTextInput } from '@/app/components/common/SmartTextInput';
import StarBorder from '@/app/components/common/StarBorder';
import HumanVerifyButton from '@/app/components/common/HumanVerifyButton';
import favicon from '@/assets/images/favicon.png';
import slideshow1 from '@/assets/images/slideshow/slideshow-1.jpg';
import slideshow2 from '@/assets/images/slideshow/slideshow-2.jpg';
import slideshow3 from '@/assets/images/slideshow/slideshow-3.jpeg';
import slideshow4 from '@/assets/images/slideshow/slideshow-4.jpg';
import slideshow5 from '@/assets/images/slideshow/slideshow-5.jpg';
import slideshow6 from '@/assets/images/slideshow/slideshow-6.jpg';
import slideshow7 from '@/assets/images/slideshow/slideshow-7.jpg';
import slideshow8 from '@/assets/images/slideshow/slideshow-8.jpg';
import slideshow9 from '@/assets/images/slideshow/slideshow-9.jpg';
import slideshow10 from '@/assets/images/slideshow/slideshow-10.jpg';
import slideshow11 from '@/assets/images/slideshow/slideshow-11.jpg';

const slideshowImages = [
  slideshow1,
  slideshow2,
  slideshow3,
  slideshow4,
  slideshow5,
  slideshow6,
  slideshow7,
  slideshow8,
  slideshow9,
  slideshow10,
  slideshow11,
];

export default function LoginPage() {
  const { login } = useAuth();
  const { redirectUrl, clearRedirectUrl } = useRedirect();
  const navigate = useNavigate();
  const [keepLoggedIn, setKeepLoggedIn] = useState(false);
  const [currentSlide, setCurrentSlide] = useState(0);
  const [humanToken, setHumanToken] = useState<string | null>(null);
  const [submitAttempt, setSubmitAttempt] = useState(0);

  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentSlide((prev) => (prev + 1) % slideshowImages.length);
    }, 10000);

    return () => clearInterval(interval);
  }, []);

  const form = useForm({
    initialValues: {
      userName: '',
      password: '',
    },
    validate: {
      userName: isNotEmpty('نام کاربری یا ایمیل را بنویسید'),
      password: isNotEmpty('رمز عبور را بنویسید'),
    },
  });

  const handleSubmit = async (values: typeof form.values) => {
    if (!humanToken) {
      appMessage.error('نشان دهید که ربات نیستید');
      return;
    }
    try {
      await login({ ...values, humanVerifyToken: humanToken });
      const targetUrl = redirectUrl || '/';
      clearRedirectUrl();
      navigate(targetUrl);
    } catch (error) {
      setSubmitAttempt((n) => n + 1);
      appMessage.error(getErrorMessage(error, 'چیزی نادرست پیش رفت؛ دوباره تلاش کنید'));
    }
  };

  return (
    <Box
      style={{
        display: 'flex',
        minHeight: '100vh',
        direction: 'ltr',
      }}
    >
      <Box
        style={{
          flex: 1,
          position: 'relative',
          display: 'none',
          alignItems: 'center',
          justifyContent: 'center',
          padding: rem(40),
          overflow: 'hidden',
        }}
        className="login-image-section"
      >
        {slideshowImages.map((img, index) => (
          <Box
            key={index}
            style={{
              position: 'absolute',
              top: 0,
              left: 0,
              width: '100%',
              height: '100%',
              backgroundImage: `url(${img})`,
              backgroundSize: 'cover',
              backgroundPosition: 'center',
              opacity: index === currentSlide ? 1 : 0,
              transition: 'opacity 1s ease-in-out',
              zIndex: 1,
            }}
          />
        ))}
        <Box
          style={{
            position: 'absolute',
            top: 0,
            left: 0,
            width: '100%',
            height: '100%',
            background:
              'linear-gradient(135deg, rgba(102, 126, 234, 0.7) 0%, rgba(118, 75, 162, 0.7) 100%)',
            zIndex: 2,
          }}
        />
        <Box
          style={{
            textAlign: 'center',
            color: 'white',
            direction: 'rtl',
            position: 'relative',
            zIndex: 3,
          }}
        >
          <Title className="login-hero-title" order={1} size={rem(48)} fw={900} mb="lg">
            به پنل مدیریتی خوش آمدید!
          </Title>
          <Text className="login-hero-text" size={rem(25)} fw={500} opacity={0.9}>
            سیستم مدیریت کتابخانه
          </Text>
        </Box>
      </Box>

      <Container
        size={420}
        style={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          padding: rem(40),
          direction: 'rtl',
        }}
      >
        <Group justify="center" gap="xs" mb="md">
          <Image src={favicon} alt="favicon" width={45} height={45} />
          <Title ta="center" fw={900}>
            وُهُومَنَهْ
          </Title>
        </Group>

        <StarBorder
          as="div"
          className="star-border--card star-border--login"
          theme="dark"
          color="#111827"
          trailColor="#4b5563"
          thickness={2.5}
          radius={12}
          stars={2}
          duration={5}
          glow={0.35}
          sparkle={false}
          hover="lap"
          clickPulse={false}
          style={{ marginTop: 30, height: 'auto' }}
        >
          <Paper withBorder shadow="md" p={30} radius="md" style={{ color: '#1a1a1a' }}>
            <form onSubmit={form.onSubmit(handleSubmit)}>
            <SmartTextInput
              label="نام کاربری یا ایمیل"
              placeholder="نام کاربری یا ایمیل خود را وارد کنید"
              required
              fieldName="userName"
              {...form.getInputProps('userName')}
            />

            <PasswordInput
              label="رمز عبور"
              placeholder="رمز عبور خود را وارد کنید"
              required
              mt="md"
              dir="ltr"
              {...form.getInputProps('password')}
            />

            <Group justify="space-between" mt="lg">
              <Checkbox
                label="مرا به خاطر بسپار"
                checked={keepLoggedIn}
                onChange={(event) => setKeepLoggedIn(event.currentTarget.checked)}
              />
              <Anchor
                component="button"
                type="button"
                size="sm"
                onClick={(e) => {
                  e.preventDefault();
                }}
              >
                رمز عبور را فراموش کرده‌اید؟
              </Anchor>
            </Group>

            <HumanVerifyButton onVerified={setHumanToken} resetKey={submitAttempt} />
            {!humanToken && (
              <Text size="xs" c="dimmed" ta="center" mt="xs">
                برای ورود، گامِ پذیرش انسانی را برآورده کنید
              </Text>
            )}
            <Button
              fullWidth
              mt="xl"
              type="submit"
              loading={form.submitting}
              disabled={!humanToken}
              color="#000"
            >
              ورود
            </Button>
            </form>
          </Paper>
        </StarBorder>
      </Container>

      <style>{`
        @media (min-width: 768px) {
          .login-image-section {
            display: flex !important;
          }
        }
      `}</style>
    </Box>
  );
}
