import * as FaIcons from 'react-icons/fa';
import type { IconType } from 'react-icons/lib';

const iconRegistry = FaIcons as unknown as Record<string, IconType | undefined>;

export function getFaIcon(iconName?: string): IconType | undefined {
  if (!iconName) {
    return undefined;
  }
  return iconRegistry[iconName];
}
