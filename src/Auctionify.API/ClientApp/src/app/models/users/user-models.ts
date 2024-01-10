import { Rate } from '../rates/rate-models';

export interface SellerModel {
    firstName: string | null;
    lastName: string | null;
    email: string;
    phoneNumber: string | null;
    aboutMe: string | null;
    profilePictureUrl: string | null;
    createdLotsCount: number | null;
    finishedLotsCount: number | null;
    senderRates: Rate[] | null;
    receiverRates: Rate[] | null;
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
    senderRates: Rate[] | null;
    receiverRates: Rate[] | null;
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
    profilePictureUrl: string | null;
}
