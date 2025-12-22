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
import ProtectedRoute from '../components/common/ProtectedRoute';
// Main entities
import BooksPage from '../pages/BooksPage';
import UsersPage from '../pages/UsersPage';
import PublicationsPage from '../pages/PublicationsPage';
import ManuscriptsPage from '../pages/ManuscriptsPage';
// Helper entities
import CitiesPage from '../pages/CitiesPage';
import LanguagesPage from '../pages/LanguagesPage';
import SubjectsPage from '../pages/SubjectsPage';
import PublishersPage from '../pages/PublishersPage';
import GapsPage from '../pages/GapsPage';
import PublicationTypesPage from '../pages/PublicationTypesPage';
// Search pages
import SearchBookPage from '../pages/SearchBookPage';
import SearchPublicationPage from '../pages/SearchPublicationPage';
import SearchManuscriptPage from '../pages/SearchManuscriptPage';
// Statistics pages (placeholders)
import BookStatisticsPage from '../pages/BookStatisticsPage';
import PublicationStatisticsPage from '../pages/PublicationStatisticsPage';
import ManuscriptStatisticsPage from '../pages/ManuscriptStatisticsPage';
// Other pages
import ReportsPage from '../pages/ReportsPage';
import SettingsPage from '../pages/SettingsPage';
// Admin pages
import RolesPage from '../pages/RolesPage';
import UserRolesPage from '../pages/UserRolesPage';

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
      { path: 'books', element: <BooksPage /> },
      { path: 'books/search', element: <SearchBookPage /> },
      { path: 'books/statistics', element: <BookStatisticsPage /> },
      // Publications routes
      { path: 'publications', element: <PublicationsPage /> },
      { path: 'publications/search', element: <SearchPublicationPage /> },
      { path: 'publications/statistics', element: <PublicationStatisticsPage /> },
      // Manuscripts routes
      { path: 'manuscripts', element: <ManuscriptsPage /> },
      { path: 'manuscripts/search', element: <SearchManuscriptPage /> },
      { path: 'manuscripts/statistics', element: <ManuscriptStatisticsPage /> },
      // Helper entities
      { path: 'cities', element: <CitiesPage /> },
      { path: 'languages', element: <LanguagesPage /> },
      { path: 'subjects', element: <SubjectsPage /> },
      { path: 'publishers', element: <PublishersPage /> },
      { path: 'gaps', element: <GapsPage /> },
      { path: 'publication-types', element: <PublicationTypesPage /> },
      // Users routes
      { path: 'users', element: <UsersPage /> },
      // Reports
      { path: 'reports', element: <ReportsPage /> },
      // Settings
      { path: 'settings', element: <SettingsPage /> },
      // Admin
      { path: 'admin/roles', element: <RolesPage /> },
      { path: 'admin/user-roles', element: <UserRolesPage /> },
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

