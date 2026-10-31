import { useSearchParams, Navigate } from 'react-router-dom';
import { useAuth } from '@/app/context/AuthContext';
import { useRedirect } from '@/app/context/RedirectContext';
import { useEffect } from 'react';
import BookProfilePage from '@/app/pages/BookProfilePage';

export default function BookProfileRouteWrapper() {
  const { isAuthenticated, loading } = useAuth();
  const { setRedirectUrl } = useRedirect();
  const [searchParams] = useSearchParams();
  const bookId = searchParams.get('id');

  useEffect(() => {
    if (!loading && !isAuthenticated) {
      const currentUrl = window.location.hash.replace('#', '');
      setRedirectUrl(currentUrl);
    }
  }, [loading, isAuthenticated, setRedirectUrl]);

  if (loading) {
    return null;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (!bookId) {
    return <Navigate to="/books" replace />;
  }

  return <BookProfilePage bookId={Number(bookId)} />;
}

