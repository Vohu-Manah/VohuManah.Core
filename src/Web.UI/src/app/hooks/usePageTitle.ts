import { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import { menuService } from '@/app/services/library/menuService';
import type { MenuItem } from '@/app/models/menu';

function findMenuItemByPath(menuItems: MenuItem[], path: string): MenuItem | null {
  const normalizePath = (p?: string) => {
    if (!p) return '';
    if (p.includes('#')) {
      const [basePath] = p.split('#');
      const cleanPath = basePath.replace(/^#/, '');
      return cleanPath.startsWith('/') ? cleanPath : `/${cleanPath}`;
    }
    const cleanPath = p.replace(/^#/, '');
    return cleanPath.startsWith('/') ? cleanPath : `/${cleanPath}`;
  };

  const currentPath = normalizePath(path);
  const currentPathWithoutHash = currentPath.split('#')[0];

  for (const item of menuItems) {
    if (item.path) {
      const itemPath = normalizePath(item.path);
      const itemPathWithoutHash = itemPath.split('#')[0];
      
      if (itemPathWithoutHash === currentPathWithoutHash) {
        return item;
      }
    }

    if (item.children && item.children.length > 0) {
      const found = findMenuItemByPath(item.children, path);
      if (found) return found;
    }
  }

  return null;
}

export function usePageTitle() {
  const location = useLocation();
  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
  const [, setPageName] = useState<string>('وهومنه');

  useEffect(() => {
    const loadMenu = async () => {
      try {
        const data = await menuService.getAll();
        setMenuItems(data);
      } catch {
      
    }
    };

    loadMenu();
  }, []);

  useEffect(() => {
    if (menuItems.length === 0) {
      document.title = 'وهومنه';
      return;
    }

    const currentPath = location.pathname;
    const menuItem = findMenuItemByPath(menuItems, currentPath);
    
    if (menuItem) {

      setPageName(menuItem.label);
      document.title = `وهومنه - ${menuItem.label}`;
    } else {
      
      if (currentPath === '/login') {
        document.title = 'وهومنه - ورود';
      } else {
        document.title = 'وهومنه';
      }
    }
  }, [location.pathname, menuItems]);
}
