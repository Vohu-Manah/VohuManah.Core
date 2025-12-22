import type { ReactElement } from 'react';
import {
  FaHome,
  FaUser,
  FaBook,
  FaUsers,
  FaCog,
  FaNewspaper,
  FaScroll,
  FaCity,
  FaLanguage,
  FaTags,
  FaBuilding,
  FaSearch,
  FaChartBar,
  FaFileAlt,
  FaShieldAlt,
  FaUserShield,
} from 'react-icons/fa';

export type AppRoute = {
  path: string;
  label: string;
  icon?: ReactElement;
  exact?: boolean;
  children?: AppRoute[];
};

export const menuRoutes: AppRoute[] = [
  { path: '/', label: 'داشبورد', icon: <FaHome size={14} /> },
  {
    label: 'کتاب‌ها',
    icon: <FaBook size={14} />,
    children: [
      { path: '/books', label: 'مدیریت کتاب‌ها' },
      { path: '/books/search', label: 'جستجوی کتاب' },
      { path: '/books/statistics', label: 'آمار کتاب‌ها' },
    ],
  },
  {
    label: 'نشریات',
    icon: <FaNewspaper size={14} />,
    children: [
      { path: '/publications', label: 'مدیریت نشریات' },
      { path: '/publications/search', label: 'جستجوی نشریه' },
      { path: '/publications/statistics', label: 'آمار نشریات' },
    ],
  },
  {
    label: 'نسخ خطی',
    icon: <FaScroll size={14} />,
    children: [
      { path: '/manuscripts', label: 'مدیریت نسخ خطی' },
      { path: '/manuscripts/search', label: 'جستجوی نسخه خطی' },
      { path: '/manuscripts/statistics', label: 'آمار نسخ خطی' },
    ],
  },
  {
    label: 'اطلاعات پایه',
    icon: <FaTags size={14} />,
    children: [
      { path: '/cities', label: 'شهرها' },
      { path: '/languages', label: 'زبان‌ها' },
      { path: '/subjects', label: 'موضوعات' },
      { path: '/publishers', label: 'ناشران' },
      { path: '/gaps', label: 'فاصله‌ها' },
      { path: '/publication-types', label: 'انواع نشریات' },
    ],
  },
  {
    label: 'کاربران',
    icon: <FaUsers size={14} />,
    children: [
      { path: '/users', label: 'مدیریت کاربران' },
    ],
  },
  { path: '/profile', label: 'پروفایل', icon: <FaUser size={14} /> },
  {
    label: 'گزارش‌ها',
    icon: <FaFileAlt size={14} />,
    children: [
      { path: '/reports', label: 'گزارش‌ها' },
    ],
  },
  {
    label: 'تنظیمات',
    icon: <FaCog size={14} />,
    children: [
      { path: '/settings', label: 'تنظیمات عمومی' },
    ],
  },
  {
    label: 'مدیریت ادمین',
    icon: <FaShieldAlt size={14} />,
    children: [
      { path: '/admin/roles', label: 'مدیریت نقش‌ها' },
      { path: '/admin/user-roles', label: 'نقش‌دهی به کاربران' },
    ],
  },
];

