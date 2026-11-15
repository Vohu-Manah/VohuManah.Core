import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import type { LoginRequest } from '@/app/services/auth/authService';
import { login, refresh, revoke } from '@/app/services/auth/authService';
import { clearAuthMetadata, getAuthMetadata } from '@/app/services/api/tokenStore';
import { appMessage } from '@/app/messages/appMessages';
import type { UserProfile } from '@/app/models/user';
import { userService } from '@/app/services/library/userService';

type AuthState = {
  user: UserProfile | null;
  isAuthenticated: boolean;
};

type AuthContextValue = {
  user: UserProfile | null;
  isAuthenticated: boolean;
  loading: boolean;
  can: (permission: string) => boolean;
  login: (payload: LoginRequest) => Promise<void>;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

async function loadProfilePermissions(userName: string | undefined, fullName: string | undefined): Promise<UserProfile | null> {
  try {
    const profile = await userService.getCurrentProfile();
    return { userName: profile.userName ?? userName, fullName: profile.fullName ?? fullName, permissions: profile.permissions ?? [] };
  } catch {
    return { userName: userName ?? '', fullName: fullName ?? '', permissions: [] };
  }
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [state, setState] = useState<AuthState>({
    user: null,
    isAuthenticated: false,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const metadata = getAuthMetadata();
    if (!metadata) {
      setLoading(false);
      return;
    }

    refresh()
      .then(async (res) => {
        const profile = await loadProfilePermissions(res.userName, res.fullName);
        setState({ user: profile, isAuthenticated: true });
      })
      .catch(() => {
        clearAuthMetadata();
        if (typeof window !== 'undefined') {
          window.location.hash = '#/login';
        }
      })
      .finally(() => setLoading(false));
  }, []);

  const handleLogin = async (payload: LoginRequest) => {
    const result = await login(payload);
    const profile = await loadProfilePermissions(result.userName, result.fullName);
    setState({ user: profile, isAuthenticated: true });
    appMessage.success('ok');
  };

  const handleLogout = async () => {
    await revoke();
    setState({ user: null, isAuthenticated: false });
  };

  const value = useMemo<AuthContextValue>(
    () => ({
      user: state.user,
      isAuthenticated: state.isAuthenticated,
      loading,
      can: (permission: string) => {
        if (!state.isAuthenticated) return false;
        const perms = state.user?.permissions;
        return !!perms && perms.includes(permission);
      },
      login: handleLogin,
      logout: handleLogout,
    }),
    [state.user, state.isAuthenticated, loading],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return ctx;
}
