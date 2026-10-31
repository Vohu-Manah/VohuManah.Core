import {
  createContext, useContext, useState, useEffect, useMemo, type ReactNode,
} from 'react';
import { settingsService } from '@/app/services/library/settingsService';
import type { UserSettings } from '@/app/models/settings';
import { handleApiError } from '@/app/helpers/errorHandler';
import { useAuth } from '@/app/context/AuthContext';

type UserSettingsContextValue = {
  userSettings: UserSettings | null;
  loading: boolean;
  reload: () => Promise<void>;
};

const UserSettingsContext = createContext<UserSettingsContextValue | undefined>(undefined);

export function UserSettingsProvider({ children }: { children: ReactNode }) {
  const [userSettings, setUserSettings] = useState<UserSettings | null>(null);
  const [loading, setLoading] = useState(false);
  const { isAuthenticated, loading: authLoading } = useAuth();

  const loadUserSettings = async () => {
    if (!isAuthenticated) {
      setUserSettings(null);
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      const settings = await settingsService.getUserSettings();
      setUserSettings(settings);
    } catch (error) {
      handleApiError(error, 'خطا در بارگذاری تنظیمات کاربر');
      setUserSettings(null);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!authLoading) {
      loadUserSettings();
    }
    
  }, [isAuthenticated, authLoading]);

  const value = useMemo<UserSettingsContextValue>(
    () => ({
      userSettings,
      loading,
      reload: loadUserSettings,
    }),
    [userSettings, loading],
  );

  return <UserSettingsContext.Provider value={value}>{children}</UserSettingsContext.Provider>;
}

export function useUserSettings() {
  const ctx = useContext(UserSettingsContext);
  if (!ctx) {
    throw new Error('useUserSettings must be used within UserSettingsProvider');
  }
  return ctx;
}

