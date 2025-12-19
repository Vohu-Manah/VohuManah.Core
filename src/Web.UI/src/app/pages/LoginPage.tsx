import { useState } from 'react';
import {
  Anchor,
  Button,
  Checkbox,
  Container,
  Group,
  Paper,
  PasswordInput,
  Text,
  TextInput,
  Title,
  Box,
  rem,
} from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useForm, isNotEmpty } from '@mantine/form';
import { useAuth } from '../context/AuthContext';
import { appMessage } from '../messages/appMessages';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [keepLoggedIn, setKeepLoggedIn] = useState(false);

  const form = useForm({
    initialValues: {
      userName: '',
      password: '',
    },
    validate: {
      userName: isNotEmpty('نام کاربری الزامی است'),
      password: isNotEmpty('رمز عبور الزامی است'),
    },
  });

  const handleSubmit = async (values: typeof form.values) => {
    try {
      await login(values);
      navigate('/');
    } catch (error: any) {
      appMessage.error(error.message ?? 'خطا در ورود');
    }
  };

  return (
    <Box
      style={{
        display: 'flex',
        minHeight: '100vh',
        direction: 'ltr', // Force LTR for layout
      }}
    >
      {/* Image Section - Left side */}
      <Box
        style={{
          flex: 1,
          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
          display: 'none',
          alignItems: 'center',
          justifyContent: 'center',
          padding: rem(40),
        }}
        className="login-image-section"
      >
        <Box style={{ textAlign: 'center', color: 'white', direction: 'rtl' }}>
          <Title order={1} size={rem(48)} fw={900} mb="md">
            به پنل مدیریتی خوش آمدید!
          </Title>
          <Text size="lg" opacity={0.9}>
            سیستم مدیریت کتابخانه
          </Text>
        </Box>
      </Box>

      {/* Form Section - Right side */}
      <Container
        size={420}
        style={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          padding: rem(40),
          direction: 'rtl', // RTL for form content
        }}
      >
        <Title ta="center" fw={900}>
        وُهُومَنَهْ
        </Title>

        <Paper withBorder shadow="md" p={30} mt={30} radius="md">
          <form onSubmit={form.onSubmit(handleSubmit)}>
            <TextInput
              label="آدرس ایمیل"
              placeholder="hello@mantine.dev"
              required
              {...form.getInputProps('userName')}
            />

            <PasswordInput
              label="رمز عبور"
              placeholder="رمز عبور خود را وارد کنید"
              required
              mt="md"
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
                  // Handle forgot password
                }}
              >
                رمز عبور را فراموش کرده‌اید؟
              </Anchor>
            </Group>

            <Button fullWidth mt="xl" type="submit" loading={form.submitting}>
              ورود
            </Button>
          </form>
        </Paper>
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

