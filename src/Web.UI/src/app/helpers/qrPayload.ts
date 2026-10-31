

const SCHEME = 'app://';


export function encodeQr(path: string): string {
  const normalized = path.replace(/^#/, '');
  return `${SCHEME}${normalized}`;
}


export function decodeQr(raw: string): string | null {
  if (!raw) return null;
  const value = raw.trim();
  if (!value) return null;

  
  if (value.startsWith(SCHEME)) {
    const inner = value.slice(SCHEME.length).replace(/^\/+/, '');
    return inner ? `/${inner}` : null;
  }

  
  const hashIndex = value.indexOf('#/');
  if (hashIndex !== -1) {
    const hashPath = value.slice(hashIndex + 2).replace(/^\/+/, '');
    return hashPath ? `/${hashPath}` : null;
  }

  
  if (/^\d+$/.test(value)) {
    return `/books/profile?id=${value}`;
  }

  
  if (value.startsWith('/') || value.includes('?')) {
    return `/${value.replace(/^\/+/, '')}`;
  }

  return null;
}
