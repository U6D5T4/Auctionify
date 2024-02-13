export interface CreateLotModel {
    title: string;
    description: string;
    startingPrice: number | null;
    startDate: Date | null;
    endDate: Date | null;
    categoryId: number | null;
    city: string;
    state: string | null;
    country: string;
    address: string;
    latitude: string;
    longitude: string;
    currencyId: number | null;
    photos: File[] | null;
    additionalDocuments: File[] | null;
    isDraft: boolean;
}

export interface UpdateLotModel {
    id: number;
    title: string;
    description: string;
    startingPrice: number | null;
    startDate: Date | null;
    endDate: Date | null;
    categoryId: number | null;
    city: string;
    state: string | null;
    country: string;
    address: string;
    latitude: string;
    longitude: string;
    currencyId: number | null;
    photos: File[] | null;
    additionalDocuments: File[] | null;
    isDraft: boolean;
}
