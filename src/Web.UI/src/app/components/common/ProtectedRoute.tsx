import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { Loader, Center } from '@mantine/core';

type Props = {
  children?: React.ReactNode;
};

export default function ProtectedRoute({ children }: Props) {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return (
      <Center mih="100vh">
        <Loader />
      </Center>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children ? <>{children}</> : <Outlet />;
}

