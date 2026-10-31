export interface MenuItem {
  id: number;
  label: string;
  path?: string;
  icon?: string;
  parentId?: number;
  displayOrder: number;
  isActive: boolean;
  isSystem: boolean;
  children?: MenuItem[];
}

export interface CreateMenuItemRequest {
  label: string;
  path?: string;
  icon?: string;
  parentId?: number;
  displayOrder: number;
  isActive?: boolean;
}

export interface UpdateMenuItemRequest {
  id: number;
  label: string;
  path?: string;
  icon?: string;
  parentId?: number;
  displayOrder: number;
  isActive: boolean;
}

export interface MenuItemOrder {
  id: number;
  displayOrder: number;
  parentId?: number;
}

export interface ReorderMenuItemsRequest {
  items: MenuItemOrder[];
}

