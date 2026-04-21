export interface PaginatedRequest {
  page: number;
  pageSize: number;
  sortField?: string;
  sortOrder?: number;
  filters?: { [key: string]: any };
}

export interface PaginatedResponse<T> {
  items: T[];
  totalRecords: number;
  page: number;
  pageSize: number;
}
