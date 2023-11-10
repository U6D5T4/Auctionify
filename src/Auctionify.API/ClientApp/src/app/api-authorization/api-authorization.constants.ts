export const ApplicationName = 'Auctionify';

export const LoginActions = {
  Login: 'login',
  Profile: 'profile',
  Register: 'register',
  RegisterRole: 'register-role',
  LoginFailed: 'login-failed',
};

let applicationPaths: ApplicationPathsType = {
  Login: `auth/${LoginActions.Login}`,
  Register: `auth/${LoginActions.Register}`,
  RegisterRole: `auth/${LoginActions.RegisterRole}`,
  Profile: `auth/${LoginActions.Profile}`,
  LoginFailed: `auth/${LoginActions.LoginFailed}`,
  LoginPathComponents: [],
  RegisterPathComponents: [],
  RegisterRolePathComponent: [],
  ProfilePathComponents: [],
};

applicationPaths = {
  ...applicationPaths,
  LoginPathComponents: applicationPaths.Login.split('/'),
  RegisterPathComponents: applicationPaths.Register.split('/'),
  ProfilePathComponents: applicationPaths.Profile.split('/'),
  RegisterRolePathComponent: applicationPaths.Profile.split('/')
};

interface ApplicationPathsType {
  readonly Login: string;
  readonly LoginFailed: string;
  readonly Register: string;
  readonly RegisterRole: string;
  readonly Profile: string;
  readonly LoginPathComponents: string[];
  readonly RegisterPathComponents: string[];
  readonly RegisterRolePathComponent: string[];
  readonly ProfilePathComponents: string[];
}

export const ApplicationPaths: ApplicationPathsType = applicationPaths;