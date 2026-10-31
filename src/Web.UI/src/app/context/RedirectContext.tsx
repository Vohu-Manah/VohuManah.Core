import { createContext, useContext, useState, useCallback } from 'react';

type RedirectContextValue = {
  redirectUrl: string | null;
  setRedirectUrl: (url: string | null) => void;
  clearRedirectUrl: () => void;
};

const RedirectContext = createContext<RedirectContextValue | undefined>(undefined);

export function RedirectProvider({ children }: { children: React.ReactNode }) {
  const [redirectUrl, setRedirectUrlState] = useState<string | null>(() => {
    if (typeof window !== 'undefined') {
      return sessionStorage.getItem('redirectUrl');
    }
    return null;
  });

  const setRedirectUrl = useCallback((url: string | null) => {
    setRedirectUrlState(url);
    if (typeof window !== 'undefined') {
      if (url) {
        sessionStorage.setItem('redirectUrl', url);
      } else {
        sessionStorage.removeItem('redirectUrl');
      }
    }
  }, []);

  const clearRedirectUrl = useCallback(() => {
    setRedirectUrlState(null);
    if (typeof window !== 'undefined') {
      sessionStorage.removeItem('redirectUrl');
    }
  }, []);

  const value = {
    redirectUrl,
    setRedirectUrl,
    clearRedirectUrl,
  };

  return <RedirectContext.Provider value={value}>{children}</RedirectContext.Provider>;
}

export function useRedirect() {
  const ctx = useContext(RedirectContext);
  if (!ctx) {
    throw new Error('useRedirect must be used within RedirectProvider');
  }
  return ctx;
}

