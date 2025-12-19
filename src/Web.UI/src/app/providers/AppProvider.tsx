import { createHashRouter, RouterProvider, Navigate } from 'react-router-dom';
import { MantineProvider } from '@mantine/core';
import { DatesProvider } from '@mantine/dates';
import { Notifications } from '@mantine/notifications';
import '@mantine/core/styles.css';
import '@mantine/dates/styles.css';
import '@mantine/notifications/styles.css';
import 'react-multi-date-picker/styles/layouts/prime.css';
import 'react-multi-date-picker/styles/colors/purple.css';
import { theme } from '../config/theme';
import { AuthProvider } from '../context/AuthContext';
import MainLayout from '../components/layout/MainLayout';
import LoginPage from '../pages/LoginPage';
import DashboardPage from '../pages/DashboardPage';
import ProfilePage from '../pages/ProfilePage';
import FormExamplePage from '../pages/FormExamplePage';
import PlaceholderPage from '../pages/PlaceholderPage';
import ProtectedRoute from '../components/common/ProtectedRoute';

const router = createHashRouter([
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    path: '/',
    element: (
      <ProtectedRoute>
        <MainLayout />
      </ProtectedRoute>
    ),
    children: [
      { index: true, element: <DashboardPage /> },
      { path: 'profile', element: <ProfilePage /> },
      { path: 'form-example', element: <FormExamplePage /> },
      // Books routes
      { path: 'books', element: <PlaceholderPage title="مدیریت کتاب‌ها" description="صفحه مدیریت کتاب‌ها به زودی اضافه خواهد شد." /> },
      { path: 'books/categories', element: <PlaceholderPage title="دسته‌بندی کتاب‌ها" description="صفحه مدیریت دسته‌بندی‌ها به زودی اضافه خواهد شد." /> },
      { path: 'books/authors', element: <PlaceholderPage title="نویسندگان" description="صفحه مدیریت نویسندگان به زودی اضافه خواهد شد." /> },
      // Users routes
      { path: 'users', element: <PlaceholderPage title="مدیریت کاربران" description="صفحه مدیریت کاربران به زودی اضافه خواهد شد." /> },
      { path: 'users/roles', element: <PlaceholderPage title="نقش‌های کاربری" description="صفحه مدیریت نقش‌ها به زودی اضافه خواهد شد." /> },
      { path: 'users/permissions', element: <PlaceholderPage title="مجوزها" description="صفحه مدیریت مجوزها به زودی اضافه خواهد شد." /> },
      // Forms routes
      { path: 'forms/simple', element: <PlaceholderPage title="فرم ساده" description="صفحه فرم ساده به زودی اضافه خواهد شد." /> },
      // Settings routes
      { path: 'settings', element: <PlaceholderPage title="تنظیمات عمومی" description="صفحه تنظیمات عمومی به زودی اضافه خواهد شد." /> },
      { path: 'settings/theme', element: <PlaceholderPage title="تنظیمات تم" description="صفحه تنظیمات تم به زودی اضافه خواهد شد." /> },
      { path: 'settings/notifications', element: <PlaceholderPage title="تنظیمات اعلان‌ها" description="صفحه تنظیمات اعلان‌ها به زودی اضافه خواهد شد." /> },
      // Security routes
      { path: 'security/2fa', element: <PlaceholderPage title="فعال‌سازی احراز هویت دو مرحله‌ای" description="صفحه فعال‌سازی 2FA به زودی اضافه خواهد شد." /> },
      { path: 'security/password', element: <PlaceholderPage title="تغییر رمز عبور" description="صفحه تغییر رمز عبور به زودی اضافه خواهد شد." /> },
      { path: 'security/recovery', element: <PlaceholderPage title="کدهای بازیابی" description="صفحه مدیریت کدهای بازیابی به زودی اضافه خواهد شد." /> },
    ],
  },
  {
    path: '*',
    element: <Navigate to="/" replace />,
  },
]);

export default function AppProvider() {
  return (
    <MantineProvider theme={theme} defaultColorScheme="light" defaultDirection="rtl">
      <DatesProvider settings={{ locale: 'fa', firstDayOfWeek: 6, weekendDays: [5] }}>
        <Notifications />
        <AuthProvider>
          <RouterProvider router={router} />
        </AuthProvider>
      </DatesProvider>
    </MantineProvider>
  );
}

