export const ApplicationName = 'Auctionify';

export const LoginActions = {
  Login: 'login',
  Profile: 'profile',
  Register: 'register',
  LoginFailed: 'login-failed',
  ForgotPassword: 'forgot-password'
};

let applicationPaths: ApplicationPathsType = {
  Login: `auth/${LoginActions.Login}`,
  Register: `auth/${LoginActions.Register}`,
  Profile: `auth/${LoginActions.Profile}`,
  LoginFailed: `auth/${LoginActions.LoginFailed}`,
  ForgotPassword: `auth/${LoginActions.ForgotPassword}`,
  LoginPathComponents: [],
  RegisterPathComponents: [],
  ProfilePathComponents: [],
  ForgotPasswordComponents: [],
};

applicationPaths = {
  ...applicationPaths,
  LoginPathComponents: applicationPaths.Login.split('/'),
  RegisterPathComponents: applicationPaths.Register.split('/'),
  ProfilePathComponents: applicationPaths.Profile.split('/'),
  ForgotPasswordComponents: applicationPaths.ForgotPassword.split('/'),
};

interface ApplicationPathsType {
  readonly Login: string;
  readonly LoginFailed: string;
  readonly Register: string;
  readonly Profile: string;
  readonly ForgotPassword: string;
  readonly LoginPathComponents: string[];
  readonly RegisterPathComponents: string[];
  readonly ProfilePathComponents: string[];
  readonly ForgotPasswordComponents: string[];
}

export const ApplicationPaths: ApplicationPathsType = applicationPaths;