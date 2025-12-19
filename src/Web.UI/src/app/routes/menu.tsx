import type { ReactElement } from 'react';
import { FaHome, FaUser, FaWpforms, FaBook, FaUsers, FaCog, FaShieldAlt } from 'react-icons/fa';

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
      { path: '/books/categories', label: 'دسته‌بندی‌ها' },
      { path: '/books/authors', label: 'نویسندگان' },
    ],
  },
  {
    label: 'کاربران',
    icon: <FaUsers size={14} />,
    children: [
      { path: '/users', label: 'مدیریت کاربران' },
      { path: '/users/roles', label: 'نقش‌ها' },
      { path: '/users/permissions', label: 'مجوزها' },
    ],
  },
  {
    label: 'فرم‌ها',
    icon: <FaWpforms size={14} />,
    children: [
      { path: '/form-example', label: 'نمونه فرم پیشرفته' },
      { path: '/forms/simple', label: 'فرم ساده' },
    ],
  },
  { path: '/profile', label: 'پروفایل', icon: <FaUser size={14} /> },
  {
    label: 'تنظیمات',
    icon: <FaCog size={14} />,
    children: [
      { path: '/settings', label: 'تنظیمات عمومی' },
      { path: '/settings/theme', label: 'تم برنامه' },
      { path: '/settings/notifications', label: 'اعلان‌ها' },
    ],
  },
  {
    label: 'امنیت',
    icon: <FaShieldAlt size={14} />,
    children: [
      { path: '/security/2fa', label: 'فعال‌سازی ۲FA' },
      { path: '/security/password', label: 'تغییر رمز عبور' },
      { path: '/security/recovery', label: 'کدهای بازیابی' },
    ],
  },
];

