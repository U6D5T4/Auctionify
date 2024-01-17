export interface SellerModel {
    firstName: string | null,
    lastName: string | null,
    email: string,
    phoneNumber: string | null,
    aboutMe: string | null,
    profilePictureUrl: string | null,
    createdLotsCount: number | null,
    finishedLotsCount: number | null,
}

export interface BuyerModel {
    firstName: string | null,
    lastName: string | null,
    email: string,
    phoneNumber: string | null,
    aboutMe: string | null,
    profilePictureUrl: string | null,
    createdLotsCount: number | null,
    finishedLotsCount: number | null,
}

export interface UpdateUserProfileModel {
    firstName: string | null,
    lastName: string | null,
    phoneNumber: string | null,
    aboutMe: string | null,
    profilePicture: File | null,
    deleteProfilePicture: boolean,
}