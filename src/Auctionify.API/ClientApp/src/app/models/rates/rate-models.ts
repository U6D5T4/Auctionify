import { UserDto } from '../users/user-models';

export interface Rate {
    receiverId: number;
    receiver: UserDto | null;
    senderId: number;
    sender: UserDto | null;
    ratingValue: number | null;
    comment: string | null;
    creationDate: Date | null;
    starCounts?: { [key: number]: number };
}

export interface RatePaginationModel {
    pageIndex: number | null;
    pageSize: number | null;
}

export interface RateResponse {
    count: number;
    hasNext: boolean;
    hasPrevious: boolean;
    index: number;
    items: Rate[];
    pages: number;
    size: number;
}

export interface RateUserCommandModel {
    lotId: number;
    comment: string | null;
    ratingValue: number;
}
