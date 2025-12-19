type Tokens = {
  accessToken: string;
  refreshToken: string;
  userName?: string;
  fullName?: string;
};

const STORAGE_KEY = 'vm_auth_tokens';

let cached: Tokens | null = load();

function load(): Tokens | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as Tokens) : null;
  } catch {
    return null;
  }
}

function persist(tokens: Tokens | null) {
  cached = tokens;
  if (tokens) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(tokens));
  } else {
    localStorage.removeItem(STORAGE_KEY);
  }
}

export function getTokens(): Tokens | null {
  return cached;
}

export function setTokens(tokens: Tokens) {
  persist(tokens);
}

export function clearTokens() {
  persist(null);
}

