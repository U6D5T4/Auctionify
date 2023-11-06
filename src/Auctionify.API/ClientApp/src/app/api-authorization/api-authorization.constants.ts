export const ApplicationName = 'Auctionify';

export const LoginActions = {
  Login: 'login',
  Profile: 'profile',
  Register: 'register',
  LoginFailed: 'login-failed',
  ForgotPassword: 'forgot-password',
  ResetPassword: 'reset-password'
};

let applicationPaths: ApplicationPathsType = {
  Login: `auth/${LoginActions.Login}`,
  Register: `auth/${LoginActions.Register}`,
  Profile: `auth/${LoginActions.Profile}`,
  LoginFailed: `auth/${LoginActions.LoginFailed}`,
  ForgotPassword: `auth/${LoginActions.ForgotPassword}`,
  ResetPassword: `auth/${LoginActions.ResetPassword}`,
  LoginPathComponents: [],
  RegisterPathComponents: [],
  ProfilePathComponents: [],
  ForgotPasswordComponents: [],
  ResetPasswordComponents: []
};

applicationPaths = {
  ...applicationPaths,
  LoginPathComponents: applicationPaths.Login.split('/'),
  RegisterPathComponents: applicationPaths.Register.split('/'),
  ProfilePathComponents: applicationPaths.Profile.split('/'),
  ForgotPasswordComponents: applicationPaths.ForgotPassword.split('/'),
  ResetPasswordComponents: applicationPaths.ResetPassword.split('/'),
};

interface ApplicationPathsType {
  readonly Login: string;
  readonly LoginFailed: string;
  readonly Register: string;
  readonly Profile: string;
  readonly ForgotPassword: string;
  readonly ResetPassword: string;
  readonly LoginPathComponents: string[];
  readonly RegisterPathComponents: string[];
  readonly ProfilePathComponents: string[];
  readonly ForgotPasswordComponents: string[];
  readonly ResetPasswordComponents: string[];
}

export const ApplicationPaths: ApplicationPathsType = applicationPaths;