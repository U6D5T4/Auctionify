export const ApplicationName = 'Auctionify';

export const LoginActions = {
  Login: 'login',
  Profile: 'profile',
  Register: 'register',
  LoginFailed: 'login-failed',
};

let applicationPaths: ApplicationPathsType = {
  Login: `auth/${LoginActions.Login}`,
  Register: `auth/${LoginActions.Register}`,
  Profile: `auth/${LoginActions.Profile}`,
  LoginFailed: `auth/${LoginActions.LoginFailed}`,
  LoginPathComponents: [],
  RegisterPathComponents: [],
  ProfilePathComponents: [],
};

applicationPaths = {
  ...applicationPaths,
  LoginPathComponents: applicationPaths.Login.split('/'),
  RegisterPathComponents: applicationPaths.Register.split('/'),
  ProfilePathComponents: applicationPaths.Profile.split('/'),
};

interface ApplicationPathsType {
  readonly Login: string;
  readonly LoginFailed: string;
  readonly Register: string;
  readonly Profile: string;
  readonly LoginPathComponents: string[];
  readonly RegisterPathComponents: string[];
  readonly ProfilePathComponents: string[];
}

export const ApplicationPaths: ApplicationPathsType = applicationPaths;