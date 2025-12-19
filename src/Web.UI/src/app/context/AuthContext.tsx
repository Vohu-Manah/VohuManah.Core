import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import type { LoginRequest } from '../services/auth/authService';
import { login, refresh, revoke } from '../services/auth/authService';
import { clearTokens, getTokens } from '../services/api/tokenStore';
import { appMessage } from '../messages/appMessages';
import type { UserProfile } from '../models/user';

type AuthState = {
  user: UserProfile | null;
  accessToken: string | null;
  refreshToken: string | null;
};

type AuthContextValue = {
  user: UserProfile | null;
  isAuthenticated: boolean;
  loading: boolean;
  login: (payload: LoginRequest) => Promise<void>;
  logout: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

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
      .then((res) => {
        setState({
          user: { userName: res.userName, fullName: res.fullName },
          accessToken: res.accessToken,
          refreshToken: res.refreshToken,
        });
      })
      .catch(() => {
        clearTokens();
      })
      .finally(() => setLoading(false));
  }, []);

  const handleLogin = async (payload: LoginRequest) => {
    const result = await login(payload);
    setState({
      user: { userName: result.userName, fullName: result.fullName },
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

