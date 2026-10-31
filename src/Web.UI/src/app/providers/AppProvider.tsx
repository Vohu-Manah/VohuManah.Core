import { useState, useEffect } from 'react';
import { createHashRouter, RouterProvider, Navigate } from 'react-router-dom';
import { MantineProvider } from '@mantine/core';
import { theme } from '@/app/config/theme';
import { settingsService } from '@/app/services/library/settingsService';
import { createMantineTheme, DEFAULT_THEME } from '@/app/helpers/themeColors';
import { DatesProvider } from '@mantine/dates';
import { Notifications } from '@mantine/notifications';
import '@mantine/core/styles.css';
import '@mantine/dates/styles.css';
import '@mantine/notifications/styles.css';
import 'react-multi-date-picker/styles/layouts/prime.css';
import 'react-multi-date-picker/styles/colors/purple.css';
import { AuthProvider, useAuth } from '@/app/context/AuthContext';
import { UserSettingsProvider } from '@/app/context/UserSettingsContext';
import { ColorSchemeProvider, useColorScheme } from '@/app/context/ColorSchemeContext';
import { RedirectProvider } from '@/app/context/RedirectContext';
import MainLayout from '@/app/components/layout/MainLayout';
import LoginPage from '@/app/pages/LoginPage';
import DashboardPage from '@/app/pages/DashboardPage';
import ProtectedRoute from '@/app/components/common/ProtectedRoute';
import BooksPage from '@/app/pages/BooksPage';
import UsersPage from '@/app/pages/UsersPage';
import PublicationsPage from '@/app/pages/PublicationsPage';
import ManuscriptsPage from '@/app/pages/ManuscriptsPage';
import CitiesPage from '@/app/pages/CitiesPage';
import LanguagesPage from '@/app/pages/LanguagesPage';
import SubjectsPage from '@/app/pages/SubjectsPage';
import PublishersPage from '@/app/pages/PublishersPage';
import GapsPage from '@/app/pages/GapsPage';
import PublicationTypesPage from '@/app/pages/PublicationTypesPage';
import SearchBookPage from '@/app/pages/SearchBookPage';
import SearchPublicationPage from '@/app/pages/SearchPublicationPage';
import SearchManuscriptPage from '@/app/pages/SearchManuscriptPage';
import BookStatisticsPage from '@/app/pages/BookStatisticsPage';
import PublicationStatisticsPage from '@/app/pages/PublicationStatisticsPage';
import ManuscriptStatisticsPage from '@/app/pages/ManuscriptStatisticsPage';
import ReportsPage from '@/app/pages/ReportsPage';
import SettingsPage from '@/app/pages/SettingsPage';
import MenuSettingsPage from '@/app/pages/MenuSettingsPage';
import RolesPage from '@/app/pages/RolesPage';
import UserRolesPage from '@/app/pages/UserRolesPage';
import BookProfileRouteWrapper from '@/app/components/common/BookProfileRouteWrapper';
import ScanPage from '@/app/pages/ScanPage';

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
      { path: 'books', element: <BooksPage /> },
      { path: 'scan', element: <ScanPage /> },
      { path: 'books/profile', element: <BookProfileRouteWrapper /> },
      { path: 'books/search', element: <SearchBookPage /> },
      { path: 'books/statistics', element: <BookStatisticsPage /> },
      { path: 'publications', element: <PublicationsPage /> },
      { path: 'publications/search', element: <SearchPublicationPage /> },
      { path: 'publications/statistics', element: <PublicationStatisticsPage /> },
      { path: 'manuscripts', element: <ManuscriptsPage /> },
      { path: 'manuscripts/search', element: <SearchManuscriptPage /> },
      { path: 'manuscripts/statistics', element: <ManuscriptStatisticsPage /> },
      { path: 'cities', element: <CitiesPage /> },
      { path: 'languages', element: <LanguagesPage /> },
      { path: 'subjects', element: <SubjectsPage /> },
      { path: 'publishers', element: <PublishersPage /> },
      { path: 'gaps', element: <GapsPage /> },
      { path: 'publication-types', element: <PublicationTypesPage /> },
      { path: 'users', element: <UsersPage /> },
      { path: 'reports', element: <ReportsPage /> },
      { path: 'settings', element: <SettingsPage /> },
      { path: 'menu-settings', element: <MenuSettingsPage /> },
      { path: 'admin/roles', element: <RolesPage /> },
      { path: 'admin/user-roles', element: <UserRolesPage /> },
    ],
  },
  {
    path: '*',
    element: <Navigate to="/" replace />,
  },
]);

function ThemedApp() {
  const { colorScheme } = useColorScheme();
  const { isAuthenticated } = useAuth();
  
  
  const [dynamicTheme, setDynamicTheme] = useState(() => createMantineTheme(theme, DEFAULT_THEME.primary));

  useEffect(() => {
    let cancelled = false;
    if (isAuthenticated) {
      settingsService.getUserSettings()
        .then((s) => {
          if (cancelled) return;
          setDynamicTheme(createMantineTheme(theme, s.themePrimaryColor || DEFAULT_THEME.primary));
        })
        .catch(() => {});
    } else {
      setDynamicTheme(createMantineTheme(theme, DEFAULT_THEME.primary));
    }
    const onThemeChange = (e: Event) => {
      const primary = (e as CustomEvent<{ primary: string }>).detail?.primary;
      if (primary) setDynamicTheme(createMantineTheme(theme, primary));
    };
    window.addEventListener('theme-colors-changed', onThemeChange);
    return () => {
      cancelled = true;
      window.removeEventListener('theme-colors-changed', onThemeChange);
    };
  }, [isAuthenticated]);

  return (
    <MantineProvider theme={dynamicTheme} defaultColorScheme={colorScheme} forceColorScheme={colorScheme}>
      <DatesProvider settings={{ locale: 'fa', firstDayOfWeek: 6, weekendDays: [5] }}>
        <Notifications />
        <RedirectProvider>
          <RouterProvider router={router} />
        </RedirectProvider>
      </DatesProvider>
    </MantineProvider>
  );
}

export default function AppProvider() {
  return (
    <ColorSchemeProvider>
      {}
      <AuthProvider>
        <UserSettingsProvider>
          <ThemedApp />
        </UserSettingsProvider>
      </AuthProvider>
    </ColorSchemeProvider>
  );
}
