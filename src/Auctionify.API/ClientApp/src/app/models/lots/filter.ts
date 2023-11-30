export interface FilterLot {
    minimumPrice: number | null;
    maximumPrice: number | null;
    categoryId: number | null;
    location: string | null;
    lotStatuses: number[] | null;
    sortDir: string | null;
    sortField: string | null;
}
