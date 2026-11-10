import { useState, useEffect } from 'react';
import { getFaIcon } from '@/app/helpers/faIcons';
import { NavLink } from '@mantine/core';
import { FaChevronDown, FaChevronLeft } from 'react-icons/fa';
import { useLocation, useNavigate } from 'react-router-dom';
import { menuService } from '@/app/services/library/menuService';
import type { MenuItem } from '@/app/models/menu';
import { handleApiError } from '@/app/helpers/errorHandler';

type DynamicNavLinkProps = {
  menuItem: MenuItem;
  level?: number;
};

function DynamicNavLink({ menuItem, level = 0 }: DynamicNavLinkProps) {
  const { pathname } = useLocation();
  const navigate = useNavigate();
  const [opened, setOpened] = useState(false);

  const hasChildren = menuItem.children && menuItem.children.length > 0;

  const normalizePath = (path?: string) => {
    if (!path) return '';
    if (path.includes('#')) {
      const [basePath, hash] = path.split('#');
      const cleanPath = basePath.replace(/^#/, '');
      const normalized = cleanPath.startsWith('/') ? cleanPath : `/${cleanPath}`;
      return hash ? `${normalized}#${hash}` : normalized;
    }
    const cleanPath = path.replace(/^#/, '');
    return cleanPath.startsWith('/') ? cleanPath : `/${cleanPath}`;
  };

  
  
  
  const hashPath = window.location.hash.replace(/^#/, '');
  const currentLocation = hashPath || pathname;
  const currentPath = normalizePath(currentLocation);
  const itemPath = normalizePath(menuItem.path);

  const currentPathWithoutHash = currentPath.split('#')[0];
  const itemPathWithoutHash = itemPath.split('#')[0];
  const currentHash = currentLocation.includes('#') ? currentLocation.split('#')[1] : '';
  const itemHash = itemPath.includes('#') ? itemPath.split('#')[1] : '';
  
  const isExactActive = itemPathWithoutHash 
    ? (currentPathWithoutHash === itemPathWithoutHash && (!itemHash || currentHash === itemHash))
    : false;

  const hasActiveChildRecursive = (items: MenuItem[]): boolean => {
    return items.some((child) => {
      if (child.path) {
        const childPath = normalizePath(child.path);
        const childPathWithoutHash = childPath.split('#')[0];
        const childHash = childPath.includes('#') ? childPath.split('#')[1] : '';
        if (childHash) {
          if (currentPathWithoutHash === childPathWithoutHash && currentHash === childHash) {
            return true;
          }
        } else {
          if (currentPathWithoutHash === childPathWithoutHash || currentPathWithoutHash.startsWith(childPathWithoutHash + '/')) {
            return true;
          }
        }
      }
      
      if (child.children && child.children.length > 0) {
        return hasActiveChildRecursive(child.children);
      }
      
      return false;
    });
  };

  const hasActiveChild = hasChildren && hasActiveChildRecursive(menuItem.children!);
  const isActive = menuItem.path ? isExactActive : false;
  const shouldBeOpened = opened || hasActiveChild;
  


  useEffect(() => {
    if (hasActiveChild && !opened) {

      setOpened(true);
    }
  }, [hasActiveChild, opened]);

  
  
  
  const chevronOpen = shouldBeOpened || hasActiveChild;

  const handleClick = (e: React.MouseEvent) => {
    e.preventDefault();

    
    
    
    if (hasChildren) {
      setOpened(!opened);
    }

    if (menuItem.path) {
      if (menuItem.path.startsWith('/#/')) {
        const targetPath = menuItem.path.replace('/#/', '/');
        navigate(targetPath);
      } else if (menuItem.path.includes('#')) {
        const [path, hash] = menuItem.path.split('#');
        const cleanPath = path || '/';
        navigate(cleanPath + (hash ? `#${hash}` : ''));
      } else {
        const targetPath = menuItem.path.startsWith('/') ? menuItem.path : `/${menuItem.path}`;
        navigate(targetPath);
      }
    }
  };

  const getIconComponent = (iconName?: string) => {
    if (!iconName) return undefined;
    const IconComponent = getFaIcon(iconName);
    if (!IconComponent) return undefined;
    return <IconComponent size={14} />;
  };

  if (!menuItem.isActive) {
    return null;
  }

  return (
    <>
      <NavLink
        key={menuItem.id}
        label={menuItem.label}
        leftSection={getIconComponent(menuItem.icon)}
        rightSection={
          hasChildren ? (
            
            
            
            
            
            chevronOpen ? (
              <FaChevronDown size={12} className="opacity-70" />
            ) : (
              <FaChevronLeft size={12} className="opacity-70" />
            )
          ) : undefined
        }
        active={isActive}
        onClick={handleClick}
        childrenOffset={28}
        className="dir-rtl-ta-right"
      />
      {hasChildren && shouldBeOpened && (
        
        <div
          className="menu-children"
        >
          {menuItem.children!.map((child) => (
            <DynamicNavLink key={child.id} menuItem={child} level={level + 1} />
          ))}
        </div>
      )}
    </>
  );
}

export function DynamicMenu() {
  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    void (async () => {
      try {
        const data = await menuService.getAll();
        setMenuItems(data);
      } catch (error) {
        handleApiError(error, 'خطا در بارگذاری منو');
      } finally {
        setLoading(false);
      }
    })();
  }, []);


  if (loading) {
    return null;
  }

  return (
    <>
      {menuItems.map((item) => (
        <DynamicNavLink key={item.id} menuItem={item} />
      ))}
    </>
  );
}