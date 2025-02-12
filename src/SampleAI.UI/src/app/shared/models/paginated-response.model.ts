export interface PaginatedResponseModel<T> {
  data: T[],
  total: number,
}
