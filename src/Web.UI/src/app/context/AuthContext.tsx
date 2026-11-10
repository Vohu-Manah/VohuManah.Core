import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import type { LoginRequest } from '@/app/services/auth/authService';
import { login, refresh, revoke } from '@/app/services/auth/authService';
import { clearTokens, getTokens } from '@/app/services/api/tokenStore';
import { appMessage } from '@/app/messages/appMessages';
import type { UserProfile } from '@/app/models/user';
import { userService } from '@/app/services/library/userService';

type AuthState = {
  user: UserProfile | null;
  accessToken: string | null;
  refreshToken: string | null;
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

async function loadProfilePermissions(userName: string | undefined, fullName: string | undefined, accessToken: string | null): Promise<UserProfile | null> {
  if (!accessToken) return null;
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
    accessToken: null,
    refreshToken: null,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const tokens = getTokens();
    if (!tokens) {

      setLoading(false);
      return;
    }

    refresh()
      .then(async (res) => {
        const permissions = await loadProfilePermissions(res.userName, res.fullName, res.accessToken);
        setState({
          user: permissions,
          accessToken: res.accessToken,
          refreshToken: res.refreshToken,
        });
      })
      .catch(() => {
        clearTokens();
        if (typeof window !== 'undefined') {
          window.location.hash = '#/login';
        }
      })
      .finally(() => setLoading(false));
  }, []);

  const handleLogin = async (payload: LoginRequest) => {
    const result = await login(payload);
    const permissions = await loadProfilePermissions(result.userName, result.fullName, result.accessToken);
    setState({
      user: permissions,
      accessToken: result.accessToken,
      refreshToken: result.refreshToken,
    });
    appMessage.success('ok');
  };

  const handleLogout = async () => {
    await revoke();
    setState({ user: null, accessToken: null, refreshToken: null });
  };

  const value = useMemo<AuthContextValue>(
    () => ({
      user: state.user,
      isAuthenticated: !!state.accessToken,
      loading,
      can: (permission: string) => {
        if (!state.accessToken) return false;
        const perms = state.user?.permissions;
        return !!perms && perms.includes(permission);
      },
      login: handleLogin,
      logout: handleLogout,
    }),
    [state.user, state.accessToken, loading],
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