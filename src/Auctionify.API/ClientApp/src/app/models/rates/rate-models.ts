import { UserDto } from '../users/user-models';

export interface Rate {
    receiver: UserDto | null;
    sender: UserDto | null;
    ratingValue: number | null;
    comment: string | null;
    creationDate: Date | null;
}

export interface RatePaginationModel {
    pageIndex: number | null;
    pageSize: number | null;
}
