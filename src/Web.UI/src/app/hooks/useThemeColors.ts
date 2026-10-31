import { useEffect } from 'react';
import { settingsService } from '@/app/services/library/settingsService';
import { applyThemeColors, DEFAULT_THEME } from '@/app/helpers/themeColors';
import { useAuth } from '@/app/context/AuthContext';


export function useThemeColors() {
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    let cancelled = false;

    const load = async () => {
      if (!isAuthenticated) {
        applyThemeColors(DEFAULT_THEME);
        return;
      }
      try {
        const settings = await settingsService.getUserSettings();
        if (cancelled) return;

        applyThemeColors({
          primary: settings.themePrimaryColor || DEFAULT_THEME.primary,
          secondary: settings.themeSecondaryColor || DEFAULT_THEME.secondary,
          accent: settings.themeAccentColor || DEFAULT_THEME.accent,
        });
      } catch {
        if (cancelled) return;
        
        applyThemeColors(DEFAULT_THEME);
      }
    };

    load();

    const onScheme = () => load();
    window.addEventListener('color-scheme-changed', onScheme);

    return () => {
      cancelled = true;
      window.removeEventListener('color-scheme-changed', onScheme);
    };
  }, [isAuthenticated]);
}
