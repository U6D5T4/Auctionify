export const ApplicationName = 'Auctionify';

export const LoginActions = {
  Login: 'login',
  Profile: 'profile',
  Register: 'register',
  RegisterRole: 'register-role',
  LoginFailed: 'login-failed',
  ForgetPassword: 'forget-password',
  ResetPassword: 'reset-password'
};

let applicationPaths: ApplicationPathsType = {
  Login: `auth/${LoginActions.Login}`,
  Register: `auth/${LoginActions.Register}`,
  RegisterRole: `auth/${LoginActions.RegisterRole}`,
  Profile: `auth/${LoginActions.Profile}`,
  LoginFailed: `auth/${LoginActions.LoginFailed}`,
  ForgetPassword: `auth/${LoginActions.ForgetPassword}`,
  ResetPassword: `auth/${LoginActions.ResetPassword}`,
  LoginPathComponents: [],
  RegisterPathComponents: [],
  RegisterRolePathComponent: [],
  ProfilePathComponents: [],
  ForgetPasswordComponents: [],
  ResetPasswordComponents: []
};

applicationPaths = {
  ...applicationPaths,
  LoginPathComponents: applicationPaths.Login.split('/'),
  RegisterPathComponents: applicationPaths.Register.split('/'),
  ProfilePathComponents: applicationPaths.Profile.split('/'),
  ForgetPasswordComponents: applicationPaths.ForgetPassword.split('/'),
  ResetPasswordComponents: applicationPaths.ResetPassword.split('/'),
  RegisterRolePathComponent: applicationPaths.Profile.split('/'),
};

interface ApplicationPathsType {
  readonly Login: string;
  readonly LoginFailed: string;
  readonly Register: string;
  readonly RegisterRole: string;
  readonly Profile: string;
  readonly ForgetPassword: string;
  readonly ResetPassword: string;
  readonly LoginPathComponents: string[];
  readonly RegisterPathComponents: string[];
  readonly RegisterRolePathComponent: string[];
  readonly ProfilePathComponents: string[];
  readonly ForgetPasswordComponents: string[];
  readonly ResetPasswordComponents: string[];
}

export const ApplicationPaths: ApplicationPathsType = applicationPaths;