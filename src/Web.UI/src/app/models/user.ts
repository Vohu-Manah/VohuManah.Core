export type UserProfile = {
  userName: string;
  fullName: string;
};

export type LibraryUser = {
  id: number;
  userName: string;
  name: string;
  lastName: string;
  fullName: string;
};

export type CreateUserRequest = {
  userName: string;
  password: string;
  name: string;
  lastName: string;
};

export type UpdateUserRequest = {
  userName: string;
  password: string;
  name: string;
  lastName: string;
};
