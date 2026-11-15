type AuthMetadata = {
  userName?: string;
  fullName?: string;
};

const STORAGE_KEY = 'vm_auth_metadata';

let cached: AuthMetadata | null = load();

function load(): AuthMetadata | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as AuthMetadata) : null;
  } catch {
    return null;
  }
}

function persist(metadata: AuthMetadata | null) {
  cached = metadata;
  if (metadata) {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(metadata));
  } else {
    localStorage.removeItem(STORAGE_KEY);
  }
}

export function getAuthMetadata(): AuthMetadata | null {
  return cached;
}

export function setAuthMetadata(metadata: AuthMetadata) {
  persist(metadata);
}

export function clearAuthMetadata() {
  persist(null);
}
