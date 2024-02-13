export interface SellerModel {
    firstName: string | null;
    lastName: string | null;
    email: string;
    phoneNumber: string | null;
    aboutMe: string | null;
    profilePictureUrl: string | null;
    createdLotsCount: number | null;
    finishedLotsCount: number | null;
    averageRate: number;
    ratesCount: number | null;
    starCounts?: { [key: number]: number };
    isPro: boolean;
}

export interface BuyerModel {
    firstName: string | null;
    lastName: string | null;
    email: string;
    phoneNumber: string | null;
    aboutMe: string | null;
    profilePictureUrl: string | null;
    createdLotsCount: number | null;
    finishedLotsCount: number | null;
    averageRate: number;
    ratesCount: number | null;
    starCounts?: { [key: number]: number };
}

export interface GetUserById {
    firstName: string | null;
    lastName: string | null;
    email: string;
    phoneNumber: string | null;
    aboutMe: string | null;
    profilePictureUrl: string | null;
    createdLotsCount: number | null;
    finishedLotsCount: number | null;
    averageRate: number;
    ratesCount: number | null;
    starCounts?: { [key: number]: number };
    isDeleted: boolean;
}

export interface UpdateUserProfileModel {
    firstName: string | null;
    lastName: string | null;
    phoneNumber: string | null;
    aboutMe: string | null;
    profilePicture: File | null;
    deleteProfilePicture: boolean;
}

export interface UserDto {
    firstName: string | null;
    lastName: string | null;
    email: string;
    phoneNumber: string | null;
    profilePicture: string | null;
}
