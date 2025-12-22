export type Role = {
  id: number;
  name: string;
};

export type CreateRoleRequest = {
  name: string;
};

export type UpdateRolePermissionsRequest = {
  roleId: number;
  endpointNames: string[];
};

export type UserRoleResponse = {
  endpointName: string;
};

