export const ApplicationName = 'Auctionify';

export const LoginActions = {
    Login: 'login',
    Profile: 'profile',
    Register: 'register',
    RegisterRole: 'register-role',
    LoginFailed: 'login-failed',
    ForgetPassword: 'forget-password',
    ResetPassword: 'reset-password',
    EmailSent: 'email-sent',
    AfterDeletion: 'after-deletion',
};

let applicationPaths: ApplicationPathsType = {
    Login: `auth/${LoginActions.Login}`,
    Register: `auth/${LoginActions.Register}`,
    RegisterRole: `auth/${LoginActions.RegisterRole}`,
    Profile: `auth/${LoginActions.Profile}`,
    LoginFailed: `auth/${LoginActions.LoginFailed}`,
    ForgetPassword: `auth/${LoginActions.ForgetPassword}`,
    ResetPassword: `auth/${LoginActions.ResetPassword}`,
    EmailSent: `auth/${LoginActions.EmailSent}/:email`,
    AfterDeletion: `auth/${LoginActions.AfterDeletion}`,
    LoginPathComponents: [],
    RegisterPathComponents: [],
    RegisterRolePathComponent: [],
    ProfilePathComponents: [],
    ForgetPasswordComponents: [],
    ResetPasswordComponents: [],
    EmailSentComponents: [],
    AfterDeletionComponents: [],
};

applicationPaths = {
    ...applicationPaths,
    LoginPathComponents: applicationPaths.Login.split('/'),
    RegisterPathComponents: applicationPaths.Register.split('/'),
    ProfilePathComponents: applicationPaths.Profile.split('/'),
    ForgetPasswordComponents: applicationPaths.ForgetPassword.split('/'),
    ResetPasswordComponents: applicationPaths.ResetPassword.split('/'),
    RegisterRolePathComponent: applicationPaths.Profile.split('/'),
    EmailSentComponents: applicationPaths.Profile.split('/'),
    AfterDeletionComponents: applicationPaths.AfterDeletion.split('/'),
};

interface ApplicationPathsType {
    readonly Login: string;
    readonly LoginFailed: string;
    readonly Register: string;
    readonly RegisterRole: string;
    readonly Profile: string;
    readonly ForgetPassword: string;
    readonly ResetPassword: string;
    readonly EmailSent: string;
    readonly AfterDeletion: string;
    readonly LoginPathComponents: string[];
    readonly RegisterPathComponents: string[];
    readonly RegisterRolePathComponent: string[];
    readonly ProfilePathComponents: string[];
    readonly ForgetPasswordComponents: string[];
    readonly ResetPasswordComponents: string[];
    readonly EmailSentComponents: string[];
    readonly AfterDeletionComponents: string[];
}

export const ApplicationPaths: ApplicationPathsType = applicationPaths;
