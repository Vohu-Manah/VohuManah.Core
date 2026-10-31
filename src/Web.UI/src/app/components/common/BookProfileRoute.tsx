import { useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '@/app/context/AuthContext';
import { useRedirect } from '@/app/context/RedirectContext';
import { Loader, Center, Modal, Text as MantineText, Button, Stack } from '@mantine/core';
import BookProfilePage from '@/app/pages/BookProfilePage';

export default function BookProfileRoute() {
  const { isAuthenticated, loading } = useAuth();
  const { setRedirectUrl } = useRedirect();
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const bookId = searchParams.get('id');

  useEffect(() => {
    if (!loading && !isAuthenticated) {
      const currentUrl = window.location.hash.replace('#', '');
      setRedirectUrl(currentUrl);
    }
  }, [loading, isAuthenticated, setRedirectUrl]);

  if (loading) {
    return (
      <Center mih="100vh">
        <Loader />
      </Center>
    );
  }

  if (!isAuthenticated) {
    return (
      <Center mih="100vh">
        <Modal
          opened={true}
          onClose={() => navigate('/login')}
          title="ورود به سیستم"
          centered
        >
          <Stack gap="md">
            <MantineText>برای مشاهده پروفایل کتاب، لطفاً وارد سیستم شوید.</MantineText>
            <Button fullWidth onClick={() => navigate('/login')}>
              ورود به سیستم
            </Button>
          </Stack>
        </Modal>
      </Center>
    );
  }

  if (!bookId) {
    return (
      <Center mih="100vh">
        <MantineText>شناسه کتاب یافت نشد</MantineText>
      </Center>
    );
  }

  return <BookProfilePage bookId={Number(bookId)} />;
}

