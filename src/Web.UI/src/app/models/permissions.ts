 
export const PERM = {
  Book: {
    create: 'Library.Books.Create',
    update: 'Library.Books.Update',
    del: 'Library.Books.Delete',
    attach: 'Library.Attachments.Upload',
  },
  Manuscript: {
    create: 'Library.Manuscripts.Create',
    update: 'Library.Manuscripts.Update',
    del: 'Library.Manuscripts.Delete',
  },
  Publication: {
    create: 'Library.Publications.Create',
    update: 'Library.Publications.Update',
    del: 'Library.Publications.Delete',
  },
  City: { create: 'Library.Cities.Create', update: 'Library.Cities.Update', del: 'Library.Cities.Delete' },
  Language: { create: 'Library.Languages.Create', update: 'Library.Languages.Update', del: 'Library.Languages.Delete' },
  Subject: { create: 'Library.Subjects.Create', update: 'Library.Subjects.Update', del: 'Library.Subjects.Delete' },
  Publisher: { create: 'Library.Publishers.Create', update: 'Library.Publishers.Update', del: 'Library.Publishers.Delete' },
  PublicationType: { create: 'Library.PublicationTypes.Create', update: 'Library.PublicationTypes.Update', del: 'Library.PublicationTypes.Delete' },
  Gap: { create: 'Library.Gaps.Create', update: 'Library.Gaps.Update', del: 'Library.Gaps.Delete' },
  User: { create: 'Library.Users.Create', update: 'Library.Users.Update', del: 'Library.Users.Delete', revoke: 'Library.Users.Revoke', updateProfile: 'Library.Users.UpdateProfile', changePassword: 'Library.Users.ChangePassword' },
  Role: { create: 'Library.Settings.CreateRole', update: 'Library.Settings.UpdateRoleEndpoints', del: 'Library.Settings.UpdateRoleEndpoints' },
  UserRole: { assign: 'Library.Settings.AssignRoleToUser', remove: 'Library.Settings.RemoveRoleFromUser' },
  Menu: { create: 'Library.Menu.Create', update: 'Library.Menu.Update', del: 'Library.Menu.Delete', reorder: 'Library.Menu.Reorder', reset: 'Library.Menu.ResetToDefault' },
  Backup: { create: 'Library.Backup.Create', restore: 'Library.Backup.Restore', download: 'Library.Backup.Download', cancel: 'Library.Backup.Cancel', getInfo: 'Library.Backup.GetInfo' },
  Settings: {
    appInfo: 'Library.Settings.GetAppInfo',
    systemInfo: 'Library.Settings.GetSystemInfo',
    updateUser: 'Library.Settings.UpdateUserSettings',
  },
} as const;

export type PermissionName = (typeof PERM)[keyof typeof PERM][keyof (typeof PERM)[keyof typeof PERM]];
